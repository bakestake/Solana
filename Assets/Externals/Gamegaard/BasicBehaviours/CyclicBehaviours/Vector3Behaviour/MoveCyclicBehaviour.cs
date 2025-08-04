using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class MoveCyclicBehaviour : Vector3CyclicBehaviour
    {
        protected override Vector3 CurrentValue
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
    }
}