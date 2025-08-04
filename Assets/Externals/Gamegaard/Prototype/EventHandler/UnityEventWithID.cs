using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard
{
    [System.Serializable]
    public struct UnityEventWithID
    {
        [SerializeField] private string id;
        [SerializeField] private UnityEvent gameEvent;

        public string Id => id;
        public UnityEvent GameEvent => gameEvent;

        public UnityEventWithID(string id, UnityEvent gameEvent)
        {
            this.id = id;
            this.gameEvent = gameEvent;
        }
    }
}