using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovingPlatform : MonoBehaviour
{
    public Transform[] points;       // assign P0, P1 (and more if you want)
    public float speed = 2f;         // units/second
    public bool pingPong = true;     // bounce back at ends (or loop)

    public Vector2 DeltaThisFrame { get; private set; }

    Rigidbody2D rb;
    int targetIndex = 0, dir = 1;
    Vector2 prevPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        prevPos = rb.position;
    }

    void FixedUpdate()
    {
        if (points == null || points.Length == 0) return;

        Vector2 target = points[targetIndex].position;
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);

        DeltaThisFrame = newPos - prevPos;
        prevPos = newPos;

        if ((newPos - target).sqrMagnitude < 0.0001f)
        {
            if (pingPong)
            {
                if (targetIndex == points.Length - 1) dir = -1;
                else if (targetIndex == 0) dir = 1;
                targetIndex += dir;
            }
            else
            {
                targetIndex = (targetIndex + 1) % points.Length;
            }
        }
    }
}