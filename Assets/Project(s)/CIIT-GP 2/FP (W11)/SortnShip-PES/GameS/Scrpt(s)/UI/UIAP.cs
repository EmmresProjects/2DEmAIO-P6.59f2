// UIAudioPlayer.cs
using UnityEngine;
using System.Collections;

public class UIAudioPlayer : MonoBehaviour
{
    /// <summary>
    /// Play a clip on a temporary GameObject that survives scene loads.
    /// Returns the created AudioSource so callers can inspect length if needed.
    /// </summary>
    public static AudioSource PlayClipPersist(AudioClip clip, float volume = 1f, bool ignoreListenerPause = true)
    {
        if (clip == null) return null;

        GameObject go = new GameObject("UIAudioPlayer_Temp");
        DontDestroyOnLoad(go);
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
        src.spatialBlend = 0f;
        src.volume = Mathf.Clamp01(volume);
        src.ignoreListenerPause = ignoreListenerPause;
        src.clip = clip;
        src.Play();
        go.AddComponent<AutoDestroyAfterPlay>();
        return src;
    }

    // Helper component that destroys the GameObject after the clip finishes
    class AutoDestroyAfterPlay : MonoBehaviour
    {
        void Start()
        {
            var src = GetComponent<AudioSource>();
            if (src == null)
            {
                Destroy(gameObject);
                return;
            }

            float len = src.clip != null ? src.clip.length : 0.12f;
            // Add a small safety margin
            StartCoroutine(DestroyAfterRealtime(len + 0.05f));
        }

        System.Collections.IEnumerator DestroyAfterRealtime(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Destroy(gameObject);
        }
    }
}
