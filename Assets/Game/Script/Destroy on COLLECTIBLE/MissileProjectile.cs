using PhoenixaStudio;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MissileProjectile : MonoBehaviour
{
    [Header("Missile")]
    public float speed = 8f;
    public float turnSpeed = 360f; // degrees per second for homing
    public float lifeTime = 6f;

    [Header("Explosion")]
    public float explosionRadius = 1.2f;
    public int explosionDamage = 10000; // will call Enemy.Hit(10000, true)
    public LayerMask enemyLayer;

    [Header("FX")]
    public ParticleSystem trailEffect;
    public ParticleSystem explosionEffect;
    public AudioSource explosionSound;

    private Transform target;
    private Rigidbody2D rb;
    private float spawnTime;
    private bool exploded = false;
    
    

    // Call this right after Instantiate
    public void Initialize(Transform targetTransform, float missileSpeed, LayerMask enemyLayerMask)
    {
        target = targetTransform;
        speed = missileSpeed;
        enemyLayer = enemyLayerMask;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        spawnTime = Time.time;
        // optionally set initial velocity forward
        rb.linearVelocity = transform.right * speed;
        explosionEffect = GameManager.Instance.DestroyAllParticle;
    }

    private void FixedUpdate()
    {
        // simple homing: rotate towards target if assigned
        if (target != null)
        {
            Vector2 dir = (Vector2)(target.position - transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float step = turnSpeed * Time.fixedDeltaTime;
            float newZ = Mathf.MoveTowardsAngle(transform.eulerAngles.z, angle, step);
            transform.eulerAngles = new Vector3(0, 0, newZ);
            rb.linearVelocity = transform.right * speed;
        }
        else
        {
            // no target: fly straight to right (uses current rotation)
            rb.linearVelocity = transform.right * speed;
        }

        // lifetime check
        if (Time.time - spawnTime >= lifeTime)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;

        // ignore triggers from collectible/player etc — focus on enemies and world
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // direct hit: call Hit and explode
            enemy.Hit(explosionDamage, true);
            Explode();
            return;
        }

        // optionally explode when hitting environment
        // if (!other.CompareTag("Player") && !other.CompareTag("Collectible")) Explode();
    }

    private void Explode()
    {
        exploded = true;

        // play explosion fx
        if (explosionEffect != null)
        {
            var fx = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, 2f);
        }
        if (explosionSound != null) explosionSound.Play();

        // splash damage: find enemies in radius and call Hit
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (var c in hits)
        {
            var enemy = c.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Hit(explosionDamage, true);
            }
        }

        // destroy missile gameobject (give sound a frame)
        Destroy(gameObject, 0.05f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0.1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
