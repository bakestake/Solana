using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamegaard
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class RectObjectMover : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2 borderSize = new Vector2(20, 20);
        [SerializeField] private bool setOverAllOnDrag;
        [SerializeField] private RectTransform canvasRect;
        private PolygonCollider2D polygonCollider;
        private Vector2 initialOffset;

        private void Awake()
        {
            polygonCollider = GetComponent<PolygonCollider2D>();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (IsPointerOverPolygon(eventData))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition);
                initialOffset = (Vector2)target.localPosition - localPointerPosition;
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (setOverAllOnDrag && IsPointerOverPolygon(eventData))
            {
                transform.SetAsLastSibling();
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (IsPointerOverPolygon(eventData))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition);
                Vector2 newPosition = localPointerPosition + initialOffset;
                CheckBounds(newPosition);
            }
        }

        private bool IsPointerOverPolygon(PointerEventData eventData)
        {
            Vector3 worldPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(target, eventData.position, eventData.pressEventCamera, out worldPosition);
            return polygonCollider.OverlapPoint(worldPosition);
        }

        public void CheckLocalBounds()
        {
            CheckBounds(target.anchoredPosition);
        }

        public void CheckBounds(Vector2 newPosition)
        {
            float width = target.rect.width * target.localScale.x;
            float height = target.rect.height * target.localScale.y;

            float angleRad = target.eulerAngles.z * Mathf.Deg2Rad;
            float cosAngle = Mathf.Cos(angleRad);
            float sinAngle = Mathf.Sin(angleRad);

            float rotatedWidth = Mathf.Abs(width * cosAngle) + Mathf.Abs(height * sinAngle);
            float rotatedHeight = Mathf.Abs(width * sinAngle) + Mathf.Abs(height * cosAngle);

            float pivotOffsetX = rotatedWidth * target.pivot.x;
            float pivotOffsetY = rotatedHeight * target.pivot.y;

            float minPosX = canvasRect.rect.xMin + borderSize.x + (cosAngle >= 0 ? pivotOffsetX : rotatedWidth - pivotOffsetX);
            float maxPosX = canvasRect.rect.xMax - borderSize.x - (cosAngle >= 0 ? rotatedWidth - pivotOffsetX : pivotOffsetX);
            float minPosY = canvasRect.rect.yMin + borderSize.y + (sinAngle >= 0 ? pivotOffsetY : rotatedHeight - pivotOffsetY);
            float maxPosY = canvasRect.rect.yMax - borderSize.y - (sinAngle >= 0 ? rotatedHeight - pivotOffsetY : pivotOffsetY);

            float clampedX = Mathf.Clamp(newPosition.x, minPosX, maxPosX);
            float clampedY = Mathf.Clamp(newPosition.y, minPosY, maxPosY);

            target.localPosition = new Vector2(clampedX, clampedY);
        }
    }
}
