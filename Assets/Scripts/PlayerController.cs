using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : PhysicsObject
{
    [Header("Tuning")]
    public float moveSpeed = 3f;
    public float jumpSpeed = 6.5f;
    public float coyoteTime = 0.1f; // seconds

    [Header("Spawn")]
    public Vector3 startingPosition;

    private float desiredX;
    private float coyoteTimer;

    private readonly Collider2D[] contactBuffer = new Collider2D[16];

    protected override void Awake()
    {
        base.Awake();
        if (rb != null) rb.useFullKinematicContacts = true;
    }

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        // Update coyote timer based on last physics result
        coyoteTimer = grounded ? coyoteTime : Mathf.Max(0f, coyoteTimer - Time.deltaTime);

        // Horizontal input
        float h = Input.GetAxisRaw("Horizontal");
        desiredX = (h > 0f) ? moveSpeed : (h < 0f ? -moveSpeed : 0f);

        // Jump: allow while coyote timer is alive
        if (Input.GetButtonDown("Jump") && coyoteTimer > 0f)
        {
            velocity.y = jumpSpeed;
            coyoteTimer = 0f; // consume it
        }
    }

    protected override void FixedUpdate()
    {
        velocity.x = desiredX;
        base.FixedUpdate();

        // Detect hazards even when idle
        if (col != null)
        {
            int count = col.GetContacts(contactBuffer);
            for (int i = 0; i < count; i++)
            {
                var other = contactBuffer[i];
                if (!other) continue;

                if (IsWater(other)) { ResetPlayer(); break; }
            }
        }
    }

    public override void CollideWithVertical(Collider2D other, Vector2 normal)
    {
        if (IsWater(other)) ResetPlayer();
    }

    public override void CollideWithHorizontal(Collider2D other)
    {
        if (IsWater(other)) ResetPlayer();
    }

    private bool IsWater(Collider2D other) => other != null && other.CompareTag("water");

    private void ResetPlayer()
    {
        transform.position = startingPosition;
        velocity = Vector2.zero;
    }
}
