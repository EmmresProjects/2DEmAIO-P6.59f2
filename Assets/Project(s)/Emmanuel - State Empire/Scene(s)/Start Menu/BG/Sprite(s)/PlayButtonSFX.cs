using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class PlayButtonSFX : MonoBehaviour
{
    private AudioSource sfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (sfx != null && sfx.clip != null)
            sfx.Play();
    }
}
