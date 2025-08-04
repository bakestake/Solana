using UnityEngine;
using UnityEngine.EventSystems;

public class DragableElement : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private bool setOverAll;

    private Canvas canvas;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (setOverAll)
        {
            rectTransform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}