using UnityEngine;

public class ToolbarTriggerWithPosition : ToolbarTrigger
{
    [SerializeField] private Vector2 originPivot = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 optionsPivot;
    [SerializeField] private RectTransform origin;

    private Canvas canvas;

    private void Awake()
    {
        if (origin == null) return;
        canvas = origin.root.GetComponent<Canvas>();
    }

    public void Initialize(string optionText, Toolbar toolbar, ToolbarButtonValueInspector[] options, Vector2 optionsPivot, RectTransform origin, Vector2 originPivot)
    {
        canvas = origin.root.GetComponent<Canvas>();
        Initialize(optionText, toolbar, options);
        this.originPivot = originPivot;
        this.optionsPivot = optionsPivot;
        this.origin = origin;
    }

    public override void TriggerToolbar(bool selectFirstElement)
    {
        Vector2 originSize = origin.rect.size * canvas.scaleFactor;
        Vector2 size = GetOffset(originSize, originPivot);
        Vector2 orignSizeOffset = GetOffset(originSize, origin.pivot);
        Vector2 finalPosition = (Vector2)origin.position + size - orignSizeOffset;
        toolbar.OpenToolbar(options, finalPosition, optionsPivot, selectFirstElement);
    }

    private Vector2 GetOffset(Vector2 size, Vector2 offset)
    {
        return size * offset;
    }
}