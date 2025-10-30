using PhoenixaStudio;
using UnityEngine;

public class DestroyAllCollectible : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;

    [Header("Effect Settings")]
    public AudioSource pickupSound;
    public ParticleSystem pickupEffect;

    [Header("Destroy Radius")]
    public float destroyRadius = 10f;
    public LayerMask enemyLayer; // set this to "Enemy" layer in inspector for optimization

    private bool isCollected = false;

    void Start()
    {
        pickupEffect = DistanceProgressBar.Instance.bonusParticle;
    }
    private void Update()
    {
        // Move left only when the game is in Playing state
        if (GameManager.Instance != null && GameManager.Instance.State == GameManager.GameState.Playing)
        {
            transform.Translate(speed * Time.deltaTime * -1, 0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        string n = other.gameObject.name.ToLower();
        if (n.Contains("submarine"))
        {
            isCollected = true;

            // Play effects
            if (pickupSound != null) pickupSound.Play();
            if (pickupEffect != null) pickupEffect.Play();

            // Destroy enemies nearby
            DestroyEnemiesInRadius();

            // Disable visuals & collider
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            // Destroy after short delay
            Destroy(gameObject, 1.5f);
        }
    }

    private void DestroyEnemiesInRadius()
    {
        // Get all colliders within radius on the given layer
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, destroyRadius, enemyLayer);

        int count = 0;
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Hit(10000, true);
                count++;
            }
        }

        Debug.Log($"💥 Destroyed {count} enemies within {destroyRadius} units!");
    }

    // To visualize the radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, destroyRadius);
    }
}
