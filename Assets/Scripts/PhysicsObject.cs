using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Header("Runtime")]
    public Vector2 velocity;
    public float castPadding = 0.01f;

    protected Rigidbody2D rb;
    protected bool grounded;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        // Reset ground flag at the start of the step
        grounded = false;

        // Constant gravity (units/second^2)
        Vector2 acceleration = -9.81f * Vector2.up;
        velocity += acceleration * Time.fixedDeltaTime;

        // Move X first, then Y
        Vector2 delta = velocity * Time.fixedDeltaTime;
        Movement(new Vector2(delta.x, 0f), true);
        Movement(new Vector2(0f, delta.y), false);
    }

    protected void Movement(Vector2 move, bool moveX)
    {
        if (move.sqrMagnitude < 1e-10f) return;

        var results = new RaycastHit2D[16];
        int cnt = rb.Cast(move.normalized, results, move.magnitude + castPadding);

        for (int i = 0; i < cnt; i++)
        {
            var hit = results[i];

            if (moveX && Mathf.Abs(hit.normal.x) > 0.5f)
            {
                move.x = 0f;
                velocity.x = 0f;
                CollideWithHorizontal(hit.collider);
            }

            if (!moveX)
            {
                // Hitting ground
                if (hit.normal.y > 0.5f)
                {
                    grounded = true;
                    move.y = 0f;
                    if (velocity.y < 0f) velocity.y = 0f;
                    CollideWithVertical(hit.collider, hit.normal);
                }
                // Hitting ceiling
                else if (hit.normal.y < -0.5f)
                {
                    move.y = 0f;
                    if (velocity.y > 0f) velocity.y = 0f;
                    CollideWithVertical(hit.collider, hit.normal);
                }
            }
        }

        transform.position += (Vector3)move;
    }

    // Hooks for subclasses
    public virtual void CollideWithHorizontal(Collider2D other) { }
    public virtual void CollideWithVertical(Collider2D other, Vector2 normal) { }
}
