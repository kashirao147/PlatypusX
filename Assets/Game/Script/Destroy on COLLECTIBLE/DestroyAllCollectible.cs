using UnityEngine;
using System.Collections.Generic;
using PhoenixaStudio;

public class DestroyAllCollectible : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Missile Settings")]
    [Tooltip("Missile prefab (should contain MissileProjectile component).")]
    public GameObject missilePrefab;
    public int missileCount = 12;
    public float spawnRadius = 0.5f; // spawn offset from collectible center
    public float missileSpeed = 8f;

    [Header("Targeting")]
    public float enemySearchRadius = 50f; // how far to look for enemies
    public LayerMask enemyLayer; // set to Enemy layer in inspector

    [Header("Effects")]
    public ParticleSystem pickupEffect;
    public AudioSource pickupSound;

    private bool isCollected = false;

    private void Update()
    {
        // Move only while playing
        if (GameManager.Instance != null && GameManager.Instance.State == GameManager.GameState.Playing)
        {
            transform.Translate(-speed * Time.deltaTime, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
       string n = other.gameObject.name.ToLower();
        if (!n.Contains("submarine")) return;

        isCollected = true;

        // play FX
        if (pickupSound != null) pickupSound.Play();
        if (pickupEffect != null) pickupEffect.Play();

        // Launch missiles
        LaunchMissiles();

        // disable visuals & collider
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // destroy collectible after effects
        Destroy(gameObject, 2f);
    }

    private void LaunchMissiles()
    {
        // Gather available enemies within search radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, enemySearchRadius, enemyLayer);
        List<Enemy> enemies = new List<Enemy>();
        foreach (var h in hits)
        {
            var e = h.GetComponent<Enemy>();
            if (e != null) enemies.Add(e);
        }

        // Determine targets for missiles: try to assign one unique enemy per missile if possible
        List<Enemy> assignedTargets = new List<Enemy>();
        int enemyCount = enemies.Count;

        for (int i = 0; i < enemyCount; i++)
        {
            Transform target = null;

            if (enemyCount > 0)
            {
                // assign enemy in round-robin to spread missiles across enemies
                Enemy chosen = enemies[i % enemyCount];
                assignedTargets.Add(chosen);
                target = chosen.transform;
            }

            // spawn position slightly offset so missiles don't overlap exactly
            float angle = (360f / missileCount) * i;
            Vector3 spawnPos = transform.position + (Vector3)(Quaternion.Euler(0, 0, angle) * Vector2.right) * spawnRadius;

            SpawnMissile(spawnPos, target);
        }
    }

    private void SpawnMissile(Vector3 spawnPos, Transform target)
    {
        if (missilePrefab == null)
        {
            Debug.LogWarning("Missile prefab not assigned!");
            return;
        }

        var go = Instantiate(missilePrefab, spawnPos, Quaternion.identity);
        var missile = go.GetComponent<MissileProjectile>();
        if (missile != null)
        {
            missile.Initialize(target, missileSpeed, enemyLayer);
        }
        else
        {
            // If prefab doesn't have the script, still try to set velocity
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.right * missileSpeed;
            }
        }
    }

    // Editor gizmo to show search radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, enemySearchRadius);
    }
}
