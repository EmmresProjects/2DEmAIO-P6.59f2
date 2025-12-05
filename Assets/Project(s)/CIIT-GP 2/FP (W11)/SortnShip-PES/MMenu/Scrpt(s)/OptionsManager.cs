// OptionsManager.cs
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Options manager for MainMenu (simplified for Simple build).
/// - Only Lazada and Shopee remapping are exposed.
/// - Conflict detection prevents assigning the same key to both stores unless user replaces.
/// - Attach to OptionsManager GameObject and assign UI references in inspector.
/// </summary>
public class OptionsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject optionsPanel;
    public Slider volumeSlider;
    public Button backButton;
    public Button resetDefaultsButton;

    [Header("Key Remap Rows (TextMeshPro)")]
    public TextMeshProUGUI lazadaLabel;
    public Button lazadaRemapButton;
    public TextMeshProUGUI shopeeLabel;
    public Button shopeeRemapButton;

    [Header("Conflict / Alert Panels")]
    public GameObject conflictPanel;
    public TextMeshProUGUI conflictText;
    public Button conflictReplaceButton;
    public Button conflictCancelButton;

    public GameObject alertPanel;
    public TextMeshProUGUI alertText;
    public Button alertOkButton;

    // Internal state
    private bool waitingForKey = false;
    private string currentTargetPref = null; // "Lazada" or "Shopee"
    private Action<KeyCode> onKeyCaptured;

    // Pending conflict state
    private KeyCode pendingCapturedKey = KeyCode.None;
    private string pendingOwner = null;
    private string pendingTarget = null;

    // PlayerPrefs keys (local constants)
    const string PREF_VOLUME = "PREF_MASTER_VOLUME";
    const string PREF_KEY_LAZADA = "PREF_KEY_LAZADA";
    const string PREF_KEY_SHOPEE = "PREF_KEY_SHOPEE";

    void Start()
    {
        // Load saved volume
        float vol = PlayerPrefs.GetFloat(PREF_VOLUME, 1f);
        if (volumeSlider != null) volumeSlider.value = vol;
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(vol);

        // Wire UI
        if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (backButton != null) backButton.onClick.AddListener(OnBackClicked);
        if (resetDefaultsButton != null) resetDefaultsButton.onClick.AddListener(OnResetDefaults);

        if (lazadaRemapButton != null) lazadaRemapButton.onClick.AddListener(() => StartKeyRemap(PREF_KEY_LAZADA, (k) => { PlayerPrefs.SetInt(PREF_KEY_LAZADA, (int)k); UpdateAllKeyLabels(); }));
        if (shopeeRemapButton != null) shopeeRemapButton.onClick.AddListener(() => StartKeyRemap(PREF_KEY_SHOPEE, (k) => { PlayerPrefs.SetInt(PREF_KEY_SHOPEE, (int)k); UpdateAllKeyLabels(); }));

        if (conflictReplaceButton != null) conflictReplaceButton.onClick.AddListener(OnConflictReplace);
        if (conflictCancelButton != null) conflictCancelButton.onClick.AddListener(OnConflictCancel);

        if (alertOkButton != null) alertOkButton.onClick.AddListener(() => { if (alertPanel != null) alertPanel.SetActive(false); });

        if (conflictPanel != null) conflictPanel.SetActive(false);
        if (alertPanel != null) alertPanel.SetActive(false);

        UpdateAllKeyLabels();
    }

    void Update()
    {
        if (waitingForKey)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kc in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kc))
                    {
                        waitingForKey = false;
                        HandleCapturedKey(kc);
                        break;
                    }
                }
            }
        }
    }

    void OnVolumeChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(value);
        PlayerPrefs.SetFloat(PREF_VOLUME, value);
    }

    void OnBackClicked()
    {
        if (IsAnyKeyEmpty())
        {
            ShowAlert("One or more store keys are unassigned. Please assign keys before leaving Options.");
            return;
        }

        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    void OnResetDefaults()
    {
        PlayerPrefs.SetFloat(PREF_VOLUME, 1f);
        if (volumeSlider != null) volumeSlider.value = 1f;
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(1f);

        PlayerPrefs.SetInt(PREF_KEY_LAZADA, (int)KeyCode.UpArrow);
        PlayerPrefs.SetInt(PREF_KEY_SHOPEE, (int)KeyCode.DownArrow);

        UpdateAllKeyLabels();
    }

    void StartKeyRemap(string prefKey, Action<KeyCode> onCaptured)
    {
        if (waitingForKey) return;
        waitingForKey = true;
        currentTargetPref = prefKey;
        onKeyCaptured = onCaptured;

        if (prefKey == PREF_KEY_LAZADA && lazadaLabel != null) lazadaLabel.text = "Lazada: Press any key...";
        if (prefKey == PREF_KEY_SHOPEE && shopeeLabel != null) shopeeLabel.text = "Shopee: Press any key...";
    }

    void HandleCapturedKey(KeyCode kc)
    {
        string owner = GetOwnerPrefKey(kc);

        if (!string.IsNullOrEmpty(owner) && owner != currentTargetPref)
        {
            pendingCapturedKey = kc;
            pendingOwner = owner;
            pendingTarget = currentTargetPref;

            if (conflictPanel != null && conflictText != null)
            {
                string ownerName = PrefKeyToStoreName(owner);
                string targetName = PrefKeyToStoreName(currentTargetPref);
                conflictText.text = $"The key {kc} is already assigned to {ownerName}.\nReplace it and assign to {targetName}?";
                conflictPanel.SetActive(true);
            }
            else
            {
                CancelRemapVisual(currentTargetPref);
            }
            return;
        }

        AssignKeyToPref(currentTargetPref, kc);
        onKeyCaptured?.Invoke(kc);
        onKeyCaptured = null;
        currentTargetPref = null;
        pendingCapturedKey = KeyCode.None;
        UpdateAllKeyLabels();
    }

    void OnConflictReplace()
    {
        if (string.IsNullOrEmpty(pendingOwner) || string.IsNullOrEmpty(pendingTarget))
        {
            CloseConflictPanel();
            return;
        }

        // Clear previous owner
        PlayerPrefs.SetInt(pendingOwner, (int)KeyCode.None);

        // Assign new key to target
        AssignKeyToPref(pendingTarget, pendingCapturedKey);

        UpdateAllKeyLabels();

        pendingCapturedKey = KeyCode.None;
        pendingOwner = null;
        pendingTarget = null;
        CloseConflictPanel();
    }

    void OnConflictCancel()
    {
        CancelRemapVisual(pendingTarget);
        pendingCapturedKey = KeyCode.None;
        pendingOwner = null;
        pendingTarget = null;
        CloseConflictPanel();
    }

    void CloseConflictPanel()
    {
        if (conflictPanel != null) conflictPanel.SetActive(false);
        waitingForKey = false;
        currentTargetPref = null;
        onKeyCaptured = null;
    }

    void CancelRemapVisual(string prefKey)
    {
        if (prefKey == PREF_KEY_LAZADA && lazadaLabel != null) lazadaLabel.text = "Lazada: " + GetKeyName(PREF_KEY_LAZADA, KeyCode.LeftArrow);
        if (prefKey == PREF_KEY_SHOPEE && shopeeLabel != null) shopeeLabel.text = "Shopee: " + GetKeyName(PREF_KEY_SHOPEE, KeyCode.DownArrow);
    }

    void AssignKeyToPref(string prefKey, KeyCode kc)
    {
        if (string.IsNullOrEmpty(prefKey)) return;
        PlayerPrefs.SetInt(prefKey, (int)kc);
    }

    string GetOwnerPrefKey(KeyCode kc)
    {
        if ((KeyCode)PlayerPrefs.GetInt(PREF_KEY_LAZADA, (int)KeyCode.LeftArrow) == kc) return PREF_KEY_LAZADA;
        if ((KeyCode)PlayerPrefs.GetInt(PREF_KEY_SHOPEE, (int)KeyCode.DownArrow) == kc) return PREF_KEY_SHOPEE;
        return null;
    }

    string PrefKeyToStoreName(string prefKey)
    {
        switch (prefKey)
        {
            case PREF_KEY_LAZADA: return "Lazada";
            case PREF_KEY_SHOPEE: return "Shopee";
            default: return "Unknown";
        }
    }

    void UpdateAllKeyLabels()
    {
        if (lazadaLabel != null) lazadaLabel.text = "Lazada: " + GetKeyName(PREF_KEY_LAZADA, KeyCode.LeftArrow);
        if (shopeeLabel != null) shopeeLabel.text = "Shopee: " + GetKeyName(PREF_KEY_SHOPEE, KeyCode.DownArrow);
    }

    string GetKeyName(string prefKey, KeyCode defaultKey)
    {
        int val = PlayerPrefs.GetInt(prefKey, (int)defaultKey);
        KeyCode kc = (KeyCode)val;
        if (kc == KeyCode.None) return "Unassigned";
        return kc.ToString();
    }

    bool IsAnyKeyEmpty()
    {
        if ((KeyCode)PlayerPrefs.GetInt(PREF_KEY_LAZADA, (int)KeyCode.LeftArrow) == KeyCode.None) return true;
        if ((KeyCode)PlayerPrefs.GetInt(PREF_KEY_SHOPEE, (int)KeyCode.DownArrow) == KeyCode.None) return true;
        return false;
    }

    void ShowAlert(string message)
    {
        if (alertPanel != null && alertText != null)
        {
            alertText.text = message;
            alertPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }
}
