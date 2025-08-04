using UnityEngine;

namespace Gamegaard.CursorSystem
{
    [System.Serializable]
    public abstract class BaseCursorData : ScriptableObject
    {
        [SerializeField] private Sprite uiIcon;
        [SerializeField] private int priority;

        public Sprite UIIcon => uiIcon;
        public int Priority => priority;
    }
}