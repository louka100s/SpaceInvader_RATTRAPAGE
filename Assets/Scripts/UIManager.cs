using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Menu (MainMenu scene only)")]
    public GameObject menuPanel;
    public Button playButton;
    public Button quitButton;
    public Text highScoreText;

    [Header("HUD (Level1 / Level2 scenes only)")]
    public GameObject hudPanel;
    public Text scoreText;
    public Text livesText;

    [Header("Game Over (GameOver scene only)")]
    public GameObject gameOverPanel;
    public Text gameOverTitleText;
    public Text finalScoreText;
    public Text gameOverHighScoreText;
    public Button restartButton;
    public Button menuButton;

    private const string HighScoreKey = "HighScore";
    private const string MainMenuScene = "MainMenu";
    private const string Level1Scene = "Level1";
    private const string Level2Scene = "Level2";
    private const string GameOverScene = "GameOver";

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == MainMenuScene)
        {
            InitMenu();
        }
        else if (sceneName == GameOverScene)
        {
            InitGameOver();
        }
    }

    /// <summary>
    /// Initializes the main menu UI: high score display and button listeners.
    /// </summary>
    private void InitMenu()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Meilleur score : " + PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayButton);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButton);
        }
    }

    /// <summary>
    /// Initializes the Game Over UI: title (victory vs defeat), final score, high score, and button listeners.
    /// </summary>
    private void InitGameOver()
    {
        if (gameOverTitleText != null && GameManager.Instance != null)
        {
            gameOverTitleText.text = GameManager.Instance.hasWon ? "VICTOIRE !" : "GAME OVER";
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score : " + GameManager.Instance.score;
        }

        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "Meilleur score : " + GameManager.Instance.GetHighScore();
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButton);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButton);
        }
    }

    private void Update()
    {
        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text = "Score : " + GameManager.Instance.score;
        }

        if (livesText != null && GameManager.Instance != null)
        {
            livesText.text = "Vies : " + GameManager.Instance.lives;
        }
    }

    /// <summary>
    /// Starts the game from Level1, resetting score, lives, level, and win flag.
    /// </summary>
    private void OnPlayButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.score = 0;
            GameManager.Instance.lives = 3;
            GameManager.Instance.currentLevel = 1;
            GameManager.Instance.hasWon = false;
        }

        SceneManager.LoadScene(Level1Scene);
    }

    /// <summary>
    /// Quits the application. Logs a message in the editor since Application.Quit does not work there.
    /// </summary>
    private void OnQuitButton()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /// <summary>
    /// Restarts the game from Level1 via GameManager.
    /// </summary>
    private void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
    }

    /// <summary>
    /// Returns to the main menu, resetting score and level.
    /// </summary>
    private void OnMenuButton()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.score = 0;
            GameManager.Instance.currentLevel = 1;
        }

        SceneManager.LoadScene(MainMenuScene);
    }
}
