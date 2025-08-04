using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public abstract class CursorManagerBase : MonoBehaviour
    {
        public abstract BaseCursorData GetDefaultCursorData(int cursorID = 0);
        public abstract BaseCursorData GetActiveCursorData(int cursorID = 0);
        public abstract void SetCursorToScreenPosition(Vector2 position, int cursorID = 0);
        public abstract void HideCursor(int cursorID = 0);
        public abstract void ShowCursor(int cursorID = 0);
        public abstract void ReleaseAll(int cursorID = 0);
        public abstract void SetCursorState(string stateName, int cursorID = 0);
        public abstract void RequestCursorAsGeneric(BaseCursorData cursorData, object requester, string stateName = "Default", int cursorID = 0);
        public abstract void ReleaseCursorAsGeneric(BaseCursorData cursorData, object requester, int cursorID = 0);
        public abstract void SetDefaultCursorAsGeneric(BaseCursorData cursorData, int cursorID = 0);
    }
}