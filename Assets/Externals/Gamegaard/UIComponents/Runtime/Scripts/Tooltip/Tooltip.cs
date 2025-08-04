using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Gamegaard
{
    [DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class Tooltip : MonoBehaviour
    {
        [Header("Positioning")]
        [SerializeField] protected Vector2 offset;
        [SerializeField] protected float padding;

        [Header("References")]
        [SerializeField] protected RectTransform stretchBackground;
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI descriptionText;

        protected VerticalLayoutGroup verticalLayoutGroup;
        protected Canvas canvas;

        protected virtual void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            TooltipManager.Instance.OnCallToolTip += EnableTooltip;
            TooltipManager.Instance.OnEndToolTip += DisableTooltip;
            verticalLayoutGroup = GetComponentInChildren<VerticalLayoutGroup>();
            gameObject.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            TooltipManager.Instance.OnCallToolTip -= EnableTooltip;
            TooltipManager.Instance.OnEndToolTip -= DisableTooltip;
        }

        protected virtual void Update()
        {
            SetPosition();
        }

        protected virtual void SetPosition()
        {
            Vector2 newPosition = (Vector2)Input.mousePosition + offset;
            float scale = canvas.scaleFactor * 0.5f;
            float rectWidth = stretchBackground.rect.width * scale;
            float rectHeight = stretchBackground.rect.height * scale;
            newPosition.x = Mathf.Clamp(newPosition.x, rectWidth + padding, Screen.width - rectWidth - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, rectHeight + padding, Screen.height - rectHeight - padding);

            transform.position = newPosition;
        }

        protected virtual void EnableTooltip(string title, string description)
        {
            SetPosition();
            SetText(title, description);
            gameObject.SetActive(true);
            ResizeToolbar();
        }

        protected virtual void ResizeToolbar()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(stretchBackground);
            Vector2 rect = stretchBackground.sizeDelta;
            rect.y = CalculateTotalHeight();
            stretchBackground.sizeDelta = rect;
        }

        protected virtual void DisableTooltip()
        {
            gameObject.SetActive(false);
        }

        protected virtual void SetText(string title, string description)
        {
            titleText.SetText(title);
            descriptionText.SetText(description);
        }

        protected virtual float CalculateTotalHeight()
        {
            float totalHeight = 0f;

            int childCount = verticalLayoutGroup.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                RectTransform child = verticalLayoutGroup.transform.GetChild(i) as RectTransform;

                if (child != null)
                {
                    totalHeight += child.rect.height;
                }
            }

            totalHeight += (childCount - 1) * verticalLayoutGroup.spacing;
            totalHeight += verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom;

            return totalHeight;
        }
    }
}