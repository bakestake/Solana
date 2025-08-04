using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public abstract class TransformTargetBasicBehaviour : MonoBehaviour
    {
        [SerializeField] protected Transform target;

        private void Update()
        {
            if(target != null)
            {
                UpdateBehaviour();
            }
        }

        protected abstract void UpdateBehaviour();

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}