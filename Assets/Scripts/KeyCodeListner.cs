using Gamegaard.CustomValues;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.RuntimeDebug
{
    public class KeyCodeListner : MonoBehaviour
    {
        [SerializeField] private SerializableValue<IInteractionCondition> condition;
        [SerializeField] private KeyCode keyCode;
        [SerializeField] private UnityEvent OnTrigger;

        private void Update()
        {
            if (Input.GetKeyDown(keyCode) && condition.Value.CanInteract())
            {
                OnTrigger?.Invoke();
            }
        }
    }
}