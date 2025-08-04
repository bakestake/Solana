using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public class PopupTextGUI : TMPPopupText
    {
        private RectTransform rectParent;

        public void SetRectParent(RectTransform rectParent)
        {
            this.rectParent = rectParent;
        }

        public void Initialize(string text, PopupTextManager popupTextManager, RectTransform rectParent, Vector2 position, Transform followedTarget = null)
        {
            Initialize(text, popupTextManager);
            SetRectParent(rectParent);
        }

        public override void SetColor(Color color)
        {
            TextComponent.color = color;
        }
    }
}