using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class SmoothLookAtTarget : TransformTargetBasicBehaviour
    {
        [SerializeField] private float rotationSpeed = 5f;

        protected override void UpdateBehaviour()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }
}