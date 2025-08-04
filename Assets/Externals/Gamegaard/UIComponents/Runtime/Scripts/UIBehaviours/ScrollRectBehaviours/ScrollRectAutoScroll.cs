using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private float scrollSpeed = 10;

    private readonly List<Selectable> selectables = new List<Selectable>();
    private ScrollRect scrollRect;
    private EventSystem eventSystem;
    private Vector2 targetScrollPosition;
    private bool isMouseOver;
    private bool isDraging;
    private bool isVertical;

    private bool HasSelectables => selectables.Count > 0;
    private int SelectableCount => selectables.Count;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        eventSystem = EventSystem.current;
        scrollRect.content.GetComponentsInChildren(selectables);
        targetScrollPosition = scrollRect.normalizedPosition;
        isVertical = scrollRect.vertical;
    }

    private void Update()
    {
        InputScroll();
        if (!isMouseOver && !isDraging)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, targetScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
        }
        else
        {
            targetScrollPosition = scrollRect.normalizedPosition;
        }
    }

    private void InputScroll()
    {
        if (HasSelectables && Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            ScrollToSelected(false);
        }
    }

    private void ScrollToSelected(bool useQuickScroll)
    {
        if (eventSystem.TryGetSelectable(out Selectable selectedElement))
        {
            int selectedIndex = selectables.IndexOf(selectedElement);
            float percentage = selectedIndex / (float)(SelectableCount - 1);
            Vector2 normalizedPosition = isVertical ? new Vector2(0, 1 - percentage) : new Vector2(percentage, 0);
            if (useQuickScroll)
            {
                scrollRect.normalizedPosition = normalizedPosition;
                targetScrollPosition = scrollRect.normalizedPosition;
            }
            else
            {
                targetScrollPosition = normalizedPosition;
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDraging = false;
    }

    public void AddSelectable(Selectable selectable)
    {
        selectables.Add(selectable);
    }

    public void RemoveSelectable(Selectable selectable)
    {
        selectables.Remove(selectable);
    }
}