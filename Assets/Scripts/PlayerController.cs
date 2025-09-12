using UnityEngine;

public class PlayerController : PhysicsObject
{
    [Header("Tuning")]
    public float moveSpeed = 3f;
    public float jumpSpeed = 6.5f;

    [Header("Spawn")]
    public Vector3 startingPosition;

    private float desiredX;

    protected override void Awake()
    {
        base.Awake();
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

        // Jump (Space by default) - only if grounded
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpSpeed;
            grounded = false;
        }
    }

    protected override void FixedUpdate()
    {
        // Inject desired X velocity each physics step
        velocity.x = desiredX;
        base.FixedUpdate();
    }

    public override void CollideWithVertical(Collider2D other, Vector2 normal)
    {
        // If we touched the ground (normal pointing up), grounded was set in base.
        if (other.CompareTag("water"))
        {
            // Reset on water
            transform.position = startingPosition;
            velocity = Vector2.zero;
        }

        if (other.CompareTag("goal"))
        {
            Debug.Log("You win!");
        }
    }

    public override void CollideWithHorizontal(Collider2D other)
    {
        if (other.CompareTag("water"))
        {
            transform.position = startingPosition;
            velocity = Vector2.zero;
        }
    }
}
