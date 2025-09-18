using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Smooth, seamless parallax by scrolling the texture UVs.
// Needs sprite Wrap Mode = Repeat and SpriteRenderer Draw Mode = Tiled.
[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxUV : MonoBehaviour
{
    public Transform target;                 // usually Main Camera
    [Range(0f, 1f)] public float strengthX;  // 0=fixed, 1=follow camera
    public int pixelsPerUnit = 64;           // match your project PPU

    SpriteRenderer sr;
    float texWorldWidth;   // how many world units per 1 texture repeat (UV 0..1)
    Vector2 startOffset;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!target) target = Camera.main.transform;

        // World width of one repeat = texture width / PPU
        texWorldWidth = (float)sr.sprite.texture.width / pixelsPerUnit;

        // Force a unique material instance for this renderer
        startOffset = sr.material.mainTextureOffset;
    }

    void LateUpdate()
    {
        // Keep the renderer centered near the camera so the tiled area always covers view
        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);

        // Scroll the texture based on camera movement and parallax strength
        float repeats = (target.position.x * strengthX) / texWorldWidth;

        // frac() keeps offset in 0..1 so it never jumps
        float u = repeats - Mathf.Floor(repeats);

        var m = sr.material;                     // this is the per-instance material
        var off = m.mainTextureOffset;
        off.x = u;
        m.mainTextureOffset = off;
    }
}
