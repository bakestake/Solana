using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.InLevelUpgrades
{
    public class CardHoldOnObject : SizeUpHoldOnObject
    {
        [SerializeField] private Image image;
        [SerializeField] private Color targetColor = Color.white;
        private Color originalColor;

        protected override void Awake()
        {
            base.Awake();
            originalColor = image.color;
        }

        protected override void Update()
        {
            base.Update();
            image.color = Color.Lerp(originalColor, targetColor, sizeCurve.Evaluate(Percentage));
        }
    }
}