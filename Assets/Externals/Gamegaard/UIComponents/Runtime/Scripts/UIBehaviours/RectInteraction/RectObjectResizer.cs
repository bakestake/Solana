using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamegaard.RuntimeDebug
{
    public class RectObjectResizer : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] protected RectTransform target;
        [SerializeField] protected Vector2 minSize = new Vector2(200, 200);
        [SerializeField] protected Vector2 maxSizeBorder = new Vector2(20, 20);
        [SerializeField] private Corner resizeFromCorner = Corner.BottomRight;

        protected Vector2 initialCursorPosition;
        protected Vector2 initialSize;
        protected Vector2 initialLocalPointerPosition;
        protected Vector2 cursorOffset;
        protected Canvas canvas;
        protected RectTransform canvasRect;

        private void Awake()
        {
            canvas = transform.root.GetComponent<Canvas>();
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        private void Start()
        {
            UpdatePivot(resizeFromCorner);
        }

        private void UpdatePivot(Corner corner)
        {
            switch (corner)
            {
                case Corner.TopLeft:
                    target.pivot = new Vector2(1, 0);
                    break;
                case Corner.TopRight:
                    target.pivot = new Vector2(0, 0);
                    break;
                case Corner.BottomLeft:
                    target.pivot = new Vector2(1, 1);
                    break;
                case Corner.BottomRight:
                    target.pivot = new Vector2(0, 1);
                    break;
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            cursorOffset = (Vector2)transform.position - eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out initialLocalPointerPosition);
            initialCursorPosition = eventData.position;
            initialSize = target.sizeDelta;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out Vector2 currentCursorPosition);
            Vector2 positionDifference = currentCursorPosition - initialLocalPointerPosition;
            Vector2 newSize = initialSize + new Vector2(positionDifference.x * (target.pivot.x == 0 ? 1 : -1), positionDifference.y * (target.pivot.y == 0 ? 1 : -1));

            float maxX = (target.pivot.x == 0 ? canvasRect.sizeDelta.x - (target.anchoredPosition.x + initialSize.x * target.pivot.x) - maxSizeBorder.x : target.anchoredPosition.x + initialSize.x * (1 - target.pivot.x) - maxSizeBorder.x);
            float maxY = (target.pivot.y == 0 ? canvasRect.sizeDelta.y - (target.anchoredPosition.y + initialSize.y * target.pivot.y) - maxSizeBorder.y : target.anchoredPosition.y + initialSize.y * (1 - target.pivot.y) - maxSizeBorder.y);

            newSize.x = Mathf.Clamp(newSize.x, minSize.x, maxX);
            newSize.y = Mathf.Clamp(newSize.y, minSize.y, maxY);

            target.sizeDelta = newSize;
        }
    }
}