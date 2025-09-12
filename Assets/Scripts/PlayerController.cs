using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : PhysicsObject
{
    [Header("Tuning")]
    public float moveSpeed = 3f;
    public float jumpSpeed = 6.5f;

    [Header("Spawn")]
    public Vector3 startingPosition;

    private float desiredX;

    // For "idle" contact detection with hazards/goals
    private Collider2D col;
    private readonly Collider2D[] overlapHits = new Collider2D[16];

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        // Horizontal input (-1, 0, 1)
        float h = Input.GetAxisRaw("Horizontal");
        if (h > 0f) desiredX = moveSpeed;
        else if (h < 0f) desiredX = -moveSpeed;
        else desiredX = 0f;

        // Jump only if grounded
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpSpeed;
            grounded = false;
        }
    }

    protected override void FixedUpdate()
    {
        // Inject desired X velocity then run base physics (gravity, casts, ground resolve)
        velocity.x = desiredX;
        base.FixedUpdate();

        // ALSO detect hazards/goals even when not moving (overlap at rest)
        if (col != null)
        {
            var filter = new ContactFilter2D();
            // Include triggers too, in case your hazards/goals use trigger colliders
            filter.useTriggers = true;

            int count = col.OverlapCollider(filter, overlapHits);
            for (int i = 0; i < count; i++)
            {
                var other = overlapHits[i];
                if (!other) continue;

                if (IsWater(other))
                {
                    ResetPlayer();
                    break;
                }
                if (IsGoal(other))
                {
                    Debug.Log("You win!");
                }
            }
        }
    }

    // Called by PhysicsObject when vertical motion hits something
    public override void CollideWithVertical(Collider2D other, Vector2 normal)
    {
        if (IsWater(other))
        {
            ResetPlayer();
        }
        else if (IsGoal(other))
        {
            Debug.Log("You win!");
        }
    }

    // Called by PhysicsObject when horizontal motion hits something
    public override void CollideWithHorizontal(Collider2D other)
    {
        if (IsWater(other))
        {
            ResetPlayer();
        }
    }

    private bool IsWater(Collider2D other)
    {
        // TAG-ONLY
        return other != null && other.CompareTag("water");
    }

    private bool IsGoal(Collider2D other)
    {
        // TAG-ONLY
        return other != null && other.CompareTag("goal");
    }

    private void ResetPlayer()
    {
        transform.position = startingPosition;
        velocity = Vector2.zero;
    }
}
