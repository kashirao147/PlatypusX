using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.UI;

public static class PlayFabChallengeService
{

    private static readonly Dictionary<string,string> _nameCache = new Dictionary<string,string>();
    public static void LoadFriendNamesMap(System.Action<Dictionary<string,string>> done)
    {
        var req = new GetFriendsListRequest {
            ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
        };
        PlayFabClientAPI.GetFriendsList(req, r => {
            var map = new Dictionary<string,string>();
            foreach (var f in r.Friends)
            {
                var id   = f.FriendPlayFabId;
                var name = f.Profile?.DisplayName ?? f.TitleDisplayName ?? id;
                map[id] = name;
                _nameCache[id] = name; // seed cache
            }
            done(map);
        }, err => {
            Debug.LogWarning("LoadFriendNamesMap failed: " + err.ErrorMessage);
            done(new Dictionary<string,string>());
        });
    }

    public static void ResolveDisplayName(string playFabId,
                                       Dictionary<string, string> friendNames,
                                       System.Action<string> done)
    {
        // 1) Provided map
        if (friendNames != null &&
            friendNames.TryGetValue(playFabId, out var nice) &&
            !string.IsNullOrEmpty(nice))
        { done(nice); return; }

        // 2) Local cache
        if (_nameCache.TryGetValue(playFabId, out var cached) &&
            !string.IsNullOrEmpty(cached))
        { done(cached); return; }

        // 3) Fetch from PlayFab
        var req = new GetPlayerProfileRequest
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
        };
        PlayFabClientAPI.GetPlayerProfile(req, r =>
        {
            var dn = r.PlayerProfile?.DisplayName;
            if (!string.IsNullOrEmpty(dn)) _nameCache[playFabId] = dn;
            done(!string.IsNullOrEmpty(dn) ? dn : playFabId);   // fallback to id
        }, err =>
        {
            Debug.LogWarning("ResolveDisplayName failed: " + err.ErrorMessage);
            done(playFabId); // fallback
        });
    }
    
    // ----- DTOs (match the CloudScript JSON) -----

    private static int ToInt(object v)
    {
        if (v == null) return 0;
        if (v is int i) return i;
        if (v is long l) return (int)l;
        if (v is double d) return (int)Math.Floor(d);
        if (v is float f) return (int)Math.Floor(f);
        if (int.TryParse(v.ToString(), out var x)) return x;
        return 0;
    }



    [Serializable]
    public class UpdateScoreResult
    {
        public bool success;
        public string message;
        public Challenge challenge;
    }
    [Serializable]
    public class ResolveResult
    {
        public bool success;
        public string state;     // none | active | expired
        public string outcome;   // win | lose | draw (only when expired)
        public string winnerId;  // null or PlayFabId
        public int myScore;      // when active/expired
        public int oppScore;     // when active/expired
        public Challenge challenge;
        public string message;
    }

    [Serializable] public class Challenge {
        public string id;
        public string from;
        public string to;
        public string status;      // pending | active | expired | ...
        public string createdAt;
        public string expiresAt;
        public string acceptedAt;  // optional
        public Dictionary<string, object> scores; // <— NEW
    }


    [Serializable]
    public class ReopenResult
    {
        public bool success;
        public string state; // "none" | "active" | "expired"
        public Challenge challenge; // present for active/expired
        public string message;
    }


    [Serializable]
    public class ListResult
    {
        public bool success;
        public List<Challenge> incoming;
        public List<Challenge> outgoing;
        public Challenge active;
        public string message;
    }

    [Serializable]
    public class ActionResult
    {
        public bool success;
        public string message;
        public string code;        // e.g., SENDER_BUSY, YOU_BUSY
        public Challenge challenge;
    }

    // Utility: convert CloudScript result.FunctionResult => T
    private static T As<T>(ExecuteCloudScriptResult r)
    {
        // FunctionResult is already a dictionary; serialize then deserialize for safety

        var json = PlayFabSimpleJson.SerializeObject(r.FunctionResult);
        return PlayFabSimpleJson.DeserializeObject<T>(json);
    }

    // Timezone offset in minutes (e.g., +300 for UTC+05:00)
    private static int LocalTzOffsetMinutes()
    {
        return (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalMinutes;
    }

    // ---------------- API ----------------

    /// Send a challenge to a friend (by PlayFabId).
    // public static void SendChallenge(string targetPlayFabId, Action<ActionResult> done, Action<PlayFabError> fail = null)
    // {
    //     var request = new ExecuteCloudScriptRequest
    //     {
    //         FunctionName = "SendChallenge",
    //         FunctionParameter = new Dictionary<string, object> {
    //             { "TargetPlayFabId", targetPlayFabId },
    //             { "tzOffsetMinutes", LocalTzOffsetMinutes() }
    //         },
    //         GeneratePlayStreamEvent = false
    //     };

    //     PlayFabClientAPI.ExecuteCloudScript(request, r =>
    //     {
    //         var res = As<ActionResult>(r);
    //         done?.Invoke(res);
    //     }, err => { fail?.Invoke(err); });
    // }
    
    

    public static void SendChallenge(
        string targetPlayFabId,
        Action<ActionResult> done,
        Action<PlayFabError> fail = null,
        int? testMinutes = 10)                // <— NEW
    {
        var fp = new Dictionary<string, object> {
            { "TargetPlayFabId", targetPlayFabId },
            { "tzOffsetMinutes", LocalTzOffsetMinutes() }
        };
        if (testMinutes.HasValue && testMinutes.Value > 0)
            fp["TestMinutes"] = testMinutes.Value;

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SendChallenge",
            FunctionParameter = fp,
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, r =>
        {
            var res = As<ActionResult>(r);
            done?.Invoke(res);
        }, err => { fail?.Invoke(err); });
    }


    /// List my challenges (incoming + outgoing + active).
    public static void ListChallenges(string direction, Action<ListResult> done, Action<PlayFabError> fail = null)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "ListChallenges",
            FunctionParameter = new Dictionary<string, object> { { "Direction", direction } }, // "incoming", "outgoing", "both"
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, r =>
        {
            var res = As<ListResult>(r);
            done?.Invoke(res);
        }, err => { fail?.Invoke(err); });
    }

    /// Accept a pending incoming challenge.
    public static void AcceptChallenge(string challengeId, Action<ActionResult> done, Action<PlayFabError> fail = null)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "AcceptChallenge",
            FunctionParameter = new Dictionary<string, object> { { "ChallengeId", challengeId } },
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, r =>
        {
            var res = As<ActionResult>(r);
            done?.Invoke(res);
        }, err => { fail?.Invoke(err); });
    }

    /// Deny (or cancel) a pending challenge — removes it from both sides.
    public static void DenyChallenge(string challengeId, Action<ActionResult> done, Action<PlayFabError> fail = null)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "DenyChallenge",
            FunctionParameter = new Dictionary<string, object> { { "ChallengeId", challengeId } },
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, r =>
        {
            var res = As<ActionResult>(r);
            done?.Invoke(res);
        }, err => { fail?.Invoke(err); });
    }

    /// Get my active challenge (null if none). Also auto-clears if expired.
    public static void GetMyActive(Action<ActionResult> done, Action<PlayFabError> fail = null)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "GetMyActiveChallenge",
            FunctionParameter = new Dictionary<string, object>(),
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(request, r =>
        {
            // Reuse ActionResult to carry "active" inside "challenge" for convenience
            var json = PlayFabSimpleJson.SerializeObject(r.FunctionResult);
            // Transform { success, active } -> ActionResult { challenge = active }
            var dict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(json);
            var success = dict.ContainsKey("success") && (bool)dict["success"];
            Challenge active = null;
            if (dict.ContainsKey("active") && dict["active"] != null)
            {
                var aJson = PlayFabSimpleJson.SerializeObject(dict["active"]);
                active = PlayFabSimpleJson.DeserializeObject<Challenge>(aJson);
            }
            done?.Invoke(new ActionResult { success = success, challenge = active, message = success ? "ok" : "error" });
        }, err => { fail?.Invoke(err); });
    }

    // (Optional) If your game completes/aborts a match, call this.
    public static void ClearMyActive(Action<ActionResult> done = null, Action<PlayFabError> fail = null)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "ClearMyActiveChallenge",
            FunctionParameter = new Dictionary<string, object>(),
            GeneratePlayStreamEvent = false
        };
        PlayFabClientAPI.ExecuteCloudScript(request, r =>
        {
            var res = As<ActionResult>(r);
            done?.Invoke(res);
        }, err => fail?.Invoke(err));
    }














    /// Shows current active challenge: opponent name only, and wires Quit button.
    /// - opponentNameText: where to print the opponent's name (falls back to PlayFabId)
    /// - quitButton: will call ClearMyActive and then re-run this binding
    /// - container: optional panel to hide if there's no active challenge
    /// - friendNames: optional map PlayFabId -> DisplayName
    /// - onQuitDone: optional callback after quitting (e.g., refresh your lists)
   public static void ShowActiveWithScoresAndQuit(
    Text opponentNameText,
    Text myScoreText,
    Text oppScoreText,
    Button quitButton,
    GameObject container = null,
    Dictionary<string,string> friendNames = null,
    Action onQuitDone = null)
    {
        // First, resolve (non-mutating) status and get the active object.
        // Use GetMyActive because you said you only want Resolve() to clear expired.
        var req = new ExecuteCloudScriptRequest {
            FunctionName = "GetMyActiveChallenge",
            FunctionParameter = new Dictionary<string, object>(),
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(req, r =>
        {
            var json = PlayFab.Json.PlayFabSimpleJson.SerializeObject(r.FunctionResult);
            var dict = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(json);

            Challenge active = null;
            if (dict.TryGetValue("active", out var activeObj) && activeObj != null)
            {
                var aJson = PlayFab.Json.PlayFabSimpleJson.SerializeObject(activeObj);
                active = PlayFab.Json.PlayFabSimpleJson.DeserializeObject<Challenge>(aJson);
            }

            if (active == null)
            {
                if (container) container.SetActive(false);
                if (opponentNameText) opponentNameText.text = "No active challenge";
                if (myScoreText)  myScoreText.text  = "My Score : 0";
                if (oppScoreText) oppScoreText.text = "Opponent Score : 0";
                if (quitButton) quitButton.interactable = false;
                return;
            }

            if (container) container.SetActive(true);
            if (quitButton) quitButton.interactable = true;

            // Resolve opponent + scores
            EnsureMyId(myId =>
            {

                
                var oppId = (active.from == myId) ? active.to : active.from;
                // old (map-only, fails if not present)
                var display = (friendNames != null && friendNames.TryGetValue(oppId, out var nice) && !string.IsNullOrEmpty(nice))
                                ? nice : oppId;
                
                if (opponentNameText) opponentNameText.text = "Challenge with "+display;
                // new (robust)
                ResolveDisplayName(oppId, friendNames, name => {
                    if (opponentNameText) opponentNameText.text = name;
                });

                // var display = (friendNames != null && friendNames.TryGetValue(oppId, out var nice) && !string.IsNullOrEmpty(nice))
                //             ? nice : oppId;

                

                int myScore = 0, oppScore = 0;
                if (active.scores != null)
                {
                    if (active.scores.TryGetValue(myId, out var mv))   myScore  = ToInt(mv);
                    if (active.scores.TryGetValue(oppId, out var ov))  oppScore = ToInt(ov);
                }
                if (myScoreText)  myScoreText.text  = "My Score :"+myScore.ToString();
                if (oppScoreText) oppScoreText.text = "Opponent Score : "+oppScore.ToString();

                // Wire Quit (your server already: remove me; mark other as expired)
                if (quitButton)
                {
                    quitButton.onClick.RemoveAllListeners();
                    quitButton.onClick.AddListener(() =>
                    {
                        quitButton.interactable = false;
                        ClearMyActive(_ =>
                        {
                            // Rebind UI after quit so it hides or shows correct state
                            ShowActiveWithScoresAndQuit(opponentNameText, myScoreText, oppScoreText,
                                                        quitButton, container, friendNames, onQuitDone);
                            onQuitDone?.Invoke();
                        },
                        err =>
                        {
                            Debug.LogError(err.GenerateErrorReport());
                            quitButton.interactable = true;
                        });
                    });
                }
            },
            err => {
                Debug.LogError(err.GenerateErrorReport());
                if (opponentNameText) opponentNameText.text = "Active challenge";
            });
        },
        err => {
            Debug.LogError(err.GenerateErrorReport());
            if (container) container.SetActive(false);
        });
    }






    // Cache my PlayFabId so we don't fetch it every time
    private static string _myIdCache;

    private static void EnsureMyId(System.Action<string> done, System.Action<PlayFabError> fail = null)
    {
        if (!string.IsNullOrEmpty(_myIdCache)) { done?.Invoke(_myIdCache); return; }

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), r =>
        {
            _myIdCache = r.AccountInfo.PlayFabId;
            done?.Invoke(_myIdCache);
        },
        e => { fail?.Invoke(e); });
    }



    public static void CheckChallengeOnReopen(Action<ReopenResult> done, Action<PlayFabError> fail = null)
    {
        var req = new ExecuteCloudScriptRequest
        {
            FunctionName = "CheckChallengeOnReopen",
            FunctionParameter = new Dictionary<string, object>(),
            GeneratePlayStreamEvent = false
        };

        PlayFabClientAPI.ExecuteCloudScript(req, r =>
        {
            var res = As<ReopenResult>(r);
            done?.Invoke(res);
        }, err => fail?.Invoke(err));
    }





    public static void UpdateActiveScore(int myScore, int? opponentScore = null, bool appendHistory = false, Action<UpdateScoreResult> done = null, Action<PlayFabError> fail = null)
    {
        var args = new Dictionary<string, object> {
            { "MyScore", myScore },
            { "AppendHistory", appendHistory }
        };
        if (opponentScore.HasValue) args["OpponentScore"] = opponentScore.Value;

        var req = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateActiveScore",
            FunctionParameter = args,
            GeneratePlayStreamEvent = false
        };
        PlayFabClientAPI.ExecuteCloudScript(req, r =>
        {
            var res = As<UpdateScoreResult>(r);
            done?.Invoke(res);
        }, err => fail?.Invoke(err));
    }
    

    public static void ResolveChallengeOnReopen(Action<ResolveResult> done, Action<PlayFabError> fail = null)
    {
        var req = new ExecuteCloudScriptRequest {
            FunctionName = "ResolveChallengeOnReopen",
            FunctionParameter = new Dictionary<string, object>(),
            GeneratePlayStreamEvent = false
        };
        PlayFabClientAPI.ExecuteCloudScript(req, r => {
            var res = As<ResolveResult>(r);
            done?.Invoke(res);
        }, err => fail?.Invoke(err));
    }





}
