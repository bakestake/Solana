using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    [System.Serializable]
    public class HideEffect : IAdaptativeBehaviorEffect
    {
        public HideEffect()
        {

        }

        public bool Apply(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                return true;
            }

            return false;
        }
    }
}
