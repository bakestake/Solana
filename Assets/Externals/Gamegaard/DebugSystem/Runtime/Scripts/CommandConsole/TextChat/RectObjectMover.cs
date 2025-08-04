using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamegaard.RuntimeDebug
{
    public class RectObjectMover : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] protected RectTransform target;
        [SerializeField] private Canvas canvas;
        [SerializeField] protected Vector2 borderSize = new Vector2(20, 20);

        private RectTransform canvasRect;
        private Vector2 initialOffset;

        private void Awake()
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition);
            initialOffset = (Vector2)target.localPosition - localPointerPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition);
            Vector2 newPosition = localPointerPosition + initialOffset;

            float minPosX = canvasRect.rect.min.x + borderSize.x + (target.pivot.x * target.rect.width);
            float maxPosX = canvasRect.rect.max.x - borderSize.x - ((1 - target.pivot.x) * target.rect.width);
            float minPosY = canvasRect.rect.min.y + borderSize.y + (target.pivot.y * target.rect.height);
            float maxPosY = canvasRect.rect.max.y - borderSize.y - ((1 - target.pivot.y) * target.rect.height);

            float clampedX = Mathf.Clamp(newPosition.x, minPosX, maxPosX);
            float clampedY = Mathf.Clamp(newPosition.y, minPosY, maxPosY);

            target.localPosition = new Vector2(clampedX, clampedY);
        }
    }
}
