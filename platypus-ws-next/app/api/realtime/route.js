// app/api/realtime/route.js
export const runtime = "edge";

/* ------------ in-memory connections ------------ */
const socketsByPid = new Map(); // PlayFabId -> Set<WebSocket>
function addSocket(pid, ws) {
  let set = socketsByPid.get(pid);
  if (!set) { set = new Set(); socketsByPid.set(pid, set); }
  set.add(ws);
  ws.__pid = pid;
}
function delSocket(ws) {
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
  for (const ws of set) if (ws.readyState === ws.OPEN) { ws.send(msg); n++; }
  return n;
}

/* ------------ PlayFab helpers ------------ */
const TITLE_ID = process.env.PLAYFAB_TITLE_ID;
const SECRET   = process.env.PLAYFAB_SECRET_KEY;

async function authTicket(sessionTicket) {
  const r = await fetch(`https://${TITLE_ID}.playfabapi.com/Server/AuthenticateSessionTicket`, {
    method: "POST",
    headers: { "Content-Type": "application/json", "X-SecretKey": SECRET },
    body: JSON.stringify({ SessionTicket: sessionTicket })
  });
  if (!r.ok) throw new Error("auth_failed");
  const j = await r.json();
  return j?.data?.UserInfo?.PlayFabId;
}

async function getChallenge(chId) {
  const r = await fetch(`https://${TITLE_ID}.playfabapi.com/Server/GetSharedGroupData`, {
    method: "POST",
    headers: { "Content-Type": "application/json", "X-SecretKey": SECRET },
    body: JSON.stringify({ SharedGroupId: chId, Keys: ["meta"] })
  });
  if (!r.ok) return null;
  const j = await r.json();
  const meta = j?.data?.Data?.meta?.Value;
  try { return meta ? JSON.parse(meta) : null; } catch { return null; }
}

function isWebSocketUpgrade(req) {
  const up = req.headers.get("upgrade") || req.headers.get("Upgrade");
  const conn = req.headers.get("connection") || req.headers.get("Connection") || "";
  const key = req.headers.get("sec-websocket-key");
  return !!key || (up && up.toLowerCase() === "websocket") || /\bupgrade\b/i.test(conn);
}

export async function GET(req) {
  // Non-WS → health JSON
  if (!isWebSocketUpgrade(req)) {
    return new Response(JSON.stringify({ ok: true, connected: socketsByPid.size }), {
      status: 200, headers: { "content-type": "application/json" }
    });
  }

  // IMPORTANT: use numeric indices
  const pair = new WebSocketPair();
  const client = pair[0];
  const server = pair[1];

  server.accept();

  server.addEventListener("message", async (evt) => {
    let msg;
    try {
      const raw = typeof evt.data === "string" ? evt.data : new TextDecoder().decode(evt.data);
      msg = JSON.parse(raw);
    } catch {
      server.send(JSON.stringify({ type: "error", reason: "bad_json" })); return;
    }

    if (msg.type === "auth") {
      if (!msg.sessionTicket) { server.close(1008, "missing_session_ticket"); return; }
      try {
        const pid = await authTicket(msg.sessionTicket);
        addSocket(pid, server);
        server.send(JSON.stringify({ type: "auth_ok", playFabId: pid }));
      } catch {
        server.close(1008, "auth_failed");
      }
      return;
    }

    if (!server.__pid) { server.close(1008, "unauthenticated"); return; }

    if (msg.type === "notify_challenge" && msg.challengeId && msg.targetId) {
      const meta = await getChallenge(msg.challengeId);
      if (!meta || meta.status !== "pending" || !meta.participants?.includes(msg.targetId)) {
        server.send(JSON.stringify({ type: "notify_error", reason: "verify_failed" })); return;
      }
      const delivered = pushTo(msg.targetId, {
        type: "challenge_request",
        id: meta.id,
        from: meta.from,
        expiresAt: meta.expiresAt
      });
      server.send(JSON.stringify({ type: "notify_ok", id: meta.id, delivered }));
      return;
    }

    if (msg.type === "ping") {
      server.send(JSON.stringify({ type: "pong", t: Date.now() })); return;
    }

    server.send(JSON.stringify({ type: "error", reason: "unknown_type" }));
  });

  server.addEventListener("close", () => delSocket(server));
  server.addEventListener("error", () => delSocket(server));

  return new Response(null, { status: 101, webSocket: client });
}
