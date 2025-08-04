using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    public class StateConditionReceiver : MonoBehaviour, IStateConditionHandler
    {
        private readonly HashSet<string> conditions = new HashSet<string>();

        public event Action<string> OnConditionAdded;
        public event Action<string> OnConditionRemoved;
        public event Action OnAllConditionsCleared;

        public bool ContainsCondition(string name) => conditions.Contains(name);

        public bool AddCondition(string name)
        {
            if (conditions.Add(name))
            {
                OnConditionAdded?.Invoke(name);
                return true;
            }
            return false;
        }

        public bool RemoveCondition(string name)
        {
            if (conditions.Remove(name))
            {
                OnConditionRemoved?.Invoke(name);
                return true;
            }
            return false;
        }

        public void ClearConditions()
        {
            OnAllConditionsCleared?.Invoke();
            conditions.Clear();
        }
    }
}