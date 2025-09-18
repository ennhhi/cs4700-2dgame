using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : PhysicsObject
{
    [Header("Tuning")]
    public float moveSpeed = 3f;
    public float jumpSpeed = 6.5f;
    public float coyoteTime = 0.1f; // seconds

    [Header("Death by falling")]
    public float deathY = -12f; // set below your level

    [Header("Spawn")]
    public Vector3 startingPosition;

    private float desiredX;
    private float coyoteTimer;

    private Collider2D col;
    private readonly Collider2D[] contactBuffer = new Collider2D[16];
    private MovingPlatform ridingPlatform;

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider2D>();
        if (rb != null) rb.useFullKinematicContacts = true;
    }

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        // Fall-off death 
        if (transform.position.y < deathY)
        {
            ResetPlayer();
            return;
        }

        // Update coyote timer from last physics result
        coyoteTimer = grounded ? coyoteTime : Mathf.Max(0f, coyoteTimer - Time.deltaTime);

        // Horizontal input
        float h = Input.GetAxisRaw("Horizontal");
        desiredX = (h > 0f) ? moveSpeed : (h < 0f ? -moveSpeed : 0f);

        // Jump
        if (Input.GetButtonDown("Jump") && coyoteTimer > 0f)
        {
            velocity.y = jumpSpeed;
            coyoteTimer = 0f;
            if (Sfx.I) Sfx.I.PlayJump();
        }
    }

    protected override void FixedUpdate()
    {
        velocity.x = desiredX;
        base.FixedUpdate();

        if (col != null)
        {
            int count = col.GetContacts(contactBuffer);
            for (int i = 0; i < count; i++)
            {
                var other = contactBuffer[i];
                if (!other) continue;

                if (IsWater(other)) { ResetPlayer(); break; }
                if (IsGoal(other))  { if (GameManager.I) GameManager.I.TryWin(); }
            }
        }

        // If we're grounded on a moving platform, apply its motion so we ride it
        if (grounded && ridingPlatform != null)
        {
            transform.position += (Vector3)ridingPlatform.DeltaThisFrame;
        }
        else if (!grounded)
        {
            // Clear when we step off
            ridingPlatform = null;
        }
    }

    public override void CollideWithVertical(Collider2D other, Vector2 normal)
    {
        if (IsWater(other)) { ResetPlayer(); return; }
        if (IsGoal(other)) { if (GameManager.I) GameManager.I.TryWin(); return; }

        // If we landed on top of something, see if it's a moving platform
        if (normal.y > 0.5f)
            ridingPlatform = other ? other.GetComponent<MovingPlatform>() : null;
    }

    public override void CollideWithHorizontal(Collider2D other)
    {
        if (IsWater(other)) ResetPlayer();
        else if (IsGoal(other)) { if (GameManager.I) GameManager.I.TryWin(); }
    }

    private bool IsWater(Collider2D other) => other != null && other.CompareTag("water");
    private bool IsGoal(Collider2D other)  => other != null && other.CompareTag("goal");

    private void ResetPlayer()
    {
        if (GameManager.I)
        {
            if (Sfx.I) Sfx.I.PlayDie();
            Time.timeScale = 1f; UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            ridingPlatform = null;
        }
    }
}
