using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public class PopupTextMesh : TMPPopupText
    {
        public void Initialize(PopupTextManager popupTextManager, Vector3 position, float value = 0, string format = null, Transform targetToFollow = null)
        {
            PopupTextManager = popupTextManager;
            SetText(value, format);
        }

        public void Initialize(PopupTextManager popupTextManager, Vector3 position, string text = "", Transform targetToFollow = null)
        {
            PopupTextManager = popupTextManager;
            SetText(text);
        }

        public override void SetColor(Color color)
        {
            TextComponent.color = color;
        }
    }
}