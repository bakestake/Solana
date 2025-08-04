using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class OrbitAroundTarget : TransformTargetBasicBehaviour
    {
        [SerializeField] private float orbitDistance = 5f;
        [SerializeField] private float orbitSpeed = 30f;
        [SerializeField] private Vector3 orbitAxis = Vector3.up;

        private void Start()
        {
            if (target != null)
            {
                transform.position = target.position + (transform.position - target.position).normalized * orbitDistance;
            }
        }

        protected override void UpdateBehaviour()
        {
            transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);

            Vector3 direction = (transform.position - target.position).normalized;
            transform.position = target.position + direction * orbitDistance;
        }

        public void SetOrbitAxis(Vector3 orbitAxis)
        {
            this.orbitAxis = orbitAxis;
        }

        public void SetOrbitDistance(float orbitDistance)
        {
            this.orbitDistance = orbitDistance;
        }

        public void SetOrbitSpeed(float orbitSpeed)
        {
            this.orbitSpeed = orbitSpeed;
        }
    }
}