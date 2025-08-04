using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class MoveOverTime : LinearVectorBehaviour
    {
        protected override Vector3 AffectedValue
        {
            get => transform.position;
            set => transform.position = value;
        }
    }
}