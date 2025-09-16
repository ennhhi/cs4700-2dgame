using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1;
    public AudioClip collectSfx;
    AudioSource audioSrc;
    bool taken;

    void Awake() => audioSrc = GetComponent<AudioSource>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (taken) return;
        if (!other.CompareTag("Player")) return;

        taken = true;
        GameManager.Instance.Collect(value);

        if (collectSfx && audioSrc)
            audioSrc.PlayOneShot(collectSfx);

        // Hide visuals + collider immediately, destroy after sfx
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (var c in GetComponents<Collider2D>()) c.enabled = false;
        Destroy(gameObject, collectSfx ? collectSfx.length : 0f);
    }
}
