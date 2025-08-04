using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class ScaleOverTime : LinearVectorBehaviour
    {
        protected override Vector3 AffectedValue
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }
    }
}