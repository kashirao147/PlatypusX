using System.Collections;
using UnityEngine;
namespace PhoenixaStudio
{
    public class CannonShooter2D : MonoBehaviour
    {
        [Header("Detection")]
        public float detectRadius = 6f;
        public LayerMask playerMask;

        [Header("Shooting")]
        public Transform muzzle;                // empty child at cannon mouth
        public GameObject projectilePrefab;     // assign IceProjectile prefab
        public float fireCooldown = 1.25f;      // seconds between shots
        public float projectileSpeed = 14f;

        [Header("Punch/Recoil")]
        public float punchDistance = 0.12f;     // local recoil distance along -right
        public float punchOutTime = 0.06f;      // back motion time
        public float punchReturnTime = 0.10f;   // return time
        public AnimationCurve punchCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Optional Animator")]
        public Animator animator;               // optional; will Trigger "Punch" if set

        float _cooldownTimer;
        Vector3 _defaultLocalPos;

        void Awake()
        {
            _defaultLocalPos = transform.localPosition;
            if (!muzzle) Debug.LogWarning($"[{name}] Muzzle not assigned.");
        }

        void Update()
        {
            _cooldownTimer -= Time.deltaTime;
            bool playerNear = false;
            if (Mathf.Abs(GameManager.Instance.Player.transform.position.x - transform.position.x) < detectRadius)
            {
                //allow moving
                playerNear = true;
            }

            // Detect player
            // bool playerNear = Physics2D.OverlapCircle(transform.position, detectRadius, playerMask);
            if (playerNear && _cooldownTimer <= 0f)
            {
                Fire();
                _cooldownTimer = fireCooldown;
            }
        }

        void Fire()
        {
            if (!projectilePrefab || !muzzle) return;

            // Spawn projectile
            var go = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            if (go.TryGetComponent<Rigidbody2D>(out var rb))
            {
                // Face right? We use the cannon’s local right as forward.
                rb.velocity = muzzle.right * projectileSpeed * -1;
            }

            // Punch / recoil
            if (animator)
                animator.SetTrigger("Punch");      // make an Animator state with this trigger (optional)

            StopAllCoroutines();
            StartCoroutine(PunchRoutine());
        }

        IEnumerator PunchRoutine()
        {
            // Move a bit backward along -right, then return
            Vector3 start = transform.localPosition;
            Vector3 punchTarget = _defaultLocalPos - transform.right * punchDistance;

            // Out
            float t = 0f;
            while (t < punchOutTime)
            {
                t += Time.deltaTime;
                float a = punchCurve.Evaluate(Mathf.Clamp01(t / punchOutTime));
                transform.localPosition = Vector3.Lerp(_defaultLocalPos, punchTarget, a);
                yield return null;
            }

            // Return
            t = 0f;
            while (t < punchReturnTime)
            {
                t += Time.deltaTime;
                float a = punchCurve.Evaluate(Mathf.Clamp01(t / punchReturnTime));
                transform.localPosition = Vector3.Lerp(punchTarget, _defaultLocalPos, a);
                yield return null;
            }

            transform.localPosition = _defaultLocalPos;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
        
    }
}