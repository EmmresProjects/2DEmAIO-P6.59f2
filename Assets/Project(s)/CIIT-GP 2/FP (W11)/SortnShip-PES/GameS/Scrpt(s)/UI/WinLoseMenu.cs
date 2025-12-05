// LoseWinMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseWinMenu : MonoBehaviour
{
    [Tooltip("Exact name of your main menu scene (must be in Build Settings).")]
    public string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// Continue endless (Win panel).
    /// </summary>
    public void OnContinueEndless()
    {
        if (GameManager.Instance == null) return;
        EventManager.Instance?.HideAllPanels();
        GameManager.Instance.ForceUnpause();
        GameManager.Instance.OnWinContinueEndless();
    }

    /// <summary>
    /// Restart campaign from Level 1 (used by Win / Lose restart buttons).
    /// </summary>
    public void OnRestart()
    {
        if (GameManager.Instance != null) GameManager.Instance.ForceUnpause();
        EventManager.Instance?.HideAllPanels();
        GameManager.Instance?.OnLoseRestart();
    }

    /// <summary>
    /// Back to Main Menu immediate.
    /// </summary>
    public void OnBackToMainMenu()
    {
        if (GameManager.Instance != null) GameManager.Instance.ForceUnpause();
        EventManager.Instance?.HideAllPanels();

        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Debug.LogWarning("LoseWinMenu: mainMenuSceneName is empty. Add the scene name in the inspector.");
    }

    /// <summary>
    /// Exit the application.
    /// </summary>
    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
