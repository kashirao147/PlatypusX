# Realtime PlayFab WS (Next.js Edge)

This is a tiny Next.js app that exposes a WebSocket endpoint at `/api/realtime`
which authenticates with PlayFab SessionTicket and lets a challenger notify the
target player instantly about a challenge request.

## Deploy to Vercel

1. Create a new Vercel project from this folder.
2. Add **Environment Variables** (Production + Preview):
   - `PLAYFAB_TITLE_ID` = your title id (e.g. F7FEA)
   - `PLAYFAB_SECRET_KEY` = your Title Secret Key
3. Deploy.

Check health in a browser tab:
```
https://<your-domain>.vercel.app/api/realtime
```
You should see JSON like `{ "ok": true, "connected": 0 }`.

## Test in browser console
```js
const ws = new WebSocket("wss://<your-domain>.vercel.app/api/realtime");
ws.onopen = () => { console.log("OPEN"); ws.send(JSON.stringify({type:"ping"})); };
ws.onmessage = e => console.log("MSG:", e.data);
ws.onerror = e => console.log("ERR", e);
ws.onclose = e => console.log("CLOSE", e.code, e.reason);
```

## Unity usage (NativeWebSocket)
```csharp
// after PlayFab login
FindObjectOfType<VercelWS>()?.OnPlayFabLogin(loginResult);

// creator after CreateHighScoreChallengeRequest success
VercelWS.I.NotifyChallenge(TargetPlayFabId, resp.challenge.id);
```

See earlier messages for the `VercelWS` script implementation.
