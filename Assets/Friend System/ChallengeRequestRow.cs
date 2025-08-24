using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;

public class ChallengeRequestRow : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text senderNameText;   // set in the prefab
    [SerializeField] private Button acceptButton;   // set in the prefab
    [SerializeField] private Button denyButton;     // set in the prefab
    [SerializeField] private Text statusText;       // optional (e.g., "Pending", "Expired", "Active")

    // Data
    private PlayFabChallengeService.Challenge _challenge;
    private Action _onChanged;              // callback to refresh the list (optional)
    private Action<string> _toast;          // optional: show message to user

    /// <summary>
    /// Call this right after instantiating the row.
    /// </summary>
    /// <param name="challenge">Incoming challenge object (status=pending)</param>
    /// <param name="senderDisplayName">Name to show (fallback to PlayFabId if null/empty)</param>
    /// <param name="onChanged">Callback to refresh the whole list after an action</param>
    /// <param name="toast">Optional message popup callback</param>
    public void Setup(
        PlayFabChallengeService.Challenge challenge,
        string senderDisplayName,
        Action onChanged = null,
        Action<string> toast = null)
    {
        _challenge = challenge;
        _onChanged = onChanged;
        _toast     = toast;

        senderNameText.text = string.IsNullOrEmpty(senderDisplayName)
            ? challenge.from       // fallback to id if you didn't pass a name
            : senderDisplayName;

        // Wire buttons
        acceptButton.onClick.RemoveAllListeners();
        denyButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(OnAcceptClicked);
        denyButton.onClick.AddListener(OnDenyClicked);

        RefreshStateLabelAndButtons();
    }

    public void SetSenderName(string name)
    {
        senderNameText.text = name;
    }

    // --- Button handlers ---
    private void OnAcceptClicked()
    {
        SetButtonsInteractable(false);

        PlayFabChallengeService.AcceptChallenge(_challenge.id, res =>
        {
            if (!res.success)
            {
                // sender already busy or any other reason
                if (!string.IsNullOrEmpty(res.code) && res.code == "SENDER_BUSY")
                    Toast("He is in another challenge.");
                else
                    Toast(res.message ?? "Accept failed.");

                // remove this pending item locally by asking parent to refresh
                _onChanged?.Invoke();
                return;
            }

            Toast("Challenge accepted.");
            // Optionally: navigate to your game/match scene here.
            _onChanged?.Invoke();

        }, err =>
        {
            Toast("Accept failed: " + err.ErrorMessage);
            SetButtonsInteractable(true);
        });
    }

    private void OnDenyClicked()
    {
        SetButtonsInteractable(false);

        PlayFabChallengeService.DenyChallenge(_challenge.id, res =>
        {
            // Row should disappear after refresh
            Toast(res.success ? "Challenge removed." : (res.message ?? "Deny failed."));
            _onChanged?.Invoke();

        }, err =>
        {
            Toast("Deny failed: " + err.ErrorMessage);
            SetButtonsInteractable(true);
        });
    }

    // --- Helpers ---
    private void RefreshStateLabelAndButtons()
    {
        bool expired = IsExpiredUtc(_challenge.expiresAt);
        if (statusText != null)
            statusText.text = expired ? "Expired" : "Pending";

        acceptButton.interactable = !expired;
        denyButton.interactable   = true;
    }

    private void SetButtonsInteractable(bool v)
    {
        acceptButton.interactable = v;
        denyButton.interactable   = v;
    }

    private static bool IsExpiredUtc(string isoUtc)
    {
        if (string.IsNullOrEmpty(isoUtc)) return false;
        if (DateTime.TryParse(isoUtc, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt))
            return DateTime.UtcNow >= dt;
        return false;
    }

    private void Toast(string msg)
    {
        MessagePopup.ShowPopup(msg);
        if (_toast != null) _toast.Invoke(msg);
        else Debug.Log(msg);
    }
}
