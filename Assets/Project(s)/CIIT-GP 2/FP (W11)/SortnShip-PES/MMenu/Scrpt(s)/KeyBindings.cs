using UnityEngine;

/// <summary>
/// Static helper to get current key bindings from PlayerPrefs.
/// Simplified for the Simple MainMenu: only Lazada and Shopee keys are required.
/// </summary>
public static class KeyBindings
{
    const string PREF_KEY_LAZADA = "PREF_KEY_LAZADA";
    const string PREF_KEY_SHOPEE = "PREF_KEY_SHOPEE";

    // Defaults: Lazada = UpArrow, Shopee = DownArrow
    public static KeyCode GetLazada() => (KeyCode)PlayerPrefs.GetInt(PREF_KEY_LAZADA, (int)KeyCode.UpArrow);
    public static KeyCode GetShopee() => (KeyCode)PlayerPrefs.GetInt(PREF_KEY_SHOPEE, (int)KeyCode.DownArrow);

    // Helpers to set keys (used by OptionsManager)
    public static void SetLazada(KeyCode key) => PlayerPrefs.SetInt(PREF_KEY_LAZADA, (int)key);
    public static void SetShopee(KeyCode key) => PlayerPrefs.SetInt(PREF_KEY_SHOPEE, (int)key);
}
