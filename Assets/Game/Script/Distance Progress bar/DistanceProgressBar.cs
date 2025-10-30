using System;
using PhoenixaStudio;
using PlayFab.PfEditor;
using UnityEngine;
using UnityEngine.UI;

public class DistanceProgressBar : MonoBehaviour
{
    public static DistanceProgressBar Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text targetText;

    private float targetDistance;
    private float distanceCovered;
    private bool isActive = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (progressSlider != null)
            progressSlider.interactable = false;

        gameObject.SetActive(false); // Hide initially
    }

    /// <summary>
    /// Initializes and activates the progress bar.
    /// </summary>
    public void SetTarget(float distance)
    {
        if (progressSlider == null || targetText == null)
        {
            Debug.LogWarning("Progress bar setup missing references!");
            return;
        }

        targetDistance = Mathf.Max(distance, 0f);
        distanceCovered = 0f;

        progressSlider.minValue = 0f;
        progressSlider.maxValue = targetDistance;
        progressSlider.value = 0f;

        targetText.text = $"Target: {targetDistance:0} m";

        gameObject.SetActive(true);
        isActive = true;
    }

    private void Update()
    {
        if (!isActive)
            return;

        // Get background speed from GameManager
        if (GameManager.Instance == null)
            return;

        float speed = GameManager.Instance.Speed;

        // Increase distance based on speed * deltaTime
        distanceCovered += speed * Time.deltaTime;
        distanceCovered = Mathf.Clamp(distanceCovered, 0f, targetDistance);

        // Update progress bar and text
        progressSlider.value = distanceCovered;
        targetText.text = $"Progress: {distanceCovered:0} / {targetDistance:0} m";

        if (distanceCovered >= targetDistance)
            OnTargetAchieved();
    }

    private void OnTargetAchieved()
    {
        Debug.Log("🎯 Target achieved!");
        ResetProgress();
    }

    /// <summary>
    /// Resets the progress and hides the bar.
    /// </summary>
    public void ResetProgress()
    {
        isActive = false;
        progressSlider.value = 0f;
        targetText.text = "Target Achieved!";
        gameObject.SetActive(false);

        // Optionally, you can auto-set the next target here, e.g.:
         SetTarget(targetDistance + 100f);
    }

    /// <summary>
    /// Manually enable or disable the progress bar.
    /// </summary>
    public void SetActive(bool state)
    {
        progressSlider.gameObject.SetActive(state);
        isActive = state;
    }
}
