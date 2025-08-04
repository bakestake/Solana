using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class LookAtTarget : TransformTargetBasicBehaviour
    {
        protected override void UpdateBehaviour()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = lookRotation;
        }
    }
}