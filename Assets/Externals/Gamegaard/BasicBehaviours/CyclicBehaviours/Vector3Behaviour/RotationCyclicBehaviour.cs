using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class RotationCyclicBehaviour : Vector3CyclicBehaviour
    {
        protected override Vector3 CurrentValue
        {
            get => transform.localEulerAngles;
            set => transform.localEulerAngles = value;
        }
    }
}