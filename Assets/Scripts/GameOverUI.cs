using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game Over panel: appears once PlayerDeath finishes its death animation,
/// pauses the game, and offers Restart / Quit buttons wired to GameManager.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The panel/root object containing the Game Over UI. Left inactive until the player dies.")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);

        if (restartButton != null) restartButton.onClick.AddListener(HandleRestart);
        if (quitButton != null) quitButton.onClick.AddListener(HandleQuit);
    }

    private void OnEnable()
    {
        PlayerDeath.OnDeathSequenceFinished += Show;
    }

    private void OnDisable()
    {
        PlayerDeath.OnDeathSequenceFinished -= Show;
    }

    private void Show()
    {
        if (panel != null) panel.SetActive(true);

        // Pause gameplay behind the Game Over screen. Buttons still work
        // since Unity UI input isn't driven by Time.timeScale.
        Time.timeScale = 0f;
    }

    private void HandleRestart()
    {
        GameManager.Instance.RestartLevel();
    }

    private void HandleQuit()
    {
        GameManager.Instance.QuitGame();
    }
}
