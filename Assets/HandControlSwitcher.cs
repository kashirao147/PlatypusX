using UnityEngine;
using UnityEngine.UI;

public class HandControlSwitcher : MonoBehaviour
{
    public GameObject rightHandControls; // Assign your right-hand control GameObject
    public GameObject leftHandControls; // Assign your left-hand control GameObject
 
    public Button switchHandButton; // Assign your button in the Inspector

    private const string HandPreferenceKey = "HandPreference"; // Key for PlayerPrefs

    void Start()
    {
        
        // Load preference from PlayerPrefs
        string savedHand = PlayerPrefs.GetString(HandPreferenceKey, "Left"); // Default to Right

        if (savedHand == "Left")
        {
            EnableLeftHandControls();
        }
        else
        {
            EnableRightHandControls();
        }

        // Add listener to button
        
        switchHandButton.onClick.AddListener(ToggleHandControl);
    }

    void ToggleHandControl()
    {
        if (rightHandControls.activeSelf)
        {
            EnableLeftHandControls();
        }
        else
        {
            EnableRightHandControls();
        }
    }

    void EnableRightHandControls()
    {
        rightHandControls.SetActive(true);
        leftHandControls.SetActive(false);
        switchHandButton.GetComponentInChildren<Text>().text = "Switch Left Hand";
        PlayerPrefs.SetString(HandPreferenceKey, "Right");
        PlayerPrefs.Save();
    }

    void EnableLeftHandControls()
    {
        rightHandControls.SetActive(false);
        leftHandControls.SetActive(true);
        switchHandButton.GetComponentInChildren<Text>().text = "Switch Right Hand";
        PlayerPrefs.SetString(HandPreferenceKey, "Left");
        PlayerPrefs.Save();
    }
}
