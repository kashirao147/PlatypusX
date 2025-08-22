using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;

public class FriendCard : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Text nameText;       // drag your "SARA" TMP text here
    [SerializeField] private Button challengeButton;  // drag the green "CHALLENGE" button here

    // Stored for later use (e.g., when button is pressed)
    public string FriendPlayFabId { get; private set; }

    private Action<string> _onChallenge;

    /// <summary>
    /// Initialize the card with PlayFab data and a click callback.
    /// </summary>
    public void Init(FriendInfo info, Action<string> onChallengeClicked)
    {
        FriendPlayFabId = info.FriendPlayFabId;
        _onChallenge = onChallengeClicked;

        // Pick the best available display name
        string displayName =
            !string.IsNullOrEmpty(info.TitleDisplayName) ? info.TitleDisplayName :
            !string.IsNullOrEmpty(info.Username)         ? info.Username :
            FriendPlayFabId; // fallback

        if (nameText != null) nameText.text = displayName;

        if (challengeButton != null)
        {
            challengeButton.onClick.RemoveAllListeners();
            challengeButton.onClick.AddListener(() => _onChallenge?.Invoke(FriendPlayFabId));
        }
    }
}
