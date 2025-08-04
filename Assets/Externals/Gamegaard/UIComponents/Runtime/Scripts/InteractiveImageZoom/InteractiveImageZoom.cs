using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class InteractiveImageZoom : MonoBehaviour
{
    [Header("Settings")]
    [Min(0)]
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private AnimationCurve zoomSpeedCurve = AnimationCurve.Constant(1, 1, 1);
    [Range(0, 1)]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private AnimationCurve movementSpeedCurve = AnimationCurve.Constant(1, 1, 1);
    [Min(0)]
    [SerializeField] private float minZoom = 0.5f;
    [Min(0)]
    [SerializeField] private float maxZoom = 3;

    [Header("References")]
    [SerializeField] private RectTransform container;
    [SerializeField] private Image targetImage;
    [SerializeField] private RectTransform rectTarget;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> OnValueChanged;

    private RectTransform targetRect;
    private float _currentZoom = 1;

    public float CurrentZoomValue
    {
        get => _currentZoom;
        set => _currentZoom = Mathf.Clamp(value, minZoom, maxZoom);
    }

    public float CurrentZoomPercentage => (_currentZoom - minZoom) / (maxZoom - minZoom);

    private void Awake()
    {
        targetRect = targetImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateZoom(1);
    }

    private void Update()
    {
#if UNITY_EDITOR
        ApplyMouseZoom();
#elif UNITY_ANDROID || UNITY_IOS
        ApplyTouchZoom();
#endif
    }

    private void ApplyMouseZoom()
    {
        if (!IsPointInsideRect(container, Input.mousePosition)) return;

        Vector2 pivotPosition = GetRectPoint(rectTarget, Input.mousePosition);
        if (pivotPosition == Vector2.zero) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float zoomValue = scroll * zoomSpeed * zoomSpeedCurve.Evaluate(CurrentZoomPercentage);
            float lastZoomPercentage = CurrentZoomPercentage;
            AddZoomPercentage(zoomValue);

            if (CurrentZoomPercentage > lastZoomPercentage)
            {
                float value = movementSpeed * movementSpeedCurve.Evaluate(CurrentZoomPercentage);
                targetRect.pivot = Vector2.Lerp(targetRect.pivot, pivotPosition, value);
            }
            else if (CurrentZoomPercentage < lastZoomPercentage)
            {
                float value = movementSpeed * movementSpeedCurve.Evaluate(CurrentZoomPercentage);
                Vector2 inversePivotPosition = (targetRect.pivot + pivotPosition) / 2f;
                targetRect.pivot = Vector2.Lerp(targetRect.pivot, inversePivotPosition, value);
            }
        }
    }

    public void SetSprite(Sprite sprite)
    {
        targetImage.sprite = sprite;
    }

    public void AddZoomPercentage(float percentage)
    {
        SetZoomPercentage(CurrentZoomPercentage + percentage);
    }

    public void SetZoomPercentage(float percentage)
    {
        float newValue = Mathf.Lerp(minZoom, maxZoom, percentage);
        UpdateZoom(newValue);
    }

    private void UpdateZoom(float newValue)
    {
        CurrentZoomValue = newValue;
        targetRect.localScale = Vector3.one * CurrentZoomValue;
        OnValueChanged?.Invoke(CurrentZoomPercentage);
    }

    public void ResetZoom()
    {
        UpdateZoom(1);
        targetRect.pivot = new Vector2(0.5f, 0.5f);
    }

    private bool IsPointInsideRect(RectTransform rectTransform, Vector3 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position);
    }

    private Vector2 GetRectPoint(RectTransform rectTransform, Vector2 position)
    {
        Rect rect = rectTransform.rect;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, null, out Vector2 localPosition);
        float x = Mathf.InverseLerp(0, rect.width, localPosition.x);
        float y = Mathf.InverseLerp(0, rect.height, localPosition.y);

        return new Vector2(x, y);
    }
}

public partial class InteractiveImageZoom : MonoBehaviour
{
#if UNITY_ANDROID || UNITY_IOS
    private Vector2 initialTouchPos1;
    private Vector2 initialTouchPos2;
    private float initialDistance;

    private void ApplyTouchZoom()
    {
        if (Input.touchCount != 2) return;

        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (!IsPointInsideRect(container, touch1.position) || !IsPointInsideRect(container, touch2.position)) return;

        if (touch2.phase == TouchPhase.Began)
        {
            initialTouchPos1 = touch1.position;
            initialTouchPos2 = touch2.position;
            initialDistance = Vector2.Distance(initialTouchPos1, initialTouchPos2);
        }
        else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
        {
            Vector2 currentTouchPos1 = touch1.position;
            Vector2 currentTouchPos2 = touch2.position;
            float currentDistance = Vector2.Distance(currentTouchPos1, currentTouchPos2);

            float zoomDelta = (currentDistance - initialDistance) * zoomSpeed * Time.deltaTime;
            AddZoomPercentage(zoomDelta);

            initialDistance = currentDistance;
        }
    }
#endif
}