using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeRequestRow : MonoBehaviour
{
    [SerializeField] private Text fromNameText;
    [SerializeField] private Text expiresText;
    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button denyBtn;

    private string _challengeId;

    public void Bind(string challengeId, string fromName, string expiresPretty, Action onAccept, Action onDeny)
    {
        _challengeId = challengeId;
        if (fromNameText)  fromNameText.text = fromName;
        if (expiresText)   expiresText.text = expiresPretty;

        if (acceptBtn) {
            acceptBtn.onClick.RemoveAllListeners();
            acceptBtn.onClick.AddListener(() => onAccept?.Invoke());
        }
        if (denyBtn) {
            denyBtn.onClick.RemoveAllListeners();
            denyBtn.onClick.AddListener(() => onDeny?.Invoke());
        }
    }
}


 