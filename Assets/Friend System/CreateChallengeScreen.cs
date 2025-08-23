using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateChallengeScreen : MonoBehaviour
{
    [SerializeField] private Text targetNameLabel; // optional label to show who you're challenging
    public string TargetPlayFabId { get; private set; }

    public void Open(string friendPlayFabId, string friendlyName = null)
    {
        TargetPlayFabId = friendPlayFabId;
        if (targetNameLabel) targetNameLabel.text = friendlyName ?? friendPlayFabId;

        gameObject.SetActive(true);
    }



    public void CreateChallengeRequest()
    {
        var req = new PlayFab.ClientModels.ExecuteCloudScriptRequest {
            FunctionName = "CreateHighScoreChallengeRequest",
            FunctionParameter = new {
                TargetPlayFabId = TargetPlayFabId,
                DurationHours = 4
            },
            GeneratePlayStreamEvent = true
        };

        PlayFab.PlayFabClientAPI.ExecuteCloudScript(req, r => {
            var json = r.FunctionResult?.ToString();
            var resp = JsonConvert.DeserializeObject<ChallengeResponse>(json);
            if (resp.success) {
                Debug.Log($"Request sent. ChallengeId={resp.challenge.id}");
                // creator side:
                VercelRealtime.I.NotifyChallenge(TargetPlayFabId, resp.challenge.id);

                // (Optional) start polling this id for acceptance:
                StartCoroutine(ChallengePolling.WaitForStatus(resp.challenge.id, OnChallengeStatusChanged));
            } else {
                MessagePopup.ShowPopup(resp.message);
            }
        }, e => MessagePopup.ShowPopup(e.GenerateErrorReport()));
    }

    void OnChallengeStatusChanged(ChallengeMeta meta)
    {
        // Called when status changes to active/denied/expired
        Debug.Log($"Status update: {meta.id} → {meta.status}");
        // e.g., if active -> push both players to score screen
    }

}



