using TMPro;
using UnityEngine;

namespace Gamegaard
{
    public abstract class ValuedSliderBehaviour : SliderBehaviour
    {
        [SerializeField] protected TextMeshProUGUI valueText;
        [SerializeField] protected bool isPercentage;

        protected virtual void OnEnable()
        {
            UpdateVisual();
        }

        protected virtual void UpdateVisual()
        {
            if (valueText == null) return;
            valueText.text = isPercentage ? FormatedPercentage : slider.value.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            UpdateVisual();
        }
    }
}