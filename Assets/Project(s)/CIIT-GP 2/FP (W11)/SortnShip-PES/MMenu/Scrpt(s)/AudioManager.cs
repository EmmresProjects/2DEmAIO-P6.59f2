// AudioManager.cs
using UnityEngine;

/// <summary>
/// Central audio manager for BGM and SFX.
/// - Attach to a GameObject named AudioManager.
/// - Add two AudioSource components (one for BGM, one for SFX) or let the script create them.
/// - Assign BGM clip and default UI click SFX in inspector.
/// - Use AudioManager.Instance.PlayClick() or UIButtonSFX component for button sounds.
/// </summary>
[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Clips")]
    public AudioClip bgmClip;            // background music clip (assign in inspector)
    public AudioClip uiClickClip;        // default UI click SFX (assign in inspector)

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("AudioSources (optional)")]
    public AudioSource bgmSource;        // optional: assign an AudioSource for BGM
    public AudioSource sfxSource;        // optional: assign an AudioSource for SFX

    // Internal fallback sources if none assigned
    AudioSource _bgmSource;
    AudioSource _sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure sources exist
        _bgmSource = bgmSource ? bgmSource : gameObject.AddComponent<AudioSource>();
        _sfxSource = sfxSource ? sfxSource : gameObject.AddComponent<AudioSource>();

        // Configure sources
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;

        // Apply volumes
        ApplyVolumes();

        // Auto play BGM if assigned
        if (bgmClip != null)
        {
            _bgmSource.clip = bgmClip;
            _bgmSource.volume = bgmVolume * masterVolume;
            _bgmSource.Play();
        }
    }

    void ApplyVolumes()
    {
        AudioListener.volume = masterVolume;
        if (_bgmSource != null) _bgmSource.volume = bgmVolume * masterVolume;
        if (_sfxSource != null) _sfxSource.volume = sfxVolume * masterVolume;
    }

    /// <summary>
    /// Set the master volume (0..1). This is the method OptionsManager expects.
    /// </summary>
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        // Persist
        PlayerPrefs.SetFloat("PREF_MASTER_VOLUME", masterVolume);
    }

    /// <summary>
    /// Set BGM volume (0..1).
    /// </summary>
    public void SetBGMVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        PlayerPrefs.SetFloat("PREF_BGM_VOLUME", bgmVolume);
    }

    /// <summary>
    /// Set SFX volume (0..1).
    /// </summary>
    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolumes();
        PlayerPrefs.SetFloat("PREF_SFX_VOLUME", sfxVolume);
    }

    /// <summary>
    /// Play the configured UI click sound. Returns clip length in seconds (0 if none).
    /// </summary>
    public float PlayClick()
    {
        return PlaySFX(uiClickClip);
    }

    /// <summary>
    /// Play a one-shot SFX clip. Returns clip length in seconds (0 if clip null).
    /// </summary>
    public float PlaySFX(AudioClip clip)
    {
        if (clip == null) return 0f;
        if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        return clip.length;
    }

    /// <summary>
    /// Replace the BGM clip and start playing immediately.
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (_bgmSource == null) _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.clip = clip;
        _bgmSource.loop = loop;
        _bgmSource.volume = bgmVolume * masterVolume;
        if (clip != null) _bgmSource.Play();
    }

    /// <summary>
    /// Stop BGM playback.
    /// </summary>
    public void StopBGM()
    {
        if (_bgmSource != null) _bgmSource.Stop();
    }
}
