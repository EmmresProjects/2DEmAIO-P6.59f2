using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class gpDifficultySelector : MonoBehaviour
{
    public GameObject difficultyPanel;
    public Button easyButton, mediumButton, hardButton;
    public GameObject tooltipPanel;
    public Text tooltipText;

    void Start()
    {
        easyButton.onClick.AddListener(() => SetDifficulty("Easy"));
        mediumButton.onClick.AddListener(() => SetDifficulty("Medium"));
        hardButton.onClick.AddListener(() => SetDifficulty("Hard"));

        AddHoverListener(easyButton, "Easy: $1000 start, $100k goal, 1.0x cost");
        AddHoverListener(mediumButton, "Medium: $750 start, $150k goal, 1.1x cost");
        AddHoverListener(hardButton, "Hard: $500 start, $200k goal, 1.2x cost");
    }

    void AddHoverListener(Button button, string tooltip)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => ShowTooltip(tooltip));
        trigger.triggers.Add(entryEnter);
        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => HideTooltip());
        trigger.triggers.Add(entryExit);
    }

    void ShowTooltip(string text)
    {
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
    }

    void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Easy":
                gpGameManager.Instance.SetDifficulty(1000f, 100000f, 1.0f);
                break;
            case "Medium":
                gpGameManager.Instance.SetDifficulty(750f, 150000f, 1.1f);
                break;
            case "Hard":
                gpGameManager.Instance.SetDifficulty(500f, 200000f, 1.2f);
                break;
        }
        difficultyPanel.SetActive(false);
    }
}