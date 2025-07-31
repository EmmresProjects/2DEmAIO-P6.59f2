using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButtonLogic : MonoBehaviour
{
    public CanvasGroup fadeGroup;      // Assign FadePanel's CanvasGroup
    public float fadeDuration = 1f;    // Time to fade out
    public AudioSource clickSound;     // Assign AudioSource with click sound

    public void LoadGameplayScene()
    {
        clickSound.Play();             // Play SFX immediately
        StartCoroutine(FadeThenLoad());
    }

    private IEnumerator FadeThenLoad()
    {
        fadeGroup.blocksRaycasts = true;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeGroup.alpha = Mathf.Clamp01(time / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("Gameplay");
    }
}
