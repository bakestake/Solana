using UnityEngine.UI;

namespace Gamegaard.Commons
{
    public abstract class ButtonBehaviour : SingleComponentBehaviour<Button>
    {
        protected override void Awake()
        {
            base.Awake();
            targetComponent.onClick.AddListener(OnClick);
        }

        public abstract void OnClick();
    }
}