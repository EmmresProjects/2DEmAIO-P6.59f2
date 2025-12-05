// EventManager.cs
using UnityEngine;
using System;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Intro images (assign in order)")]
    public GameObject[] introImages;

    [Header("Level banners (index 0..3 for Level1..4). If LevelXBanner assigned, it will be used for level > 4")]
    public GameObject[] levelBanners;
    public GameObject levelXBanner; // optional: single prefab used for dynamic level text

    [Header("Countdown (optional)")]
    public TMP_Text countdownText; // assign a TMP text that will show 3,2,1 (optional)
    public int countdownStart = 3; // default 3..2..1

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    // internal caches
    Dictionary<GameObject, Vector2> bannerDefaultPos = new Dictionary<GameObject, Vector2>();
    Dictionary<GameObject, Coroutine> bannerCoroutines = new Dictionary<GameObject, Coroutine>();

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);

        // Cache default anchored positions for banners (if they have RectTransform)
        if (levelBanners != null)
        {
            foreach (var b in levelBanners)
            {
                if (b == null) continue;
                var rt = b.GetComponent<RectTransform>();
                if (rt != null)
                    bannerDefaultPos[b] = rt.anchoredPosition;
            }
        }

        if (levelXBanner != null)
        {
            var rt = levelXBanner.GetComponent<RectTransform>();
            if (rt != null)
                bannerDefaultPos[levelXBanner] = rt.anchoredPosition;
        }
    }

    /// <summary>
    /// Play intro images in order, then optional countdown, then call onComplete.
    /// Uses realtime waits so it works even if Time.timeScale is 0.
    /// </summary>
    public IEnumerator PlayIntroThen(Action onComplete)
    {
        if (introImages != null && introImages.Length > 0)
        {
            for (int i = 0; i < introImages.Length; i++)
            {
                var img = introImages[i];
                if (img == null) continue;
                img.SetActive(true);
                yield return new WaitForSecondsRealtime(0.6f);
                img.SetActive(false);
            }
        }

        // Optional 3-2-1 countdown using realtime waits
        if (countdownText != null && countdownStart > 0)
        {
            countdownText.gameObject.SetActive(true);
            for (int n = countdownStart; n >= 1; n--)
            {
                countdownText.text = n.ToString();
                yield return new WaitForSecondsRealtime(1.0f);
            }
            countdownText.gameObject.SetActive(false);
        }

        onComplete?.Invoke();
    }

    /// <summary>
    /// Show a level banner. If a banner is already animating, stop it and reset position first.
    /// </summary>
    public IEnumerator ShowLevelBanner(int level, bool endless)
    {
        GameObject banner = null;

        // Choose banner: explicit per-level banners for campaign, levelXBanner for endless or overflow
        if (!endless && levelBanners != null && level <= levelBanners.Length && levelBanners[level - 1] != null)
        {
            banner = levelBanners[level - 1];
        }
        else if (endless && levelXBanner != null)
        {
            banner = levelXBanner;
        }
        else if (levelBanners != null && levelBanners.Length > 0)
        {
            // fallback: use last assigned banner
            banner = levelBanners[Mathf.Min(level - 1, levelBanners.Length - 1)];
        }

        if (banner == null) yield break;

        // If this banner already has an active coroutine, stop it and reset position
        if (bannerCoroutines.TryGetValue(banner, out Coroutine existing))
        {
            if (existing != null) StopCoroutine(existing);
            bannerCoroutines.Remove(banner);
            ResetBannerPosition(banner);
            banner.SetActive(false);
        }

        // Set dynamic text if TMP present
        var tmp = banner.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            if (endless) tmp.text = "Endless Level " + level;
            else tmp.text = "Level " + Mathf.Min(level, 4);
        }

        // Start the animation coroutine and track it
        Coroutine c = StartCoroutine(AnimateBannerRoutine(banner));
        bannerCoroutines[banner] = c;
        yield return c;

        // remove tracking after finished
        if (bannerCoroutines.ContainsKey(banner)) bannerCoroutines.Remove(banner);
    }

    IEnumerator AnimateBannerRoutine(GameObject banner)
    {
        var rt = banner.GetComponent<RectTransform>();
        Vector2 original = Vector2.zero;
        if (rt != null)
        {
            // Use cached default if available, otherwise current anchoredPosition
            if (!bannerDefaultPos.TryGetValue(banner, out original))
                original = rt.anchoredPosition;
        }

        banner.SetActive(true);

        // Slide in from above (200 units)
        if (rt != null)
        {
            Vector2 end = original;
            Vector2 start = new Vector2(end.x, end.y + 200f);
            rt.anchoredPosition = start;
            float t = 0f;
            float dur = 0.35f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                rt.anchoredPosition = Vector2.Lerp(start, end, Mathf.Clamp01(t / dur));
                yield return null;
            }
            rt.anchoredPosition = end;
        }

        // Visible time (use realtime so pause doesn't affect it)
        yield return new WaitForSecondsRealtime(1.0f);

        // Slide out upward and then hide; ensure we animate back to original + offset
        if (rt != null)
        {
            Vector2 start = rt.anchoredPosition;
            Vector2 end = new Vector2(start.x, start.y + 200f);
            float t = 0f;
            float dur = 0.25f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                rt.anchoredPosition = Vector2.Lerp(start, end, Mathf.Clamp01(t / dur));
                yield return null;
            }
            // Reset to original so next show starts from correct baseline
            rt.anchoredPosition = original;
        }

        banner.SetActive(false);
    }

    void ResetBannerPosition(GameObject banner)
    {
        if (banner == null) return;
        var rt = banner.GetComponent<RectTransform>();
        if (rt == null) return;
        if (bannerDefaultPos.TryGetValue(banner, out Vector2 pos))
            rt.anchoredPosition = pos;
    }

    public void ShowWinPanel(string text)
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        if (losePanel != null) losePanel.SetActive(true);
    }

    /// <summary>
    /// Show the pause panel and ensure its CanvasGroup allows interaction.
    /// </summary>
    public void ShowPausePanel()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);

        // Ensure CanvasGroup allows interaction (if present)
        var cg = pausePanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
            cg.alpha = 1f;
        }
    }

    /// <summary>
    /// Hide the pause panel and disable interaction.
    /// </summary>
    public void HidePausePanel()
    {
        if (pausePanel == null) return;

        // Disable interaction first (defensive)
        var cg = pausePanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
            // keep alpha as-is or set to 0 if you want fade-out
        }

        pausePanel.SetActive(false);
    }

    public void HideAllPanels()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        if (levelBanners != null)
        {
            for (int i = 0; i < levelBanners.Length; i++)
                if (levelBanners[i] != null)
                {
                    levelBanners[i].SetActive(false);
                    ResetBannerPosition(levelBanners[i]);
                }
        }

        if (levelXBanner != null)
        {
            levelXBanner.SetActive(false);
            ResetBannerPosition(levelXBanner);
        }

        // hide countdown if present
        if (countdownText != null) countdownText.gameObject.SetActive(false);
    }
}
