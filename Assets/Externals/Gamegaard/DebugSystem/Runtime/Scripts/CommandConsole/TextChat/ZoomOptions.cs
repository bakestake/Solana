using Gamegaard.CustomValues;
using TMPro;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class ZoomOptions : MonoBehaviour
    {
        [SerializeField] private RangedStepFloatValue zoom;
        [SerializeField] private TextChat textChat;
        [SerializeField] private TextMeshProUGUI percentageText;

        public void AddZoom(float value)
        {
            zoom.CurrentValue += value;
            OnZoomChanged();
        }

        public void SubtractZoom(float value)
        {
            zoom.CurrentValue -= value;
            OnZoomChanged();
        }

        public void ResetZoom()
        {
            zoom.CurrentValue = 0;
            OnZoomChanged();
        }

        private void OnZoomChanged()
        {
            percentageText.text = $"{(zoom.CurrentValue * 100):00}%";
            textChat.SetZoom(zoom.CurrentValue);
        }
    }
}