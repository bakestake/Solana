using Bakeland;
using UnityEngine;

namespace Gamegaard
{
    public abstract class ValuedSettingsSliderBehaviour : ValuedSliderBehaviour
    {
        protected PersistentSettings persistentSettings;
        public abstract float SettingsValue { get; protected set; }

        protected virtual void Start()
        {
            persistentSettings = PersistentSettings.Instance;
            float value = isPercentage ? Mathf.Lerp(slider.minValue, slider.maxValue, SettingsValue) : SettingsValue;
            slider.SetValueWithoutNotify(value);
            UpdateVisual();
        }

        protected override void OnValueChanged(float value)
        {
            SettingsValue = isPercentage ? CurrentPercentage : value;
            base.OnValueChanged(value);
        }
    }
}