using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ButtonDoubleClickListner : MonoBehaviour, IPointerClickHandler
{
    [Range(0.01f, 0.5f)] public float doubleClickDuration = 0.4f;
    [SerializeField] private UnityEvent onDoubleClick;

    private float lastClickTime = 0f;

    public void OnPointerClick(PointerEventData eventData)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickDuration)
        {
            onDoubleClick?.Invoke();
            lastClickTime = 0f;
        }
        else
        {
            lastClickTime = Time.time;
        }
    }
}