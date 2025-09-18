using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxRepeater : MonoBehaviour
{
    public Transform follow;       // usually the Main Camera
    public int pixelsPerUnit = 64; // match your project
    private SpriteRenderer sr;
    private float tileWidth, upp;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!follow) follow = Camera.main.transform;
        upp = 1f / pixelsPerUnit;

        // Use the tiled size if available, else the renderer bounds
        tileWidth = (sr.drawMode == SpriteDrawMode.Tiled) ? sr.size.x : sr.bounds.size.x;

        // Round to an integer number of pixels in world units
        tileWidth = Mathf.Round(tileWidth / upp) * upp;
    }

    void LateUpdate()
    {
        float dx = follow.position.x - transform.position.x;
        float threshold = tileWidth * 0.5f - upp; // tiny buffer

        if (dx > threshold || dx < -threshold)
        {
            float shift = tileWidth * Mathf.Sign(dx);
            float newX = transform.position.x + shift;

            // snap to pixel grid
            newX = Mathf.Round(newX / upp) * upp;

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
