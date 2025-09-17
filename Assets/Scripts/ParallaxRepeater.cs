using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxRepeater : MonoBehaviour
{
    public Transform follow; // usually Camera.main.transform
    private SpriteRenderer sr;
    private float tileWidth;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!follow) follow = Camera.main.transform;

        // If Draw Mode is Tiled, sr.size.x is the world width we set in the Inspector.
        // Otherwise fall back to bounds.
        tileWidth = (sr.drawMode == SpriteDrawMode.Tiled) ? sr.size.x : sr.bounds.size.x;
    }

    void LateUpdate()
    {
        float dx = follow.position.x - transform.position.x;
        if (Mathf.Abs(dx) >= tileWidth * 0.5f)
        {
            float shift = tileWidth * Mathf.Sign(dx);
            transform.position += new Vector3(shift, 0f, 0f);
        }
    }
}
