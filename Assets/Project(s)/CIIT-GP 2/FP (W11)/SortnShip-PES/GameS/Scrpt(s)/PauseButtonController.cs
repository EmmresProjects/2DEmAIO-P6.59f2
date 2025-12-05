// PauseButtonController.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class PauseButtonController : MonoBehaviour
{
    [Tooltip("Optional: assign the Button component here (will be auto-found if left empty)")]
    public Button pauseButton;

    [Tooltip("Optional: click sound for this button")]
    public AudioClip clickClip;

    [Tooltip("Realtime delay (seconds) to allow the click to start before pausing)")]
    public float clickLeadTime = 0.06f;

    [Range(0f, 1f)]
    public float localVolume = 1f;

    // Local AudioSource used for UI click playback; configured to ignore global pause
    AudioSource localSource;

    void Awake()
    {
        if (pauseButton == null) pauseButton = GetComponent<Button>();

        // Ensure a local AudioSource exists
        localSource = GetComponent<AudioSource>();
        if (localSource == null)
        {
            localSource = gameObject.AddComponent<AudioSource>();
            localSource.playOnAwake = false;
            localSource.loop = false;
            localSource.spatialBlend = 0f; // 2D
        }

        // Make sure UI clicks are audible even when AudioListener.pause = true
        localSource.ignoreListenerPause = true;
    }

    /// <summary>
    /// Called by the Pause button OnClick.
    /// Plays click SFX immediately (on localSource that ignores listener pause),
    /// waits a short realtime lead time so the sound starts, then forces pause.
    /// </summary>
    public void OnPauseClicked()
    {
        // Play click immediately (prefer localSource so it won't be cut by AudioListener.pause)
        if (clickClip != null && localSource != null)
        {
            localSource.PlayOneShot(clickClip, localVolume);
        }
        else if (AudioManager.Instance != null)
        {
            // Fallback to AudioManager if you have one; note AudioManager playback may be paused by AudioListener.pause
            try { AudioManager.Instance.PlayClick(); }
            catch { /* ignore */ }
        }

        // Start coroutine to force pause after a short realtime delay
        StartCoroutine(DoForcePauseAfterRealtimeDelay(clickLeadTime));
    }

    IEnumerator DoForcePauseAfterRealtimeDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, delay));

        // Defensive: disable the button to prevent double clicks
        if (pauseButton != null) pauseButton.interactable = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ForcePause();
        }
        else
        {
            Debug.LogWarning("PauseButtonController: GameManager.Instance is null when trying to force pause.");
        }
    }

    /// <summary>
    /// Called by GameManager to re-enable the pause button when unpausing.
    /// </summary>
    public void SetInteractable(bool value)
    {
        if (pauseButton != null) pauseButton.interactable = value;
    }
}
