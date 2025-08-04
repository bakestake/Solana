using Gamegaard.CustomValues;
using UnityEngine;
using UnityEngine.Events;

namespace Bakeland
{
    public class ConditionChecker : MonoBehaviour
    {
        [SerializeField] private bool checkOnStart;
        [SerializeField] private SerializableList<IInteractionCondition> conditions;
        [SerializeField] private UnityEvent onConditionMeet;
        [SerializeField] private UnityEvent onConditionFailed;

        private bool isInitialized;
        private bool isMeet;

        private void Start()
        {
            if (!checkOnStart) return;
            Check();
        }

        public void Check()
        {
            if ((!isInitialized || !isMeet) && CheckConditions())
            {
                isMeet = true;
                onConditionMeet?.Invoke();
            }
            else if (!isInitialized || isMeet)
            {
                isMeet = false;
                onConditionFailed?.Invoke();
            }
            isInitialized = true;
        }

        private bool CheckConditions()
        {
            foreach (var condition in conditions)
            {
                if (!condition.CanInteract())
                {
                    return false;
                }
            }
            return true;
        }
    }
}