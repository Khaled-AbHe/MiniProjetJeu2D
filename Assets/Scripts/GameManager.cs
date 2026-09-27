using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Setup Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps manager alive across scenes
        }
    }

    // Note: this used to restart the level immediately on
    // PlayerHealth.OnPlayerDeath. That's now handled by PlayerDeath (death
    // animation) and GameOverUI (Restart/Quit buttons) instead, so the
    // player gets a proper death sequence rather than an instant reload.

    /// <summary>
    /// Reloads the currently active scene. Called by GameOverUI's Restart button.
    /// </summary>
    public void RestartLevel()
    {
        // Undo the pause GameOverUI applies before reloading, otherwise
        // the new scene would load still frozen at timeScale 0.
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Quits the game. Called by GameOverUI's Quit button.
    /// </summary>
    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
