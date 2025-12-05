// LevelBannerController.cs
using UnityEngine;
using TMPro;

public class LevelBannerController : MonoBehaviour
{
    public TMP_Text label;

    void Awake()
    {
        if (label == null) label = GetComponentInChildren<TMP_Text>();
    }

    public void SetText(string text)
    {
        if (label != null) label.text = text;
    }
}
