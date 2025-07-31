using UnityEngine;
using System.Collections;

public class ExitButtonLogic : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public float fadeDuration = 1f;
    public AudioSource exitSound;

    public void ExitWithFade()
    {
        StartCoroutine(FadeThenExit());
    }

    private IEnumerator FadeThenExit()
    {
        if (exitSound != null)
            exitSound.Play();

        if (fadeGroup != null)
        {
            fadeGroup.blocksRaycasts = true;

            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                fadeGroup.alpha = Mathf.Clamp01(time / fadeDuration);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
