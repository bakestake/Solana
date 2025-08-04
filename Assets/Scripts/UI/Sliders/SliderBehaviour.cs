using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public abstract class SliderBehaviour : MonoBehaviour
    {
        protected Slider slider;

        protected float CurrentPercentage
        {
            get => (slider.value - slider.minValue) / (slider.maxValue - slider.minValue);
            set => slider.value = Mathf.Lerp(slider.minValue,slider.maxValue, value);
        }

        protected float CurrentValue
        {
            get => slider.value;
            set => slider.value = value;
        }

        protected string FormatedPercentage => $"{CurrentPercentage * 100:00}%";

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        protected abstract void OnValueChanged(float value);
    }
}