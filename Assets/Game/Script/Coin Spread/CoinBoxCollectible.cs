using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using PhoenixaStudio;

public class CoinBoxCollectible : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> coins;        // assign coin objects here (child coins)
    public AudioSource pickupSound;       // optional sound when collected
    public ParticleSystem pickupEffect;   // optional particle effect

    [Header("Animation Settings")]
    public float totalDuration = 2f;      // total duration for all coins to activate
    public float scalePop = 1.4f;         // how big the coin pops when activated

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool collected = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        pickupEffect = GameManager.Instance.CoinBoxCollectParticle;
        // ensure all coins start disabled
        foreach (var coin in coins)
            if (coin != null) coin.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        string n = other.gameObject.name.ToLower();
        if (!n.Contains("submarine")) return;

        collected = true;

        // disable box visuals and collider
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;

        // play sound and effect
        if (pickupSound != null) pickupSound.Play();
        if (pickupEffect != null)
        {
            pickupEffect.transform.position = other.transform.position;
          pickupEffect.Play();  
        } 

        // activate coins with animation
        StartCoinSequence();
    }

    private void StartCoinSequence()
    {
        if (coins == null || coins.Count == 0)
            return;

        // interval per coin
        float interval = totalDuration / coins.Count;

        // ensure DOTween uses unscaled time
        DOTween.defaultTimeScaleIndependent = true;

        for (int i = 0; i < coins.Count; i++)
        {
            int index = i;
            DOVirtual.DelayedCall(interval * i, () =>
            {
                if (coins[index] == null) return;

                // enable coin
                coins[index].SetActive(true);

                // reset coin transform
                coins[index].transform.localScale = Vector3.zero;

                // animate pop-up effect
                coins[index].transform
                    .DOScale(scalePop, 0.3f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true) // unscaled time
                    .OnComplete(() =>
                    {
                        // return to normal scale
                        coins[index].transform
                            .DOScale(1f, 0.2f)
                            .SetEase(Ease.InOutSine)
                            .SetUpdate(true);
                    });
            }).SetUpdate(true); // unscaled time
        }

        // destroy after all coins are shown
        // DOVirtual.DelayedCall(totalDuration + 1f, () =>
        // {
        //     Destroy(gameObject);
        // }).SetUpdate(true);
    }
}
