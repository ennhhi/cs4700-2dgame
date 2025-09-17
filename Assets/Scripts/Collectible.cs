using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [Header("Value")]
    public int value = 1;

    [Header("Visual Bob (affects child only)")]
    public Transform visual;           // assign the child with the SpriteRenderer
    public float bobAmplitude = 0.12f; // world units
    public float bobSpeed = 2.0f;      // cycles per second

    Vector3 visualStartLocal;

    void Reset()
    {
        // Make sure the collider is a trigger
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;
    }

    void Start()
    {
        if (!visual && transform.childCount > 0) visual = transform.GetChild(0);
        if (visual) visualStartLocal = visual.localPosition;
    }

    void Update()
    {
        if (visual)
        {
            float y = Mathf.Sin(Time.time * Mathf.PI * 2f * bobSpeed) * bobAmplitude;
            visual.localPosition = visualStartLocal + new Vector3(0f, y, 0f);
            // Keep the diamond tilted; remove this line if you don’t want it locked at 45°
            visual.localRotation = Quaternion.Euler(0f, 0f, 45f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Count it if the Player touches
        if (other.GetComponent<PlayerController>() != null)
        {
            if (CollectibleCounter.I != null)
                CollectibleCounter.I.Add(value);

            Destroy(gameObject);
        }
    }
}
