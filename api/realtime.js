export const config = { runtime: "edge" };

/** ---------- Simple in-memory connection registry ----------
 * NOTE: Works great for simple setups. For multi-instance scale,
 * back this with Redis pub/sub (e.g., Upstash).
 */
const socketsByPid = new Map(); // PlayFabId -> Set<WebSocket>

function addSocket(pid, ws) {
  let set = socketsByPid.get(pid);
  if (!set) { set = new Set(); socketsByPid.set(pid, set); }
  set.add(ws);
  // attach pid to ws for cleanup
  // (Edge runtime allows custom props on WebSocket objects)
  ws.__pid = pid;
}
function removeSocket(ws) {
  const pid = ws.__pid;
  if (!pid) return;
  const set = socketsByPid.get(pid);
  if (!set) return;
  set.delete(ws);
  if (set.size === 0) socketsByPid.delete(pid);
}
function pushTo(pid, payload) {
  const set = socketsByPid.get(pid);
  if (!set) return 0;
  const msg = JSON.stringify(payload);
  let n = 0;
  for (const ws of set) {
    if (ws.readyState === ws.OPEN) { ws.send(msg); n++; }
  }
  return n;
}

/** ---------- PlayFab helpers (Edge-safe fetch) ---------- */
const TITLE_ID = process.env.PLAYFAB_TITLE_ID;        // e.g. "12345"
const SECRET   = process.env.PLAYFAB_SECRET_KEY;      // Title secret key

async function authenticateSessionTicket(sessionTicket) {
  const url = `https://${TITLE_ID}.playfabapi.com/Server/AuthenticateSessionTicket`;
  const r = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-SecretKey": SECRET
    },
    body: JSON.stringify({ SessionTicket: sessionTicket })
  });
  if (!r.ok) throw new Error("Auth failed");
  const data = await r.json();
  return data.data?.UserInfo?.PlayFabId;
}

async function verifyChallengeExists(challengeId) {
  const url = `https://${TITLE_ID}.playfabapi.com/Server/GetSharedGroupData`;
  const r = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "X-SecretKey": SECRET
    },
    body: JSON.stringify({ SharedGroupId: challengeId, Keys: ["meta"] })
  });
  if (!r.ok) return null;
  const data = await r.json();
  const metaStr = data.data?.Data?.meta?.Value;
  if (!metaStr) return null;
  try { return JSON.parse(metaStr); } catch { return null; }
}

/** ---------- WS handler ---------- */
export default async function handler(req) {
  // If not an upgrade request, show a tiny status page
  if (req.headers.get("upgrade") !== "websocket") {
    return new Response(JSON.stringify({ ok: true, connected: socketsByPid.size }), {
      status: 200,
      headers: { "content-type": "application/json" }
    });
  }

  const pair = new WebSocketPair();
  const [client, server] = Object.values(pair);

  server.accept();

  server.addEventListener("message", async (evt) => {
    let msg;
    try { msg = JSON.parse(typeof evt.data === "string" ? evt.data : new TextDecoder().decode(evt.data)); }
    catch { server.send(JSON.stringify({ type: "error", reason: "bad_json" })); return; }

    // 1) Authenticate with PlayFab SessionTicket
    if (msg.type === "auth") {
      if (!msg.sessionTicket) {
        server.send(JSON.stringify({ type: "auth_error", reason: "missing_session_ticket" }));
        server.close(1008, "missing_session_ticket");
        return;
      }
      try {
        const pid = await authenticateSessionTicket(msg.sessionTicket);
        if (!pid) throw new Error("no_pid");
        addSocket(pid, server);
        server.send(JSON.stringify({ type: "auth_ok", playFabId: pid }));
      } catch (e) {
        server.send(JSON.stringify({ type: "auth_error", reason: "invalid_session" }));
        server.close(1008, "auth_failed");
      }
      return;
    }

    // Require auth for other messages
    if (!server.__pid) {
      server.send(JSON.stringify({ type: "error", reason: "unauthenticated" }));
      server.close(1008, "unauthenticated");
      return;
    }

    // 2) Challenger notifies target to pop UI
    // { type:"notify_challenge", challengeId, targetId }
    if (msg.type === "notify_challenge" && msg.challengeId && msg.targetId) {
      try {
        // Optional but recommended: verify the challenge exists and is pending
        const meta = await verifyChallengeExists(msg.challengeId);
        if (!meta || meta.status !== "pending" || !meta.participants?.includes(msg.targetId)) {
          server.send(JSON.stringify({ type: "notify_error", reason: "verify_failed" }));
          return;
        }
        const pushed = pushTo(msg.targetId, {
          type: "challenge_request",
          id: meta.id,
          from: meta.from,
          expiresAt: meta.expiresAt
        });
        server.send(JSON.stringify({ type: "notify_ok", id: meta.id, delivered: pushed }));
      } catch {
        server.send(JSON.stringify({ type: "notify_error", reason: "exception" }));
      }
      return;
    }

    // 3) Ping/Pong
    if (msg.type === "ping") {
      server.send(JSON.stringify({ type: "pong", t: Date.now() }));
      return;
    }

    server.send(JSON.stringify({ type: "error", reason: "unknown_type" }));
  });

  server.addEventListener("close", () => removeSocket(server));
  server.addEventListener("error", () => removeSocket(server));

  // Handshake response (Edge WS upgrade)
  return new Response(null, { status: 101, webSocket: client });
}
