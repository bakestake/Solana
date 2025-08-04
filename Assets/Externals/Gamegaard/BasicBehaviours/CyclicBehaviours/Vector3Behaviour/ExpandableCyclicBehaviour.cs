using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class ExpandableCyclicBehaviour : Vector3CyclicBehaviour
    {
        protected override Vector3 CurrentValue
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }
    }
}