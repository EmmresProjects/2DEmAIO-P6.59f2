// PausePanelUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PausePanelUI : MonoBehaviour
{
    [Tooltip("Name of your main menu scene (set in inspector).")]
    public string mainMenuSceneName = "MainMenu";

    [Tooltip("Optional click sound for pause panel buttons (Continue / Back to Menu).")]
    public AudioClip clickClip;

    [Tooltip("Fallback wait if AudioManager doesn't provide a length (seconds).")]
    public float fallbackClickDelay = 0.12f;

    [Range(0f, 1f)]
    public float localVolume = 1f;

    /// <summary>
    /// Continue button handler: play click then unpause.
    /// </summary>
    public void OnContinueClicked()
    {
        StartCoroutine(PlayClickThenContinue());
    }

    IEnumerator PlayClickThenContinue()
    {
        float wait = 0f;

        // Prefer central AudioManager if available and it returns a duration
        if (AudioManager.Instance != null)
        {
            try
            {
                wait = AudioManager.Instance.PlayClick();
            }
            catch
            {
                wait = 0f;
            }
        }

        // If AudioManager not used or returned 0, play persistent local clip
        if (wait <= 0f)
        {
            if (clickClip != null)
            {
                // Play on a DontDestroyOnLoad audio player so it won't be cut by scene changes
                var src = UIAudioPlayer.PlayClipPersist(clickClip, localVolume, true);
                if (src != null && src.clip != null) wait = src.clip.length;
            }

            if (wait <= 0f) wait = fallbackClickDelay;
        }

        yield return new WaitForSecondsRealtime(wait);

        // Hide pause UI and unpause
        if (EventManager.Instance != null) EventManager.Instance.HidePausePanel();
        if (GameManager.Instance != null) GameManager.Instance.ForceUnpause();

        // Re-enable pause button defensively
        var pauseBtn = FindObjectOfType<PauseButtonController>();
        if (pauseBtn != null) pauseBtn.SetInteractable(true);
    }

    /// <summary>
    /// Back to main menu: play click then load main menu in realtime.
    /// </summary>
    public void OnBackToMainMenuClicked()
    {
        StartCoroutine(PlayClickThenLoadMenu());
    }

    IEnumerator PlayClickThenLoadMenu()
    {
        float wait = 0f;

        if (AudioManager.Instance != null)
        {
            try
            {
                wait = AudioManager.Instance.PlayClick();
            }
            catch
            {
                wait = 0f;
            }
        }

        if (wait <= 0f)
        {
            if (clickClip != null)
            {
                var src = UIAudioPlayer.PlayClipPersist(clickClip, localVolume, true);
                if (src != null && src.clip != null) wait = src.clip.length;
            }

            if (wait <= 0f) wait = fallbackClickDelay;
        }

        // Wait in realtime so the click starts and continues even if we change Time.timeScale
        yield return new WaitForSecondsRealtime(wait);

        // Ensure unpaused before leaving
        if (GameManager.Instance != null) GameManager.Instance.ForceUnpause();

        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Debug.LogWarning("PausePanelUI: mainMenuSceneName is empty. Cannot load main menu.");
    }
}
