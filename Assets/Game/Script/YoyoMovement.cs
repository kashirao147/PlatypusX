using UnityEngine;
using DG.Tweening;

public class YoyoMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveUp = 2f;      // how high it goes
    public float moveDown = -2f;   // how low it goes
    public float duration = 1f;    // time to go from up to down

    private Tween yoyoTween;
    public bool isHorizontalMovement=false;

    void Start()
    {
        // Start the yoyo movement
        if (isHorizontalMovement)
        {
            StartHorizontalYoyo();
        }
        else
        {
            StartYoyo();
        }
    }

    void StartYoyo()
    {
        float targetY = transform.position.y + moveUp;
        yoyoTween = transform.DOMoveY(targetY, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
      void StartHorizontalYoyo()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float targetX = transform.position.x + moveUp;

        yoyoTween = transform.DOLocalMoveX(targetX, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .OnStepComplete(() =>
            {
                sr.flipX = !sr.flipX;   // Just flip visually
            });

    }

    void OnDestroy()
    {
        // Kill tween when object is destroyed (avoid memory leaks)
        if (yoyoTween != null && yoyoTween.IsActive())
            yoyoTween.Kill();
    }
}
