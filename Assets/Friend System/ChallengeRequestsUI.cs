using System;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;



 


public class ChallengeRequestsUI : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private Transform content;           // ScrollView Content
    [SerializeField] private GameObject rowPrefab;        // ChallengeRequestRow prefab
    [SerializeField] private TMP_Text emptyStateText;     // Optional: "No requests"

    private void OnEnable()
    {
        RefreshIncomingRequests();
    }

    public void RefreshIncomingRequests()
    {
        ClearList();

        var req = new ExecuteCloudScriptRequest
        {
            FunctionName = "ListMyChallengeRequests",
            FunctionParameter = new { Direction = "incoming" }
        };

        PlayFabClientAPI.ExecuteCloudScript(req, r =>
        {
            if (r.FunctionResult == null)
            {
                ShowEmpty("No requests");
                return;
            }
            var res = JsonConvert.DeserializeObject<ListRequestsResponse>(r.FunctionResult.ToString());
            if (res == null || res.incoming == null || res.incoming.Count == 0)
            {
                ShowEmpty("No requests");
                return;
            }

            if (emptyStateText) emptyStateText.gameObject.SetActive(false);

            foreach (var ch in res.incoming)
            {
                var go = Instantiate(rowPrefab, content);
                var row = go.GetComponent<ChallengeRequestRow>();
                if (row == null) row = go.AddComponent<ChallengeRequestRow>();

                string who = ch.from; // Optionally map to display name if you have it cached
                string expires = FriendlyExpires(ch.expiresAt);

                row.Bind(
                    ch.id,
                    who,
                    expires,
                    onAccept: () => AcceptChallenge(ch.id),
                    onDeny: () => DenyChallenge(ch.id)
                );
            }
        }, e =>
        {
            ShowEmpty("Failed to load requests");
            Debug.LogError(e.GenerateErrorReport());
        });
    }

    private void AcceptChallenge(string challengeId)
    {
        Exec<ChallengeResponse>("AcceptChallenge", new { ChallengeId = challengeId }, resp =>
        {
            if (resp.success)
            {
                // Move player into your "Active Challenge" flow (e.g., open score screen)
                Debug.Log($"Accepted {resp.challenge.id}, status={resp.challenge.status}");
                RefreshIncomingRequests();
            }
            else
            {
                MessagePopup.ShowPopup(resp.message);
            }
        });
    }

    private void DenyChallenge(string challengeId)
    {
        Exec<ChallengeResponse>("DenyChallenge", new { ChallengeId = challengeId }, resp =>
        {
            if (resp.success)
            {
                Debug.Log($"Denied {resp.challenge.id}");
                RefreshIncomingRequests();
            }
            else
            {
                MessagePopup.ShowPopup(resp.message);
            }
        });
    }

    // --- helpers ---
    private void Exec<T>(string fn, object param, Action<T> cb)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
        {
            FunctionName = fn,
            FunctionParameter = param
        }, r =>
        {
            var json = r.FunctionResult?.ToString();
            var data = string.IsNullOrEmpty(json) ? default : JsonConvert.DeserializeObject<T>(json);
            cb?.Invoke(data);
        }, e =>
        {
            MessagePopup.ShowPopup(e.GenerateErrorReport());
        });
    }

    private void ClearList()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);
    }

    private void ShowEmpty(string msg)
    {
        ClearList();
        if (emptyStateText)
        {
            emptyStateText.text = msg;
            emptyStateText.gameObject.SetActive(true);
        }
    }

    private static string FriendlyExpires(string expiresAtIso)
    {
        if (string.IsNullOrEmpty(expiresAtIso)) return "—";
        if (!DateTime.TryParse(expiresAtIso, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var expUtc))
            return expiresAtIso;

        var remaining = expUtc - DateTime.UtcNow;
        if (remaining.TotalSeconds <= 0) return "expired";
        if (remaining.TotalHours >= 1)
            return $"{Mathf.FloorToInt((float)remaining.TotalHours)}h {remaining.Minutes}m left";
        return $"{remaining.Minutes}m {remaining.Seconds}s left";
    }
}





