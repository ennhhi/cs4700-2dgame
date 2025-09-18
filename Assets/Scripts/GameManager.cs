using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("Win requirement")]
    public int requiredDiamonds = 5;

    [Header("UI (drag from Canvas)")]
    public TextMeshProUGUI winText;      // big centered "You Win!"
    public TextMeshProUGUI needMoreText; // small top text like "Need 2 more"

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        if (winText)      winText.gameObject.SetActive(false);
        if (needMoreText) needMoreText.gameObject.SetActive(false);
    }

    public void TryWin()
    {
        int have = CollectibleCounter.I ? CollectibleCounter.I.count : 0;
        int need = requiredDiamonds - have;
        if (need <= 0) ShowWin();
        else ShowNeedMore(need);
    }

    void ShowWin()
    {
        if (winText)
        {
            if (Sfx.I) Sfx.I.PlayWin();
            winText.text = "You Win!  (Press R to Play Again)";
            winText.gameObject.SetActive(true);
        }
        Time.timeScale = 0f; // pause the game
    }

    void ShowNeedMore(int need)
    {
        if (!needMoreText) return;
        needMoreText.text = $"Need {need} more";
        needMoreText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideNeedMore));
        Invoke(nameof(HideNeedMore), 1.5f); // hides after 1.5s (scaled time)
    }

    void HideNeedMore()
    {
        if (needMoreText) needMoreText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (winText && winText.gameObject.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
