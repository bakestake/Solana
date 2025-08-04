using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class MaintainDistance : TransformTargetBasicBehaviour
    {
        [SerializeField] private float fixedDistance = 5f;

        protected override void UpdateBehaviour()
        {
            Vector3 direction = (transform.position - target.position).normalized;
            transform.position = target.position + direction * fixedDistance;
        }
    }
}