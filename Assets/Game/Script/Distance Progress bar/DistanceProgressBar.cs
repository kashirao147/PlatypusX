using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PhoenixaStudio; // for animations

public class DistanceProgressBar : MonoBehaviour
{
    public static DistanceProgressBar Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Text targetText;

    [Header("Reward FX")]
    [SerializeField] private AudioSource bonusSound;
    [SerializeField] public ParticleSystem bonusParticle;
    [SerializeField] private Text bonusText; // e.g. “+100 Coins”

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

        gameObject.SetActive(false); // Hidden by default
        if (bonusText != null)
            bonusText.gameObject.SetActive(false);
    }

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

        // Countdown timer if active
      


        if (!isActive || GameManager.Instance == null)
            return;

        float speed = GameManager.Instance.Speed;
        distanceCovered += speed * Time.deltaTime;
        distanceCovered = Mathf.Clamp(distanceCovered, 0f, targetDistance);

        progressSlider.value = distanceCovered;
        targetText.text = $"Progress: {distanceCovered:0} / {targetDistance:0} m";

        if (distanceCovered >= targetDistance)
            OnTargetAchieved();
    }

    private void OnTargetAchieved()
    {
        if (!isActive) return;

        isActive = false;
        Debug.Log("🎯 Target achieved!");
        progressSlider.value = progressSlider.maxValue;

        // Play bonus effects
        PlayBonusEffects();

        // Reset progress bar after short delay
        DOVirtual.DelayedCall(2.5f, () =>
        {
            ResetProgress();
            SetTarget(targetDistance + 100);
        });
    }

    private void PlayBonusEffects()
    {
        // 🔊 Play sound
        if (bonusSound != null)
            bonusSound.Play();
        GameManager.Instance.GreatJobParticle.Play();
        // 💥 Play particles
        if (bonusParticle != null)
            bonusParticle.Play();
        FindFirstObjectByType<ShakeCamera>().DoShake();
        // 💰 Show “+100 Coins” animation
        if (bonusText != null)
        {
            bonusText.text = "+" + targetDistance + " Coins";
            bonusText.gameObject.SetActive(true);
            // bonusText.color = new Color(1, 1, 0, 0); // yellow and transparent

            // Fade in + move up animation
            //bonusText.DOFade(1, 0.3f);
            bonusText.rectTransform.DOScale(1.3f, 0.3f).SetEase(Ease.OutBack);
            bonusText.rectTransform.DOLocalMoveY(bonusText.rectTransform.position.y + 100f, 2f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {

                    bonusText.gameObject.SetActive(false);
                    //bonusText.DOFade(0, 1f).OnComplete(() => );
                    bonusText.rectTransform.localScale = Vector3.one * 4;
                });
        }

        // 🪙 Increment coins using DOTween
        int startCoins = GlobalValue.Coin;
        int targetCoins = startCoins + 100;
        DOVirtual.Int(startCoins, targetCoins, 2f, (value) =>
        {
            GlobalValue.Coin = value;
            // Optionally update your coin UI text here if you have one:
            // coinText.text = GlobalValue.Coin.ToString();
        });
    }

    public void ResetProgress()
    {
        progressSlider.value = 0f;
        targetText.text = "Target Achieved!";
        gameObject.SetActive(false);
    }

    public void SetActive(bool state)
    {
        progressSlider.gameObject.SetActive(state);
        isActive = state;
    }




 
}
