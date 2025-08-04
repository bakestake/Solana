using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public abstract class LinearVectorBehaviour : MonoBehaviour
    {
        [SerializeField] protected Vector3 direction;
        [SerializeField] protected float speed;

        protected abstract Vector3 AffectedValue { get; set; }

        private void Update()
        {
            AffectedValue += speed * Time.deltaTime * direction;
        }

        public void SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }
    }
}