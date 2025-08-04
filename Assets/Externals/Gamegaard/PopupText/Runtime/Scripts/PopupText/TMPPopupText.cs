using TMPro;
using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public abstract class TMPPopupText : PopupTextBaseGeneric<TMP_Text>
    {
        public override void SetText(string text)
        {
            base.SetText(text);
            TextComponent.text = text;
        }

        public void SetGradientColor(VertexGradient vertexGradient)
        {
            TextComponent.colorGradient = vertexGradient;
        }

        public void SetGradientColor(Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
        {
            TextComponent.colorGradient = new VertexGradient(topLeft, topRight, bottomLeft, bottomRight);
        }

        public void SetFontMaterial(TMP_FontAsset font)
        {
            TextComponent.font = font;
        }

        public TMP_FontAsset GetFontMaterial()
        {
            return TextComponent.font;
        }
    }
}