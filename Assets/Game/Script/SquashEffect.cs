using UnityEngine;
using DG.Tweening;

public class SquashLoop : MonoBehaviour
{
    [Header("Squash Settings")]
    public float squashScaleX = 1f; // Stretch horizontally
    public float squashScaleY = 0.6f; // Squash vertically
    public float duration = 0.3f;     // One squash duration
    public Ease easeType = Ease.InOutSine;

    private Vector3 originalScale;
    private Tween squashTween;

    void Start()
    {
        originalScale = transform.localScale;
        PlayLoop();
    }

    public void PlayLoop()
    {
        // Kill any existing loop
        squashTween?.Kill();

        // Create squash/stretch loop
        squashTween = transform
            .DOScale(new Vector3(
                 squashScaleX,
                 squashScaleY,
                originalScale.z), duration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo); // Infinite loop
    }

    void OnDisable()
    {
        squashTween?.Kill();
        transform.localScale = originalScale; // Reset
    }
}
