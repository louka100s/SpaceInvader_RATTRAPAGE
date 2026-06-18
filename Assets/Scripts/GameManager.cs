using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    /// <summary>
    /// Returns the singleton instance. Auto-creates a persistent one if it doesn't exist,
    /// which allows testing from any scene without going through MainMenu.
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }

            return _instance;
        }
    }

    public int score = 0;
    public int lives = 3;
    public int currentLevel = 1;
    public bool hasWon = false;

    private const string HighScoreKey = "HighScore";
    private const string Level1Scene = "Level1";
    private const string Level2Scene = "Level2";
    private const string Level3Scene = "Level3";
    private const string Level4Scene = "Level4";
    private const string Level5Scene = "Level5";
    private const string GameOverScene = "GameOver";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds points to the current score and updates the high score if needed.
    /// </summary>
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("AddScore called. Score = " + score);
        SaveHighScore();
    }

    /// <summary>
    /// Returns the saved high score from PlayerPrefs.
    /// </summary>
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    /// <summary>
    /// Saves the current score as high score if it exceeds the saved one.
    /// </summary>
    private void SaveHighScore()
    {
        if (score > GetHighScore())
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Decrements lives by one. Triggers Game Over when lives reach zero.
    /// </summary>
    public void LoseLife()
    {
        lives--;
        Debug.Log("Game Over triggered by: LoseLife. Lives remaining = " + lives);
        if (lives <= 0)
        {
            lives = 0;
            GameOver();
        }
    }

    /// <summary>
    /// Saves the high score and loads the GameOver scene (defeat path).
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Game Over triggered by: GameOver() called directly.");
        hasWon = false;
        SaveHighScore();
        SceneManager.LoadScene(GameOverScene);
    }

    /// <summary>
    /// Increments currentLevel then loads the next level scene,
    /// or triggers the victory ending once all levels are cleared.
    /// </summary>
    public void LevelComplete()
    {
        Debug.Log("LevelComplete called. currentLevel = " + currentLevel);
        currentLevel++;
        if (currentLevel == 2)
        {
            SceneManager.LoadScene(Level2Scene);
        }
        else if (currentLevel == 3)
        {
            SceneManager.LoadScene(Level3Scene);
        }
        else if (currentLevel == 4)
        {
            SceneManager.LoadScene(Level4Scene);
        }
        else if (currentLevel == 5)
        {
            SceneManager.LoadScene(Level5Scene);
        }
        else
        {
            hasWon = true;
            SaveHighScore();
            SceneManager.LoadScene(GameOverScene);
        }
    }

    /// <summary>
    /// Resets score, lives, level, and win flag then restarts from Level1.
    /// </summary>
    public void RestartGame()
    {
        score = 0;
        lives = 3;
        currentLevel = 1;
        hasWon = false;
        SceneManager.LoadScene(Level1Scene);
    }
}
