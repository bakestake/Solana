using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.Prototype.BasicEvents
{
    public class BasicEventTriggerClass : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnTrigger;

        public void Trigger()
        {
            OnTrigger?.Invoke();
        }
    }
}
