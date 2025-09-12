using UnityEngine;

public class EnemyController : PhysicsObject
{
    public float patrolSpeed = 3f;
    private float desiredX;

    void Start()
    {
        desiredX = patrolSpeed;
    }

    protected override void FixedUpdate()
    {
        velocity.x = desiredX;
        base.FixedUpdate();
    }

    public override void CollideWithHorizontal(Collider2D other)
    {
        // Bounce direction when hitting a wall
        desiredX = -desiredX;
    }
}
