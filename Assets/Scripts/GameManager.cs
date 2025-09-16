using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Starting values")]
    public int startingLives = 3;

    [Header("Runtime")]
    public int lives;
    public int score;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;   // assign ScoreText
    public TextMeshProUGUI livesText;   // assign LivesText
    public GameObject winPanel;         // assign WinPanel (inactive at start)
    public GameObject losePanel;        // assign LosePanel (inactive at start)

    int collectiblesInLevel;
    int collectedThisLevel;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        lives = startingLives;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Count collectibles in the newly loaded level
        collectiblesInLevel = GameObject.FindGameObjectsWithTag("Collectible").Length;
        collectedThisLevel = 0;

        // Try to auto-find UI by object name if not assigned
        if (!scoreText) scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        if (!livesText) livesText = GameObject.Find("LivesText")?.GetComponent<TextMeshProUGUI>();
        if (!winPanel)  winPanel  = GameObject.Find("WinPanel");
        if (!losePanel) losePanel = GameObject.Find("LosePanel");

        if (winPanel)  winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        Time.timeScale = 1f;

        UpdateHud();
    }

    void UpdateHud()
    {
        if (scoreText) scoreText.text = $"Score: {score}  ({collectedThisLevel}/{collectiblesInLevel})";
        if (livesText) livesText.text = $"Lives: {lives}";
    }

    // ==== Public API called by other scripts ====

    public void Collect(int value)
    {
        score += value;
        collectedThisLevel++;
        UpdateHud();
    }

    public bool CollectedAll() => collectedThisLevel >= collectiblesInLevel;

    public void PlayerHitHazard()
    {
        lives--;
        UpdateHud();

        if (lives <= 0)
        {
            if (losePanel) { losePanel.SetActive(true); Time.timeScale = 0f; }
            else LoadSceneByName("Lose");
        }
        else
        {
            // Reload current level to respawn
            var s = SceneManager.GetActiveScene();
            SceneManager.LoadScene(s.buildIndex);
        }
    }

    public void WinLevel()
    {
        if (winPanel) { winPanel.SetActive(true); Time.timeScale = 0f; }
        else LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int i = SceneManager.GetActiveScene().buildIndex;
        if (i + 1 < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(i + 1);
        else LoadSceneByName("Win"); // optional final scene
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        lives = startingLives;
        score = 0;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
    }
}
