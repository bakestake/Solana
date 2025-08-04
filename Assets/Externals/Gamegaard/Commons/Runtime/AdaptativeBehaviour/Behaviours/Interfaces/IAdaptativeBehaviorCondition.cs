using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    public interface IAdaptativeBehaviorCondition
    {
        bool Evaluate(GameObject gameObject);
    }
}
