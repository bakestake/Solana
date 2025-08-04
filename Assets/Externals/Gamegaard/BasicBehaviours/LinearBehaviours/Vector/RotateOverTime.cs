using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class RotateOverTime : LinearVectorBehaviour
    {
        protected override Vector3 AffectedValue
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }
    }
}