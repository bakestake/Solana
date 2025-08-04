using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public abstract class CursorData2D<T> : BaseCursorData
    {
        [SerializeField] private CursorState<T> defaultState = new CursorState<T>("Default", new T[1], Vector2.zero);
        [SerializeField] private CursorState<T>[] customStates = new CursorState<T>[0];

        public CursorState<T> DefaultState => defaultState;
        public CursorState<T>[] CustomStates => customStates;

        public CursorState<T> GetState(string stateName)
        {
            if (stateName == defaultState.Name)
                return defaultState;

            foreach (CursorState<T> state in customStates)
            {
                if (state.Name == stateName)
                    return state;
            }

            Debug.LogWarning($"State '{stateName}' not found. Using default state '{defaultState.Name}'.");
            return defaultState;
        }
    }
}