using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [Header("Value")]
    public int value = 1;

    [Header("Visual Bob (affects the assigned root and all its children)")]
    public Transform visual;           // assign VisualRoot (the parent of both squares)
    public float bobAmplitude = 0.12f; // world units
    public float bobSpeed = 2.0f;      // cycles per second

    Vector3 visualStartWorld;
    Quaternion visualStartLocalRot;

    void Reset()
    {
        var c = GetComponent<Collider2D>();
        if (c) c.isTrigger = true;
    }

    void Start()
    {
        if (!visual && transform.childCount > 0) visual = transform.GetChild(0);
        if (visual)
        {
            visualStartWorld    = visual.position;        // world-space bob => always vertical
            visualStartLocalRot = visual.localRotation;   // preserve whatever rotation you set in Editor
        }
    }

    void Update()
    {
        if (!visual) return;

        // Bob straight up/down in WORLD space (ignores local rotation)
        float y = Mathf.Sin(Time.time * Mathf.PI * 2f * bobSpeed) * bobAmplitude;
        visual.position = visualStartWorld + Vector3.up * y;

        // Keep the rotation you set in the Editor (e.g., 0 on VisualRoot, 45° on child squares)
        visual.localRotation = visualStartLocalRot;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (CollectibleCounter.I != null)
                CollectibleCounter.I.Add(value);

            if (Sfx.I) Sfx.I.PlayCollect();
            Destroy(gameObject);
        }
    }
}
