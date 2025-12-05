using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHovClickState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Target Graphic (optional)")]
    [Tooltip("If left empty, will try to use the Button's Image component.")]
    public Graphic targetGraphic;

    [Header("Hover Settings")]
    public bool useHoverColor = true;
    public Color hoverColor = Color.yellow;

    [Header("Click Settings")]
    public bool useClickColor = true;
    public Color clickColor = Color.green;

    private Color originalColor;

    void Awake()
    {
        if (targetGraphic == null)
        {
            targetGraphic = GetComponent<Graphic>();
        }

        if (targetGraphic != null)
        {
            originalColor = targetGraphic.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useHoverColor && targetGraphic != null)
        {
            targetGraphic.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetGraphic != null)
        {
            targetGraphic.color = originalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (useClickColor && targetGraphic != null)
        {
            targetGraphic.color = clickColor;
        }
    }
}
