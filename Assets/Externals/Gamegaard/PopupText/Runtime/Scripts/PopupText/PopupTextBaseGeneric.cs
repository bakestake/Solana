using UnityEngine;

namespace Gamegaard.UI.PopupText
{
    public abstract class PopupTextBaseGeneric<T> : PopupTextBase where T : Component
    {
        public T TextComponent { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            TextComponent = GetComponent<T>();
        }

        public void SetFollowedTarget(Transform targetToFollow)
        {
            TargetToFollow = targetToFollow;
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}