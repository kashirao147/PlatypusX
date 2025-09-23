using UnityEngine;
using DG.Tweening;

namespace PhoenixaStudio
{
    public class ScaleOnEnableDOTween : MonoBehaviour
    {
        [Header("Target Settings")]
        [Tooltip("Final scale value (uniform across all axes)")]
        public float targetScale = 1f;

        [Tooltip("Duration of the scaling animation in seconds")]
        public float duration = 1f;

        private void OnEnable()
        {
            SoundManager.PlaySfx(GameManager.Instance.SoundManager.soundClick);
            // Reset scale instantly
            transform.localScale = Vector3.zero;

            // Kill any existing tweens on this transform to avoid overlap
            transform.DOKill();

            // Animate scale up
            transform.DOScale(Vector3.one * targetScale, duration)
                     .SetEase(Ease.OutBack); // you can change Ease type
        }
    }
}