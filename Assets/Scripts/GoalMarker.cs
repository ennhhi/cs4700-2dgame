using UnityEngine;

public class GoalMarker : MonoBehaviour
{
    [Tooltip("Require collecting all items before winning.")]
    public bool requireAllCollectibles = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!requireAllCollectibles || GameManager.Instance.CollectedAll())
            GameManager.Instance.WinLevel();
        else
            Debug.Log("Collect all items to finish!");
    }
}
