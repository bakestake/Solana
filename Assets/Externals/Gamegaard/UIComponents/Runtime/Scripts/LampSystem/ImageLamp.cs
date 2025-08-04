using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public class ImageLamp : MonoBehaviour, ILamp
    {
        private Image lamp;
        private Color deactiveColor;
        private Color activeColor;

        public bool IsActive { get; private set; }

        private void Awake()
        {
            lamp = GetComponent<Image>();
        }

        public virtual void SetActiveState(bool isActive)
        {
            if (isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        public virtual void Activate()
        {
            IsActive = true;
            lamp.color = activeColor;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            lamp.color = deactiveColor;
        }

        public void SetActiveColor(Color color)
        {
            activeColor = color;
        }

        public void SetDeactiveColor(Color color)
        {
            deactiveColor = color;
        }
    }
}