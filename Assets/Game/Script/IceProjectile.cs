using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public int damage = 1;
    public LayerMask destroyOnHit;     // e.g., Ground, Player, Enemies as needed
    public GameObject hitVfx;          // optional particle prefab

    Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (!_rb) Debug.LogWarning($"[{name}] Rigidbody2D missing.");
    }

    void OnEnable() => Invoke(nameof(Despawn), lifeTime);

    void OnDisable() => CancelInvoke();

    void Despawn() => Destroy(gameObject);

   
}


