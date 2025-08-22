using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
}
