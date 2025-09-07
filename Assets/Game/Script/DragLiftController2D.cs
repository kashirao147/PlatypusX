using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DragLiftController2D : MonoBehaviour
{
    [Header("Movement Tuning")]
    [Tooltip("How much vertical speed per screen pixel of drag.")]
    public float speedPerPixel = 0.025f;

    [Tooltip("Max upward speed from drag.")]
    public float maxUpSpeed = 10f;

    [Tooltip("Max downward speed from drag (when dragging downward).")]
    public float maxDownSpeed = -10f;

    [Tooltip("How fast we approach target vertical speed.")]
    public float verticalAccel = 40f;

    [Tooltip("Ignore tiny finger noise (in pixels).")]
    public float deadZonePixels = 2f;

    [Header("Optional")]
    public bool clampXVelocity = false;   // leave X as-is for runners
    public float xVelocity = 0f;          // if you want to lock X speed

    Rigidbody2D rb;

    // drag state
    bool dragging;
    Vector2 prevScreenPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- TOUCH (device) ---
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                dragging = true;
                prevScreenPos = t.position;
            }
            else if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                ApplyDragDelta(t.position);
            }
            else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                dragging = false; // release -> gravity only
            }
        }
        // --- MOUSE (editor) ---
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                prevScreenPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                ApplyDragDelta((Vector2)Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (clampXVelocity)
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);

        // Nothing special to do on release: gravity handles falling.
        // While dragging, we already adjusted velocity in ApplyDragDelta (in Update),
        // but FixedUpdate is where we actually smooth to the target:
        if (_hasPendingTarget && dragging)
        {
            float newVy = Mathf.MoveTowards(rb.velocity.y, _targetVy, verticalAccel * Time.fixedDeltaTime);
            rb.velocity = new Vector2(rb.velocity.x, newVy);
            _hasPendingTarget = false; // consume for this physics step
        }
    }

    // -------- internals --------
    float _targetVy;
    bool _hasPendingTarget;

    void ApplyDragDelta(Vector2 currentScreenPos)
    {
        if (!dragging) return;

        float dyPixels = currentScreenPos.y - prevScreenPos.y;

        // Dead zone
        if (Mathf.Abs(dyPixels) < deadZonePixels)
            dyPixels = 0f;

        // Map pixels to desired vertical speed (positive = up)
        float desiredVy = dyPixels * speedPerPixel;
        desiredVy = Mathf.Clamp(desiredVy, maxDownSpeed, maxUpSpeed);

        _targetVy = desiredVy;
        _hasPendingTarget = true;

        prevScreenPos = currentScreenPos;
    }

    // --------- Optional public API (if other systems call these) ---------

    // Legacy compatibility: call this every frame while you want to nudge up.
    public void MoveUpLegacy(float force = 8f)
    {
        // Small upward nudge; release -> gravity.
        rb.AddForce(Vector2.up * force, ForceMode2D.Force);
    }

    // Example hook if you still need a “snow level jump” special case:
    public System.Func<bool> IsSnowLevel; // assign externally
    public System.Action JumpSnowLevel;   // assign externally
    public void MoveUp()
    {
        if (IsSnowLevel != null && IsSnowLevel.Invoke())
            JumpSnowLevel?.Invoke();
        else
            dragging = true; // if triggered by a hold button, treat as active drag
    }
}
