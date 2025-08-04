using TMPro;

namespace Gamegaard.RuntimeDebug
{
    public class ZoomableText : SingleComponentBehaviour<TextMeshProUGUI>
    {
        private float zoom;
        private float fontSize;

        public TextMeshProUGUI TextComponent => targetComponent;

        protected override void Awake()
        {
            base.Awake();
            fontSize = targetComponent.fontSize;
        }

        public void AddZoom(float value)
        {
            zoom += value;
            UpdateFontSize();
        }

        public void SubtractZoom(float value)
        {
            zoom -= value;
            UpdateFontSize();
        }

        public void SetZoom(float value)
        {
            zoom = value;
            UpdateFontSize();
        }

        private void UpdateFontSize()
        {
            targetComponent.fontSize = fontSize * zoom;
        }
    }
}