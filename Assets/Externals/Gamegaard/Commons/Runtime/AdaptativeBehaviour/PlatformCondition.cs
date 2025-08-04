using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    [System.Serializable]
    public class PlatformCondition : IAdaptativeBehaviorCondition
    {
        [SerializeField] private PlatformCheckMode checkMode;
        [SerializeField] private RuntimePlatform[] checkedPlatforms;

        public bool IsNegated => checkMode == PlatformCheckMode.ExcludePlatform;

        public PlatformCondition(RuntimePlatform[] checkedPlatforms)
        {
            this.checkedPlatforms = checkedPlatforms;
        }

        public PlatformCondition() { }

        public bool Evaluate(GameObject gameObject)
        {
            bool isPlatformAllowed = IsNegated;
            RuntimePlatform currentPlatform = Application.platform;

            foreach (RuntimePlatform platform in checkedPlatforms)
            {
                if (platform == currentPlatform)
                {
                    isPlatformAllowed = !IsNegated;
                    break;
                }
            }

            return isPlatformAllowed;
        }
    }
}
