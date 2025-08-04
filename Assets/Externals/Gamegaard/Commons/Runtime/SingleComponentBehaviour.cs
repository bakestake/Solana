using UnityEngine;

namespace Gamegaard
{
    public class SingleComponentBehaviour<T> : MonoBehaviour where T : Component
    {
        protected T targetComponent;

        protected virtual void Awake()
        {
            targetComponent = GetComponent<T>();
        }
    }
}