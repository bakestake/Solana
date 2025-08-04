using UnityEngine;

namespace Gamegaard
{
    public class ColoredFillBarUI : FillBarUI
    {
        private Color originalColor;

        protected virtual void Awake()
        {
            originalColor = attrFillBar.color;
        }


        public void SetBarColor(Color color)
        {
            attrFillBar.color = color;
        }

        public void ResetBarColor()
        {
            attrFillBar.color = originalColor;
        }
    }
}