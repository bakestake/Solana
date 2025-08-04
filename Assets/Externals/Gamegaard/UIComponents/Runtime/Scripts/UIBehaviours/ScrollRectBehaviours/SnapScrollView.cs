using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class SnapScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("Config")]
    [Min(0)]
    [SerializeField] private float snapForce = 100;
    [SerializeField] private bool isInputAllowed;

    [Header("Events")]
    [SerializeField] private UnityEvent<Transform> onSelected;
    [SerializeField] private UnityEvent onSnapExit;

    private ScrollRect scrollRect;
    private RectTransform content;
    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private Vector2 elementSize;
    private float snapSpeed;
    private float horizontalSpaceValue;
    private int currentItem;
    private bool isSnapped;
    private bool isDragging;
    private bool isTargeted;
    private float lastHorizontalInput;

    public int CurrentItem
    {
        get => currentItem;
        set => currentItem = Mathf.Clamp(value, 0, content.childCount - 1);
    }

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        layoutGroup = content.GetComponent<HorizontalOrVerticalLayoutGroup>();

        elementSize = content.GetChild(0).GetComponent<RectTransform>().rect.size;
        horizontalSpaceValue = elementSize.x + layoutGroup.spacing;
    }

    private void Update()
    {
        if (!HandleInputs() && !isTargeted)
        {
            CurrentItem = Mathf.RoundToInt(-content.localPosition.x / horizontalSpaceValue);
        }

        if (!isDragging && scrollRect.velocity.magnitude < 50)
        {
            SnapToItem(currentItem);
        }
        else if (isSnapped && scrollRect.velocity.magnitude > 0)
        {
            isSnapped = false;
            onSnapExit?.Invoke();
            snapSpeed = 0;
        }
    }

    private bool HandleInputs()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput != lastHorizontalInput)
        {
            lastHorizontalInput = horizontalInput;
            if (horizontalInput > 0)
            {
                MoveToItem(currentItem + 1);
                return true;
            }
            else if (horizontalInput < 0)
            {
                MoveToItem(currentItem - 1);
                return true;
            }
        }
        return false;
    }


    private void MoveToItem(int targetItem)
    {
        CurrentItem = targetItem;
        isDragging = false;
        isTargeted = true;
    }

    private void SnapToItem(int index)
    {
        float targetX = -index * horizontalSpaceValue;
        if (Mathf.Approximately(content.localPosition.x, targetX))
        {
            if (!isSnapped)
            {
                isSnapped = true;
                isTargeted = false;
                onSelected?.Invoke(content.GetChild(index));
            }
        }
        else
        {
            scrollRect.velocity = Vector2.zero;
            snapSpeed += snapForce * Time.deltaTime;
            content.localPosition = new Vector3(Mathf.MoveTowards(content.localPosition.x, targetX, snapSpeed), content.localPosition.y, content.localPosition.z);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isTargeted = false;
        isDragging = true;
        snapSpeed = 0;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}
