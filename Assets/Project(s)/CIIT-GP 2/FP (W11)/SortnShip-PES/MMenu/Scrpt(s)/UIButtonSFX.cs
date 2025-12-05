// UIButtonSFX.cs
using UnityEngine;

/// <summary>
/// Simple per-button SFX helper. Add this component to a GameObject and call UIButtonSFX.Play from Button OnClick.
/// </summary>
public class UIButtonSFX : MonoBehaviour
{
    public AudioClip clip; // assign per-button SFX in inspector
    public void Play()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip);
        }
        else
        {
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
            }
        }
    }
}
