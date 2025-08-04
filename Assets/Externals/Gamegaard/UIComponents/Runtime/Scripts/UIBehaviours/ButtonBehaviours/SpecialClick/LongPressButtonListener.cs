using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class LongPressButtonListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Range(0.3f, 5f)]
    [SerializeField] private float longPressDuration = 0.5f;
    [SerializeField] private UnityEvent onLongPress;

    private float lastClickTime = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        lastClickTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick >= longPressDuration)
        {
            onLongPress?.Invoke();
        }
    }
}