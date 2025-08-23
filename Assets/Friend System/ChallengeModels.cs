
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class ChallengeResponse {
    public bool success;
    public string message;
    public ChallengeMeta challenge;
}

[System.Serializable]
public class ChallengeMeta
{
    public string id, type, from, to, createdAt, expiresAt, status, acceptedAt, deniedAt, winner;
    public Dictionary<string, int?> scores;
}


[System.Serializable]
 public class ListRequestsResponse {
    public bool success;
    public List<ChallengeMeta> incoming;
    public List<ChallengeMeta> outgoing;
}




public static class ChallengePolling
{
    public static IEnumerator WaitForStatus(string challengeId, System.Action<ChallengeMeta> onChange, float pollEvery = 3f)
    {
        string lastStatus = null;
        while (true)
        {
            var done = false;
            PlayFab.PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest
            {
                FunctionName = "GetChallenge",
                FunctionParameter = new { ChallengeId = challengeId }
            }, r =>
            {
                var json = r.FunctionResult?.ToString();
                var resp = JsonConvert.DeserializeObject<ChallengeResponse>(json);
                if (resp.success)
                {
                    var st = resp.challenge.status;
                    if (st != lastStatus && lastStatus != null)
                    {
                        onChange?.Invoke(resp.challenge);
                        done = (st == "active" || st == "denied" || st == "expired" || st == "completed");
                    }
                    lastStatus = st;
                }
            }, _ => { });
            yield return new WaitForSeconds(pollEvery);
            if (done) yield break;
        }
    }
}