using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class gpBuildingSelector : MonoBehaviour
{
    public static gpBuildingSelector Instance;
    public Button[] buildingButtons;
    public GameObject tooltipPanel;
    public Text tooltipText;
    public Image selectOutline;
    public AudioSource buttonClickSound; // Assign in Inspector
    private int selectedBuildingIndex = -1;
    private bool isMouseOverAnyButton = false;
    private int currentTooltipIndex = -1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            int index = i;
            buildingButtons[i].onClick.AddListener(() => SelectBuilding(index));
            EventTrigger trigger = buildingButtons[i].gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => OnPointerEnter(index));
            trigger.triggers.Add(entryEnter);
            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((data) => OnPointerExit());
            trigger.triggers.Add(entryExit);
        }
    }

    void OnPointerEnter(int index)
    {
        isMouseOverAnyButton = true;
        currentTooltipIndex = index;
        ShowTooltip(index);
    }

    void OnPointerExit()
    {
        isMouseOverAnyButton = false;
        HideTooltip();
    }

    void SelectBuilding(int index)
    {
        if (buttonClickSound != null)
        {
            buttonClickSound.Play();
        }
        if (selectedBuildingIndex == index)
        {
            Deselect();
            gpBuildingPlacer.Instance.StopPlacing();
        }
        else
        {
            selectedBuildingIndex = index;
            selectOutline.transform.position = buildingButtons[index].transform.position;
            selectOutline.gameObject.SetActive(true);
            gpBuildingPlacer.Instance.StartPlacing((gpBuildingType)index);
        }
    }

    void ShowTooltip(int index)
    {
        gpBuildingData data = gpGameManager.Instance.GetBuildingData(index);
        tooltipText.text = $"Cost: ${data.cost:F0}\nIncome: ${data.rent:F0}\nExpenses: ${data.maintenance:F0}";
        tooltipPanel.SetActive(true);
    }

    void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public void Deselect()
    {
        selectedBuildingIndex = -1;
        selectOutline.gameObject.SetActive(false);
    }
}