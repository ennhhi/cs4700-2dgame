using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("Usually the Main Camera")]
    public Transform target;
    [Range(0f, 1f)] public float strengthX = 0.5f;
    [Range(0f, 1f)] public float strengthY = 0.0f;
    public bool lockY = true;

    Vector3 startPos, targetStart;

    void Start()
    {
        if (!target) target = Camera.main.transform;
        startPos = transform.position;
        targetStart = target.position;
    }

    void LateUpdate()
    {
        Vector3 delta = target.position - targetStart;
        float dy = lockY ? 0f : delta.y * strengthY;
        transform.position = new Vector3(
            startPos.x + delta.x * strengthX,
            startPos.y + dy,
            startPos.z
        );
    }
}
