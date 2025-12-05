// MenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Main menu actions: Start, Options, Exit.
/// Plays UI click sound before performing action.
/// Attach to MenuManager GameObject and assign buttons and optionsPanel in inspector.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject optionsPanel;
    public Button startButton;
    public Button optionsButton;
    public Button exitButton;

    // Fallback delay if no clip assigned (seconds)
    const float FALLBACK_DELAY = 0.12f;

    void Start()
    {
        // Wire up buttons
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (optionsButton != null) optionsButton.onClick.AddListener(OnOptionsClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);

        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void OnStartClicked()
    {
        // Play click then load Game scene
        StartCoroutine(PlayThenLoadSceneCoroutine("GameScene"));
    }

    public void OnOptionsClicked()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void OnExitClicked()
    {
        // Play click then quit
        StartCoroutine(PlayThenQuitCoroutine());
    }

    IEnumerator PlayThenLoadSceneCoroutine(string sceneName)
    {
        float wait = 0f;
        if (AudioManager.Instance != null)
        {
            wait = AudioManager.Instance.PlayClick();
        }
        if (wait <= 0f) wait = FALLBACK_DELAY;
        yield return new WaitForSeconds(wait);

        // Load scene (ensure scene added to Build Settings)
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator PlayThenQuitCoroutine()
    {
        float wait = 0f;
        if (AudioManager.Instance != null)
        {
            wait = AudioManager.Instance.PlayClick();
        }
        if (wait <= 0f) wait = FALLBACK_DELAY;
        yield return new WaitForSeconds(wait);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
