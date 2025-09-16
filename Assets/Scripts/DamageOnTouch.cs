using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageOnTouch : MonoBehaviour
{
    public bool useTrigger = true;

    void Reset() { GetComponent<Collider2D>().isTrigger = useTrigger; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!useTrigger) return;
        if (other.CompareTag("Player")) GameManager.Instance.PlayerHitHazard();
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (useTrigger) return;
        if (c.collider.CompareTag("Player")) GameManager.Instance.PlayerHitHazard();
    }
}
