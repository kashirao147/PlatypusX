using UnityEngine;
using DG.Tweening;

namespace PhoenixaStudio
{
    public class ShakeCamera : MonoBehaviour
    {
        [Header("Shake Settings")]
        public float shakeDuration = 0.5f;   // How long the shake lasts
        public float shakeStrength = 0.5f;   // How far it moves left/right
        public int vibrato = 10;              // Number of shakes
        public float randomness = 0f;         // Keep 0 for perfect left/right

        private Vector3 originalPos;
        private Tweener shakeTween;

        private void Awake()
        {
            originalPos = transform.localPosition; // Store starting point
        }

        public void DoShake()
        {
            // Stop any previous shake
            if (shakeTween != null && shakeTween.IsActive())
                shakeTween.Kill();

            // Shake only horizontally
            shakeTween = transform
                .DOShakePosition(
                    shakeDuration,
                    new Vector3(shakeStrength, 0, 0), // X only
                    vibrato,
                    randomness,
                    false, // Snapping
                    false  // FadeOut
                )
                .OnComplete(() =>
                {
                    transform.localPosition = originalPos; // Reset position
                });
        }
    }
}
