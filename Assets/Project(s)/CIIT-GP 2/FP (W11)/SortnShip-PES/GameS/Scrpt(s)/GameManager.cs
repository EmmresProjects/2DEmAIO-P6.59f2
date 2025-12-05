// GameManager.cs
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Core refs")]
    public PathManager pathManager;
    public Spawner spawner;
    public EventManager eventManager;
    public ScannerOrganizer scanner;

    [Header("Store targets")]
    public Transform lazadaTarget;
    public Transform shopeeTarget;

    [Header("UI (TMP)")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text levelText;
    [Tooltip("Optional: shows per-level deliveries")]
    public TMP_Text deliveriesText;

    [Header("Audio clips")]
    public AudioClip correctClip;
    public AudioClip wrongClip;
    public AudioClip winClip;
    public AudioClip loseClip;

    [Header("Optional local audio source (fallback if AudioManager missing)")]
    public AudioSource localSfxSource;

    [Header("Level progression settings (editable per level)")]
    public float packageSpeedLevel1 = 1.6f;
    public float packageSpeedLevel2 = 2.0f;
    public float packageSpeedLevel3 = 2.6f;
    public float packageSpeedLevel4 = 3.2f;

    public float spawnIntervalLevel1 = 1.2f;
    public float spawnIntervalLevel2 = 1.0f;
    public float spawnIntervalLevel3 = 0.85f;
    public float spawnIntervalLevel4 = 0.75f;

    public int deliveriesTargetLevel1 = 20;
    public int deliveriesTargetLevel2 = 30;
    public int deliveriesTargetLevel3 = 40;
    public int deliveriesTargetLevel4 = 50;

    public int endlessTargetIncrement = 5;

    public float levelUpDelayAfterLevel1 = 2.0f;
    public float levelUpDelayAfterLevel2 = 2.0f;
    public float levelUpDelayAfterLevel3 = 2.0f;

    [Header("Endless settings")]
    [Tooltip("Realtime delay between endless levels so packages can clear and banner is visible.")]
    public float endlessLevelDelay = 1.5f;

    [Header("Other")]
    [Tooltip("Default organize speed used when routing packages to stores/trash")]
    public float defaultOrganizeSpeed = 3.0f;

    // runtime state
    int level = 1;
    int deliveries = 0;
    int target = 20;
    int score = 0;
    int lives = 3;
    bool endless = false;

    // progression/flow guards
    bool levelTransitioning = false;
    bool gameplayActive = false; // becomes true only once spawning begins
    bool introRunning = false;   // true while intro/countdown/banners are running

    // pause state (tracks whether the game is currently paused via any path)
    bool isPaused = false;

    List<Package> activePackages = new List<Package>();
    Queue<string> spawnQueue = new Queue<string>();
    readonly string[] storeNames = new string[] { "Lazada", "Shopee" };

    // Prevent double-counting packages (each package processed once)
    HashSet<Package> processedPackages = new HashSet<Package>();

    // Cached scene components to avoid repeated Find calls and to remove deprecation warnings
    PauseButtonController cachedPauseBtnController;
    InputListener cachedInputListener;

    // Compatibility helper: prefer new Unity API when available, fall back otherwise
    T FindInstance<T>() where T : Component
    {
#if UNITY_2023_2_OR_NEWER
        return UnityEngine.Object.FindFirstObjectByType<T>();
#else
        return GameObject.FindObjectOfType<T>();
#endif
    }

    void Awake()
    {
        if (Instance == null) Instance = this; else Destroy(gameObject);

        // Cache commonly used scene components
        cachedPauseBtnController = FindInstance<PauseButtonController>();
        cachedInputListener = FindInstance<InputListener>();
    }

    void OnEnable()
    {
        // Ensure we have a local fallback SFX source if AudioManager isn’t present in this scene
        if (localSfxSource == null)
        {
            localSfxSource = gameObject.GetComponent<AudioSource>();
            if (localSfxSource == null)
            {
                localSfxSource = gameObject.AddComponent<AudioSource>();
                localSfxSource.playOnAwake = false;
                localSfxSource.loop = false;
                localSfxSource.spatialBlend = 0f; // 2D
                localSfxSource.volume = 1f;
            }
        }
    }

    void Start()
    {
        // Ensure PathManager organizeSpeed uses default unless overridden in inspector
        if (pathManager != null) pathManager.organizeSpeed = defaultOrganizeSpeed;

        // Disable Pause button during intro to prevent early pause clicks
        var pauseBtnController = cachedPauseBtnController ?? FindInstance<PauseButtonController>();
        if (pauseBtnController != null) pauseBtnController.SetInteractable(false);

        gameplayActive = false;
        introRunning = true;

        if (eventManager != null)
            StartCoroutine(eventManager.PlayIntroThen(() =>
            {
                // start campaign level 1 and reset score
                StartLevel(1, false, true);
            }));
        else
            // If eventManager missing, start immediately
            StartLevel(1, false, true);
    }

    /// <summary>
    /// Start a level.
    /// </summary>
    /// <param name="lvl">Level number (1..4). For endless, level can be >=4.</param>
    /// <param name="endlessMode">If true, start endless mode.</param>
    /// <param name="resetScore">If true, reset score to 0 (use true for fresh play, false to keep score across levels).</param>
    public void StartLevel(int lvl, bool endlessMode, bool resetScore = true)
    {
        if (eventManager != null) eventManager.HideAllPanels();
        endless = endlessMode;
        level = Mathf.Clamp(lvl, 1, 999); // allow >4 for endless progression but clamp minimum
        deliveries = 0;
        processedPackages.Clear();
        if (resetScore) score = 0;
        lives = 3; // reset lives at the start of each level
        levelTransitioning = false;
        gameplayActive = false;
        ConfigureLevel(level);
        UpdateUI();

        // show level banner then start spawning
        if (eventManager != null) StartCoroutine(eventManager.ShowLevelBanner(level, endless));

        if (spawner != null) spawner.StartSpawning();

        // Gameplay is now active; enable Pause
        gameplayActive = true;
        introRunning = false;

        var pauseBtn = cachedPauseBtnController ?? FindInstance<PauseButtonController>();
        if (pauseBtn != null) pauseBtn.SetInteractable(true);
    }

    void ConfigureLevel(int lvl)
    {
        int effectiveLevel = Mathf.Max(1, lvl);

        if (!endless)
        {
            if (effectiveLevel == 1)
            {
                target = Mathf.Max(1, deliveriesTargetLevel1);
                if (pathManager != null) pathManager.packageSpeed = packageSpeedLevel1;
                if (spawner != null) spawner.spawnInterval = spawnIntervalLevel1;
            }
            else if (effectiveLevel == 2)
            {
                target = Mathf.Max(1, deliveriesTargetLevel2);
                if (pathManager != null) pathManager.packageSpeed = packageSpeedLevel2;
                if (spawner != null) spawner.spawnInterval = spawnIntervalLevel2;
            }
            else if (effectiveLevel == 3)
            {
                target = Mathf.Max(1, deliveriesTargetLevel3);
                if (pathManager != null) pathManager.packageSpeed = packageSpeedLevel3;
                if (spawner != null) spawner.spawnInterval = spawnIntervalLevel3;
            }
            else // effectiveLevel >= 4
            {
                target = Mathf.Max(1, deliveriesTargetLevel4);
                if (pathManager != null) pathManager.packageSpeed = packageSpeedLevel4;
                if (spawner != null) spawner.spawnInterval = spawnIntervalLevel4;
            }
        }
        else
        {
            int extraLevels = Mathf.Max(0, effectiveLevel - 4);
            target = Mathf.Max(1, deliveriesTargetLevel4 + extraLevels * endlessTargetIncrement);

            float baseSpeed = packageSpeedLevel4;
            float speedInc = 0.15f * extraLevels;
            if (pathManager != null) pathManager.packageSpeed = baseSpeed + speedInc;

            if (spawner != null) spawner.spawnInterval = Mathf.Max(0.4f, spawnIntervalLevel4 - 0.05f * extraLevels);
        }

        // keep organize speed in sync
        if (pathManager != null) pathManager.organizeSpeed = defaultOrganizeSpeed;

        // refill spawn queue
        spawnQueue.Clear();
        for (int i = 0; i < 10; i++) spawnQueue.Enqueue(GetRandomAssignedStore());
    }

    public string GetRandomAssignedStore()
    {
        return storeNames[Random.Range(0, storeNames.Length)];
    }

    public string GetNextAssignedStore()
    {
        if (spawnQueue.Count == 0) spawnQueue.Enqueue(GetRandomAssignedStore());
        string s = spawnQueue.Dequeue();
        spawnQueue.Enqueue(GetRandomAssignedStore());
        return s;
    }

    public void RegisterSpawnedPackage(Package p)
    {
        if (p == null) return;
        activePackages.Add(p);
    }

    public void PackageReachedScanner(Package pkg)
    {
        if (scanner != null) scanner.OnPackageAtScanner(pkg);
    }

    public void PackageArrivedAtStore(Package pkg)
    {
        if (pkg == null) return;

        // Prevent double-processing the same package
        if (processedPackages.Contains(pkg)) return;
        processedPackages.Add(pkg);

        // Always apply score/life effects, but only increment deliveries when not in transition
        if (pkg.wasCorrect)
        {
            score += 1;
            if (!levelTransitioning)
            {
                deliveries += 1;
            }

            PlaySFX(correctClip);
        }
        else
        {
            lives -= 1;
            PlaySFX(wrongClip);
        }

        if (activePackages.Contains(pkg)) activePackages.Remove(pkg);
        Destroy(pkg.gameObject);

        UpdateUI();

        // Only check end conditions if not currently transitioning to next level,
        // or always check for lives <= 0 (losing should still trigger)
        if (!levelTransitioning)
        {
            CheckEndConditions();
        }
        else
        {
            // still check for losing condition during transition
            if (lives <= 0)
            {
                DespawnAllPackages();
                if (spawner != null) spawner.StopSpawning();
                if (eventManager != null) eventManager.ShowLosePanel();
                PlaySFX(loseClip);
            }
        }
    }

    public void PackageArrivedAtTrash(Package pkg)
    {
        if (pkg == null) return;

        // Prevent double-processing
        if (processedPackages.Contains(pkg)) return;
        processedPackages.Add(pkg);

        // Missed packages are treated as wrong (lose life)
        lives -= 1;
        PlaySFX(wrongClip);

        if (activePackages.Contains(pkg)) activePackages.Remove(pkg);
        Destroy(pkg.gameObject);

        UpdateUI();

        // Losing still applies during transition
        if (lives <= 0)
        {
            DespawnAllPackages();
            if (spawner != null) spawner.StopSpawning();
            if (eventManager != null) eventManager.ShowLosePanel();
            PlaySFX(loseClip);
            return;
        }
    }

    public void PackageMissedAtScanner(Package pkg)
    {
        if (pkg == null) return;
        // route directly to trash target (do not include scanner waypoint)
        List<Transform> path = new List<Transform>();
        if (pathManager != null && pathManager.trashTarget != null) path.Add(pathManager.trashTarget);
        pkg.wasCorrect = false;
        pkg.SetPath(path, pathManager.organizeSpeed, pathManager.yOffset, "Trash", Package.State.MovingToTrash);
    }

    public void PlayerPressedKey(KeyCode key)
    {
        if (activePackages.Count == 0) return;

        Package nearest = null;
        float bestDist = float.MaxValue;
        if (pathManager == null || pathManager.mainPath == null || pathManager.mainPath.Length == 0) return;
        Transform scannerTransform = pathManager.mainPath[pathManager.mainPath.Length - 1];

        foreach (var p in activePackages)
        {
            if (p == null) continue;
            if (!p.IsAtScanner()) continue;
            float dx = Mathf.Abs(p.transform.position.x - scannerTransform.position.x);
            if (dx < bestDist) { bestDist = dx; nearest = p; }
        }
        if (nearest == null) return;

        KeyCode expected = (nearest.assignedStore == "Lazada") ? KeyBindings.GetLazada() : KeyBindings.GetShopee();

        if (key == expected)
            RouteToStore(nearest, nearest.assignedStore, true);
        else
            RouteToStore(nearest, (nearest.assignedStore == "Lazada") ? "Shopee" : "Lazada", false);
    }

    void RouteToStore(Package pkg, string storeName, bool wasCorrect)
    {
        // Build path that goes directly to the store target (do not re-include scanner)
        List<Transform> path = new List<Transform>();
        if (storeName == "Lazada" && lazadaTarget != null) path.Add(lazadaTarget);
        if (storeName == "Shopee" && shopeeTarget != null) path.Add(shopeeTarget);

        pkg.wasCorrect = wasCorrect;
        pkg.SetPath(path, pathManager.organizeSpeed, pathManager.yOffset, storeName, Package.State.MovingToStore);
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (livesText) livesText.text = "Lives: " + lives;
        if (levelText) levelText.text = endless ? "Level: " + level : "Level: " + Mathf.Min(level, 4) + " / 4";
        if (deliveriesText != null) deliveriesText.text = $"Deliveries: {deliveries}";
    }

    void CheckEndConditions()
    {
        if (lives <= 0)
        {
            // lose
            DespawnAllPackages();
            if (spawner != null) spawner.StopSpawning();
            if (eventManager != null) eventManager.ShowLosePanel();
            PlaySFX(loseClip);
            return;
        }

        // Only trigger level completion when not already transitioning
        if (deliveries >= target && !levelTransitioning)
        {
            // reached target for current level
            levelTransitioning = true;
            if (spawner != null) spawner.StopSpawning();

            if (!endless)
            {
                if (level >= 4)
                {
                    // final campaign win -> show win panel and stop progression
                    DespawnAllPackages();
                    ShowWin();
                }
                else
                {
                    // advance to next campaign level after a short delay so existing packages can clear
                    StartCoroutine(LevelUpCoroutine());
                }
            }
            else
            {
                // Endless: milestone reached -> enter transition, stop spawning, show banner and then continue after a cooldown
                StartCoroutine(EndlessLevelUpCoroutine());
            }
        }
    }

    IEnumerator LevelUpCoroutine()
    {
        // Determine delay for current level (use explicit per-level fields)
        float wait = 2.0f;
        if (level == 1) wait = Mathf.Max(0f, levelUpDelayAfterLevel1);
        else if (level == 2) wait = Mathf.Max(0f, levelUpDelayAfterLevel2);
        else if (level == 3) wait = Mathf.Max(0f, levelUpDelayAfterLevel3);

        // Wait to allow packages already spawned to be processed
        yield return new WaitForSeconds(wait);

        // Advance level
        level += 1;

        // Reset deliveries for the new level and reset lives
        deliveries = 0;
        processedPackages.Clear();
        lives = 3;

        // Reconfigure difficulty for the new level
        ConfigureLevel(level);
        UpdateUI();

        // End transition state and resume spawning
        levelTransitioning = false;

        // Show banner and resume spawning
        if (eventManager != null) StartCoroutine(eventManager.ShowLevelBanner(level, endless));
        if (spawner != null) spawner.StartSpawning();
    }

    IEnumerator EndlessLevelUpCoroutine()
    {
        // Show the next level banner (level + 1) immediately
        if (eventManager != null)
            StartCoroutine(eventManager.ShowLevelBanner(level + 1, true));

        // Wait a short realtime delay so the player sees the banner and existing packages can clear
        yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, endlessLevelDelay));

        // Increase level and adjust difficulty
        level += 1;

        // Reset deliveries and processed set for the new endless level
        deliveries = 0;
        processedPackages.Clear();

        // Reconfigure difficulty for the new level
        ConfigureLevel(level);
        UpdateUI();

        // End transition state and resume spawning
        levelTransitioning = false;

        if (spawner != null) spawner.StartSpawning();
    }

    void DespawnAllPackages()
    {
        foreach (var p in activePackages)
        {
            if (p != null) Destroy(p.gameObject);
        }
        activePackages.Clear();
    }

    void ShowWin()
    {
        if (eventManager != null) eventManager.ShowWinPanel("You Win! Continue Endless?");
        PlaySFX(winClip);
    }

    // Win/Lose handlers
    public void OnWinContinueEndless()
    {
        if (eventManager != null) eventManager.HideAllPanels();
        EnsureUnpausedForIntro();
        gameplayActive = false;
        introRunning = true;

        if (eventManager != null)
            StartCoroutine(eventManager.PlayIntroThen(() => StartLevel(level, true, false)));
        else
            StartLevel(level, true, false);
    }

    public void OnWinRestart()
    {
        EnsureUnpausedForIntro();
        if (eventManager != null) eventManager.HideAllPanels();

        gameplayActive = false;
        introRunning = true;

        if (eventManager != null)
            StartCoroutine(eventManager.PlayIntroThen(() => StartLevel(1, false, true)));
        else
            StartLevel(1, false, true);
    }

    public void OnWinExit()
    {
        Application.Quit();
    }

    public void OnLoseRestart()
    {
        EnsureUnpausedForIntro();
        if (eventManager != null) eventManager.HideAllPanels();

        gameplayActive = false;
        introRunning = true;

        if (eventManager != null)
            StartCoroutine(eventManager.PlayIntroThen(() => StartLevel(1, false, true)));
        else
            StartLevel(1, false, true);
    }

    public void OnLoseExit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Toggle pause state. This implementation only pauses time and UI/audio/input.
    /// It does NOT stop or restart the spawner coroutine; spawning uses scaled time so it will automatically pause.
    /// </summary>
    public void TogglePause()
    {
        // Block pause during intro and before gameplayActive
        if (introRunning || !gameplayActive)
        {
            Debug.Log("TogglePause ignored: intro running or gameplay not active yet.");
            return;
        }

        bool currentlyPaused = Mathf.Approximately(Time.timeScale, 0f);

        var pauseBtn = cachedPauseBtnController ?? FindInstance<PauseButtonController>();

        if (!currentlyPaused)
        {
            // Pause
            Time.timeScale = 0f;
            isPaused = true;

            if (eventManager != null) eventManager.ShowPausePanel();

            // Pause global audio
            AudioListener.pause = true;

            // Disable gameplay input
            var input = cachedInputListener ?? FindInstance<InputListener>();
            if (input != null) input.enabled = false;

            // Disable pause button to prevent re-clicks while paused
            if (pauseBtn != null) pauseBtn.SetInteractable(false);
        }
        else
        {
            // Unpause
            Time.timeScale = 1f;
            isPaused = false;

            if (eventManager != null) eventManager.HidePausePanel();

            // Resume global audio
            AudioListener.pause = false;

            // Re-enable input listener
            var input = cachedInputListener ?? FindInstance<InputListener>();
            if (input != null) input.enabled = true;

            // Re-enable pause button
            if (pauseBtn != null) pauseBtn.SetInteractable(true);
        }
    }

    // Force pause/unpause bypassing intro/gameplay guards (used by UI when user explicitly requests pause)
    public void ForcePause()
    {
        if (isPaused) return;

        Time.timeScale = 0f;
        isPaused = true;

        if (eventManager != null) eventManager.ShowPausePanel();

        AudioListener.pause = true;

        var input = cachedInputListener ?? FindInstance<InputListener>();
        if (input != null) input.enabled = false;

        var pauseBtn = cachedPauseBtnController ?? FindInstance<PauseButtonController>();
        if (pauseBtn != null) pauseBtn.SetInteractable(false);
    }

    public void ForceUnpause()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        isPaused = false;

        if (eventManager != null) eventManager.HidePausePanel();

        AudioListener.pause = false;

        var input = cachedInputListener ?? FindInstance<InputListener>();
        if (input != null) input.enabled = true;

        var pauseBtn = cachedPauseBtnController ?? FindInstance<PauseButtonController>();
        if (pauseBtn != null) pauseBtn.SetInteractable(true);
    }

    // ---- Audio helpers ----
    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // Prefer central AudioManager if present (MainMenu scene should have one with DontDestroyOnLoad)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip);
            return;
        }

        // Fallback local one-shot (ensures gameplay scene still makes sound if AudioManager not loaded)
        if (localSfxSource != null)
        {
            localSfxSource.PlayOneShot(clip);
        }
        else
        {
            // As a last resort, create a temp one-shot audio source
            AudioSource.PlayClipAtPoint(clip, Camera.main ? Camera.main.transform.position : Vector3.zero, 1f);
        }
    }

    /// <summary>
    /// Ensure the game is unpaused and input/audio are enabled so realtime intro/countdown works.
    /// </summary>
    void EnsureUnpausedForIntro()
    {
        // Unpause time
        Time.timeScale = 1f;

        // Resume audio
        AudioListener.pause = false;

        // Re-enable input listener
        var input = cachedInputListener ?? FindInstance<InputListener>();
        if (input != null) input.enabled = true;

        // Re-enable pause button if present
        var pauseBtn = cachedPauseBtnController ?? FindInstance<PauseButtonController>();
        if (pauseBtn != null) pauseBtn.SetInteractable(true);
    }
}
