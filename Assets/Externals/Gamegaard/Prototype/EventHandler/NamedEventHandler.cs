using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard
{
    public class NamedEventHandler : MonoBehaviour
    {
        [SerializeField] private List<UnityEventWithID> events;

        private readonly Dictionary<string, UnityEvent> eventsByID = new Dictionary<string, UnityEvent>();

        private void Awake()
        {
            foreach (UnityEventWithID unityEvent in events)
            {
                if (!string.IsNullOrEmpty(unityEvent.Id) && !eventsByID.ContainsKey(unityEvent.Id))
                {
                    eventsByID.Add(unityEvent.Id, unityEvent.GameEvent);
                }
            }
        }

        /// <summary>
        /// Triggers an event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event to trigger.</param>
        public void TriggerEvent(string id)
        {
            if (eventsByID.TryGetValue(id, out UnityEvent gameEvent))
            {
                gameEvent.Invoke();
            }
            else
            {
                Debug.LogWarning($"Event with ID '{id}' not found.");
            }
        }

        /// <summary>
        /// Adds a new event to the handler.
        /// </summary>
        /// <param name="eventWithID">The event with its ID to add.</param>
        public void AddEvent(UnityEventWithID eventWithID)
        {
            if (!string.IsNullOrEmpty(eventWithID.Id) && !eventsByID.ContainsKey(eventWithID.Id))
            {
                eventsByID.Add(eventWithID.Id, eventWithID.GameEvent);
            }
            else
            {
                Debug.LogWarning($"Event with ID '{eventWithID.Id}' already exists or is invalid.");
            }
        }

        /// <summary>
        /// Removes an event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event to remove.</param>
        public void RemoveEvent(string id)
        {
            if (eventsByID.ContainsKey(id))
            {
                eventsByID.Remove(id);
            }
            else
            {
                Debug.LogWarning($"Event with ID '{id}' not found.");
            }
        }

        /// <summary>
        /// Checks if an event with a given ID exists.
        /// </summary>
        /// <param name="id">The ID of the event to check.</param>
        /// <returns>True if the event exists; otherwise, false.</returns>
        public bool ContainsEventID(string id)
        {
            return eventsByID.ContainsKey(id);
        }

        /// <summary>
        /// Clears all registered events.
        /// </summary>
        public void ClearAllEvents()
        {
            eventsByID.Clear();
        }

        /// <summary>
        /// Gets all event IDs currently registered.
        /// </summary>
        /// <returns>A list of all event IDs.</returns>
        public List<string> GetAllEventIDs()
        {
            return new List<string>(eventsByID.Keys);
        }

        /// <summary>
        /// Invokes all registered events.
        /// </summary>
        public void TriggerAllEvents()
        {
            foreach (UnityEvent gameEvent in eventsByID.Values)
            {
                gameEvent.Invoke();
            }
        }
    }
}