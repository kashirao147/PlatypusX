using UnityEngine;
using DG.Tweening;

public class YoyoMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveUp = 2f;      // how high it goes
    public float moveDown = -2f;   // how low it goes
    public float duration = 1f;    // time to go from up to down

    private Tween yoyoTween;

    void Start()
    {
        // Start the yoyo movement
        StartYoyo();
    }

    void StartYoyo()
    {
        float targetY = transform.position.y + moveUp;
        yoyoTween = transform.DOMoveY(targetY, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OnDestroy()
    {
        // Kill tween when object is destroyed (avoid memory leaks)
        if (yoyoTween != null && yoyoTween.IsActive())
            yoyoTween.Kill();
    }
}
