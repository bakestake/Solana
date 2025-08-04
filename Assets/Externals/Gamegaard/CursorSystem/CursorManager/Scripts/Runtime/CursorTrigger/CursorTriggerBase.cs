using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public abstract class CursorTriggerBase : MonoBehaviour
    {
        [SerializeField] private CursorReference cursorReference;

        protected virtual void Trigger(CursorIdentifierData Id)
        {
            CursorEvents.RequestCursor(Id, cursorReference.cursor, this, cursorReference.stateName);
        }

        protected virtual void UnTrigger(CursorIdentifierData Id)
        {
            CursorEvents.ReleaseCursor(Id, cursorReference.cursor, this);
        }
    }
}