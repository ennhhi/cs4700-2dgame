using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Header("Runtime")]
    public Vector2 velocity;

    [Tooltip("Extra distance when casting so we stop slightly before hits.")]
    [Range(0f, 0.1f)]
    public float castPadding = 0.03f;

    [Tooltip("How far we can 'snap' down to stick to the ground.")]
    [Range(0f, 0.2f)]
    public float groundSnapDistance = 0.08f;

    [Tooltip("Minimum separation used by depenetration (no extra gap).")]
    [Range(0f, 0.01f)]
    public float separationEpsilon = 0.001f;

    protected Rigidbody2D rb;
    protected Collider2D col;
    protected bool grounded;

    // temp buffers
    private readonly RaycastHit2D[] castHits = new RaycastHit2D[16];
    private readonly Collider2D[]   contactBuf = new Collider2D[16];

    protected virtual void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    protected virtual void FixedUpdate()
    {
        grounded = false;

        // Gravity
        velocity += Vector2.down * 9.81f * Time.fixedDeltaTime;

        Vector2 delta = velocity * Time.fixedDeltaTime;

        // Prefer Y first when falling (clean land), else X first
        if (velocity.y <= 0f)
        {
            Movement(new Vector2(0f, delta.y), moveX: false);
            Movement(new Vector2(delta.x, 0f), moveX: true);
        }
        else
        {
            Movement(new Vector2(delta.x, 0f), moveX: true);
            Movement(new Vector2(0f, delta.y), moveX: false);
        }

        // Resolve any overlaps (corner wedge cure)
        ResolvePenetration();

        // Snap to ground if very close (kills micro-hops)
        GroundSnap();

        // If grounded, ensure we don't keep a tiny downward drift
        if (grounded && velocity.y < 0f) velocity.y = 0f;
    }

    protected void Movement(Vector2 move, bool moveX)
    {
        if (move.sqrMagnitude < 1e-10f) return;

        float dist  = move.magnitude;
        Vector2 dir = move.normalized;

        int cnt = rb.Cast(dir, castHits, dist + castPadding);

        bool blockedHoriz = false;
        float cornerPushX = 0f;

        for (int i = 0; i < cnt; i++)
        {
            var hit = castHits[i];

            // Clamp to just before first hit
            float allowed = hit.distance - castPadding;
            if (allowed < dist) dist = Mathf.Max(0f, allowed);

            if (moveX && Mathf.Abs(hit.normal.x) > 0.5f)
            {
                blockedHoriz = true;
            }
            else if (!moveX)
            {
                if (hit.normal.y > 0.5f)
                {
                    grounded = true;
                    if (velocity.y < 0f) velocity.y = 0f;

                    // If landing on a corner (has some X), prepare tiny sideways nudge
                    if (Mathf.Abs(hit.normal.x) > 0.01f)
                        cornerPushX = hit.normal.x * 0.03f;

                    CollideWithVertical(hit.collider, hit.normal);
                }
                else if (hit.normal.y < -0.5f)
                {
                    if (velocity.y > 0f) velocity.y = 0f;
                    CollideWithVertical(hit.collider, hit.normal);
                }
            }
        }

        // Try stepping up small lips when we hit a wall while not rising
        if (moveX && blockedHoriz && velocity.y <= 0.05f)
        {
            if (TryStepUp(dir, dist))
                return;

            velocity.x = 0f;
        }

        // Perform allowed move
        Vector2 clampedMove = dir * dist;
        transform.position += (Vector3)clampedMove;

        // After a vertical land, tiny sideways nudge away from corner if clear
        if (!moveX && grounded && Mathf.Abs(cornerPushX) > 0f)
        {
            int sideHits = rb.Cast(new Vector2(Mathf.Sign(cornerPushX), 0f),
                                   castHits, Mathf.Abs(cornerPushX) + castPadding);
            if (sideHits == 0)
                transform.position += new Vector3(cornerPushX, 0f, 0f);
        }

        if (moveX && blockedHoriz) CollideWithHorizontal(null);
    }

    private void ResolvePenetration()
    {
        if (!col) return;

        var filter = new ContactFilter2D { useTriggers = false };

        // Up to two passes in case resolving one overlap reveals another
        for (int pass = 0; pass < 2; pass++)
        {
            int count = col.OverlapCollider(filter, contactBuf);
            bool moved = false;

            for (int i = 0; i < count; i++)
            {
                var other = contactBuf[i];
                if (!other) continue;

                var cd = col.Distance(other); // normal points from 'col' -> 'other'
                if (!cd.isOverlapped) continue;

                Vector2 n = cd.normal;

                // If this is mostly a "standing on ground" contact (normal pointing up)
                // and we're not moving upward, skip vertical depenetration to avoid hop.
                if (n.y > 0.4f && velocity.y <= 0f)
                    continue;

                float depth = -cd.distance + separationEpsilon; // distance is negative when overlapped
                if (depth <= 0f) continue;

                Vector2 push = -n * depth;       // move away from the other
                if (push.y > 0f)                 // never pop up off the floor by more than a hair
                    push.y = Mathf.Min(push.y, 0.002f);

                transform.position += (Vector3)push;
                moved = true;
            }

            if (!moved) break;
        }
    }


    private void GroundSnap()
    {
        // Only snap when descending / idle, not when rising
        if (velocity.y > 0.05f || !col) return;

        int cnt = rb.Cast(Vector2.down, castHits, groundSnapDistance);
        for (int i = 0; i < cnt; i++)
        {
            var hit = castHits[i];
            if (hit.normal.y > 0.5f)
            {
                float gap = hit.distance - separationEpsilon;
                if (gap > 0f)
                {
                    transform.position += Vector3.down * gap;
                }
                grounded = true;
                velocity.y = Mathf.Min(0f, velocity.y);
                break;
            }
        }
    }

    // Try to climb small steps by probing up then moving sideways
    private bool TryStepUp(Vector2 horizDir, float horizDist)
    {
        const float maxStep = 0.45f; // ~half a tile if 1 unit tiles
        const float inc     = 0.05f;

        Vector3 start = transform.position;

        for (float up = inc; up <= maxStep; up += inc)
        {
            // Headroom check
            int upCnt = rb.Cast(Vector2.up, castHits, up + castPadding);
            if (upCnt > 0) break;

            transform.position = start + new Vector3(0f, up, 0f);

            int sideCnt = rb.Cast(horizDir, castHits, horizDist + castPadding);
            bool blocked = false;
            for (int i = 0; i < sideCnt; i++)
                if (Mathf.Abs(castHits[i].normal.x) > 0.5f) { blocked = true; break; }

            if (!blocked)
            {
                transform.position += (Vector3)(horizDir * horizDist);
                grounded = true;
                return true;
            }
        }

        transform.position = start;
        return false;
    }

    public virtual void CollideWithHorizontal(Collider2D other) { }
    public virtual void CollideWithVertical(Collider2D other, Vector2 normal) { }
}
