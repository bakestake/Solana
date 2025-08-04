using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class SmartScrollRect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private float scrollSpeed = 10;
    [SerializeField] private bool findSelectablesOnStart;
    [SerializeField] private bool disableOnMouseHover = true;
    [SerializeField] private bool useQuickScroll;

    private readonly List<Selectable> currentSelectables = new List<Selectable>();
    private ScrollRect scrollRect;
    private EventSystem eventSystem;
    private Selectable lastSelected;
    private Vector2 targetScrollPosition;
    private bool isMouseOver;
    private bool isDragging;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        eventSystem = EventSystem.current;
    }

    private void Start()
    {
        if (findSelectablesOnStart)
        {
            FindSelectables();
        }
        targetScrollPosition = scrollRect.normalizedPosition;
    }

    public void FindSelectables()
    {
        scrollRect.content.GetComponentsInChildren(currentSelectables);
    }

    public void SetSelectables(IEnumerable<Selectable> selectables)
    {
        currentSelectables.Clear();
        currentSelectables.AddRange(selectables);
    }

    private void Update()
    {
        if (currentSelectables.Count <= 1) return;
        InputScroll();
        LerpScrollPosition();
    }

    private void InputScroll()
    {
        if (currentSelectables.Count > 0 && (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0))
        {
            ScrollToSelected(useQuickScroll);
        }
    }

    private void LerpScrollPosition()
    {
        if (!isMouseOver && !isDragging)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, targetScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
        }
    }

    private void ScrollToSelected(bool useQuickScroll)
    {
        if (eventSystem.currentSelectedGameObject == null) return;
        if (!eventSystem.currentSelectedGameObject.TryGetComponent(out Selectable selectedElement) || selectedElement == lastSelected) return;

        lastSelected = selectedElement;
        int selectedIndex = currentSelectables.IndexOf(selectedElement);
        if (selectedIndex == -1) return;

        float position = 1 - (selectedIndex / (float)(currentSelectables.Count - 1));
        targetScrollPosition = new Vector2(0, position);
        if (useQuickScroll)
        {
            scrollRect.normalizedPosition = targetScrollPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!disableOnMouseHover) return;
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!disableOnMouseHover) return;
        isMouseOver = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}
