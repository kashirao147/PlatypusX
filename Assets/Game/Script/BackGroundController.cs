using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core; // Needed for DOGetter and DOSetter

namespace PhoenixaStudio
{
    public class BackGroundController : MonoBehaviour
    {
        [Header("Material Scroll (Smooth)")]
        public Renderer Background;
        public float speedBG = 0.01f;
        public Renderer Midground;
        public float speedMG = 0.02f;
        public Renderer Forceground;
        public float speedFG = 0.03f;
        public Renderer Fish;
        public float speedFish = 0.015f;

        // Internal offset tracking
        private float offsetBG, offsetMG, offsetFG, offsetFish;

        // Tween references
        private Tween tweenBG, tweenMG, tweenFG, tweenFish;

        private void Start()
        {
            // Start looping tweens
            tweenBG = CreateScrollTween(Background, () => offsetBG, v => offsetBG = v, speedBG);
            tweenMG = CreateScrollTween(Midground, () => offsetMG, v => offsetMG = v, speedMG);
            tweenFG = CreateScrollTween(Forceground, () => offsetFG, v => offsetFG = v, speedFG);
            tweenFish = CreateScrollTween(Fish, () => offsetFish, v => offsetFish = v, speedFish);
        }
		
		private void SetTweenSpeed(Tween tween, float speed, float baseSpeed)
		{
			if (tween != null && speed > 0f && baseSpeed > 0f)
			{

				float multiplier = 1f; //GameManager.Instance.playerTemp.speedBoostMultiplier; // e.g., 2x if speed boost doubles speed
				if (GameManager.Instance.playerTemp.isSpeedBoosted)
				{
					 multiplier = GameManager.Instance.playerTemp.speedBoostMultiplier;
				}
				tween.timeScale = multiplier;
			}
		}


        private void Update()
		{
			
			// Pause/resume scrolling based on game state
			bool isScrolling = GameManager.Instance.State == GameManager.GameState.Playing ||
							   GameManager.Instance.State == GameManager.GameState.Menu;

			if (isScrolling)
			{
				ResumeTweens();
				UpdateTweenSpeeds();
			}
			else
			{
				PauseTweens();
			}
		}

      private Tween CreateScrollTween(Renderer renderer, System.Func<float> getter, System.Action<float> setter, float speed)
	{
		if (renderer == null) return null;

		return DOTween.To(
			new DOGetter<float>(() => getter()),
			new DOSetter<float>((float val) =>
			{
				setter(val);

				// Avoid Unity property setter confusion
				Vector2 texOffset = renderer.material.mainTextureOffset;
				texOffset.x = val;
				renderer.material.mainTextureOffset = texOffset;
			}),
			1f, 1f / speed)
		.SetEase(Ease.Linear)
		.SetLoops(-1, LoopType.Restart);
	}


        private void UpdateTweenSpeeds()
        {
            float speedMultiplier = 1f;

            if (GameManager.Instance.Player != null && GameManager.Instance.Player.isSpeedBoosted)
                speedMultiplier = GameManager.Instance.Player.speedBoostMultiplier;

            // Smoothly update speed without snapping
            SetTweenSpeed(tweenBG, speedBG * speedMultiplier, speedBG);
			SetTweenSpeed(tweenMG, speedMG * speedMultiplier, speedMG);
			SetTweenSpeed(tweenFG, speedFG * speedMultiplier, speedFG);
			SetTweenSpeed(tweenFish, speedFish * speedMultiplier, speedFish);

        }

        private void SetTweenSpeed(Tween tween, float speed)
        {
            if (tween != null && speed > 0f)
            {
                // Convert target speed to time scale
                float newDuration = 1f / speed;
                float newTimeScale = tween.Duration(false) / newDuration;

                // Smooth speed change
                DOTween.To(() => tween.timeScale, ts => tween.timeScale = ts, newTimeScale, 0.25f)
                       .SetEase(Ease.Linear)
                       .SetUpdate(true);
            }
        }

        private void PauseTweens()
        {
            tweenBG?.Pause();
            tweenMG?.Pause();
            tweenFG?.Pause();
            tweenFish?.Pause();
        }

        private void ResumeTweens()
        {
            tweenBG?.Play();
            tweenMG?.Play();
            tweenFG?.Play();
            tweenFish?.Play();
        }
    }
}
