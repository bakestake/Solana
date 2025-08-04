using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    protected RectTransform selectionArea;
    protected Canvas canvas;

    public Vector2 StartPosition { get; private set; }
    public Vector2 EndPosition { get; private set; }

    protected virtual void Awake()
    {
        selectionArea = GetComponent<RectTransform>();
        canvas = selectionArea.GetComponentInParent<Canvas>();
    }

    public virtual void Enable()
    {
        selectionArea.gameObject.SetActive(true);
        selectionArea.sizeDelta = Vector2.zero;
    }

    public virtual void Disable()
    {
        selectionArea.gameObject.SetActive(false);
    }

    public virtual void UpdateSize(Vector2 startPosition, Vector2 endPosition)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        Vector2 lowerLeft = new Vector2(
               Mathf.Min(startPosition.x, endPosition.x),
               Mathf.Min(startPosition.y, endPosition.y));
        Vector2 upperRight = new Vector2(
            Mathf.Max(startPosition.x, endPosition.x),
            Mathf.Max(startPosition.y, endPosition.y));

        Vector2 size = upperRight - lowerLeft;
        size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

        float scaleFactor = canvas.transform.localScale.x;
        Vector2 center = (lowerLeft + upperRight) * 0.5f;
        center /= scaleFactor;

        selectionArea.sizeDelta = size / scaleFactor;
        selectionArea.anchoredPosition = center;
    }
}