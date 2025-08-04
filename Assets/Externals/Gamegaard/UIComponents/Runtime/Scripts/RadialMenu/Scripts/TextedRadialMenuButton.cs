using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Gamegaard.RadialMenu
{
    public class TextedRadialMenuButton : RadialMenuButton
    {
        [SerializeField] private TextMeshProUGUI buttonText;
        public override void InitializeValues(int buttonIndex, Sprite sprite, UnityEvent callback, string text = "")
        {
            base.InitializeValues(buttonIndex, sprite, callback, text);
            buttonText.text = text;
        }
    }
}