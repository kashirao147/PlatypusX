using System;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class VercelRealtime : MonoBehaviour
{
    public static VercelRealtime I;

    [SerializeField] private string wsUrl = "wss://YOUR-APP.vercel.app/api/realtime";
    private WebSocket ws;
    private string _sessionTicket;
    private string _playFabId;

    void Awake() {
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    // Call this after your PlayFab login succeeds
    public void OnPlayFabLogin(LoginResult res) {
        _playFabId = res.PlayFabId;
        _sessionTicket = res.SessionTicket;
        _ = ConnectAndAuth();
    }

    async Task ConnectAndAuth() {
        ws = new WebSocket(wsUrl);

        ws.OnOpen += () => {
            var auth = JsonConvert.SerializeObject(new { type = "auth", sessionTicket = _sessionTicket });
            ws.SendText(auth);
        };

        ws.OnMessage += (bytes) => {
            var json = Encoding.UTF8.GetString(bytes);
            Handle(json);
        };

        ws.OnError += (err) => Debug.LogError("WS error: " + err);
        ws.OnClose += (code) => Debug.LogWarning("WS closed: " + code);

        await ws.Connect();
    }

    void Update() { ws?.DispatchMessageQueue(); }

    // Challenger calls this right after CloudScript creates the challenge
    public void NotifyChallenge(string targetPlayFabId, string challengeId) {
        if (ws == null || ws.State != WebSocketState.Open) return;
        var msg = JsonConvert.SerializeObject(new {
            type = "notify_challenge",
            targetId = targetPlayFabId,
            challengeId = challengeId
        });
        ws.SendText(msg);
    }

    private void Handle(string json) {
        var m = JsonConvert.DeserializeObject<Msg>(json);
        switch (m.type) {
            case "auth_ok":
                Debug.Log("WS authed as " + m.playFabId);
                break;

            case "challenge_request":
                // Target receives instant popup; optionally confirm via CloudScript
                Exec<ChallengeResponse>("GetChallenge", new { ChallengeId = m.id }, r => {
                    if (r != null && r.success && r.challenge.status == "pending")
                    {
                        // Show your Accept/Deny UI for this single request
                        Debug.Log("Challenge Recieve " + r.challenge.from + "  " + r.challenge.createdAt);
                        //ChallengeRequestRow.Instance.ShowOne(r.challenge);
                    }
                });
                break;

            case "notify_ok":
                Debug.Log($"Notify delivered to {m.delivered} socket(s) for id={m.id}");
                break;

            case "notify_error":
                Debug.LogWarning("Notify error: " + json);
                break;
        }
    }

    private void Exec<T>(string fn, object param, Action<T> cb) {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest {
            FunctionName = fn, FunctionParameter = param
        }, r => {
            var j = r.FunctionResult?.ToString();
            cb?.Invoke(string.IsNullOrEmpty(j) ? default : JsonConvert.DeserializeObject<T>(j));
        }, e => Debug.LogError(e.GenerateErrorReport()));
    }

    [Serializable] class Msg {
        public string type;
        public string playFabId;
        public string id;
        public string from;
        public string expiresAt;
        public int delivered;
    }

    [Serializable] public class ChallengeResponse { public bool success; public string message; public ChallengeMeta challenge; }
    [Serializable] public class ChallengeMeta {
        public string id, type, from, to, createdAt, expiresAt, status, acceptedAt, deniedAt, winner;
    }
}
