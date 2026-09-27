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
    /// Loads the next scene in Build Settings order (the scene one index
    /// after the currently active one). Called by PorteSortie when no
    /// specific scene name is assigned. Requires the scenes to be added to
    /// File > Build Settings > Scenes In Build, in the intended order.
    /// </summary>
    public void LoadNextScene()
    {
        Time.timeScale = 1f;

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("GameManager: No next scene found in Build Settings after the current one.");
        }
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
