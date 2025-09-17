using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectibleCounter : MonoBehaviour
{
    public static CollectibleCounter I;

    [Header("Wiring")]
    public TextMeshProUGUI label;

    [Header("State (read-only)")]
    public int count;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        UpdateLabel();
    }

    public void Add(int value = 1)
    {
        count += value;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (label != null) label.text = $"Diamonds: {count}";
    }
}
