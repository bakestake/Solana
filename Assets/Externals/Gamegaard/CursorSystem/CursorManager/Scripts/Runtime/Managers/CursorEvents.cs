using System;
using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public static class CursorEvents
    {
        public static event Action<CursorIdentifierData> OnShowCursor;
        public static event Action<CursorIdentifierData> OnHideCursor;
        public static event Action<CursorIdentifierData, Vector2> OnSetCursorToScreenPosition;
        public static event Action<CursorIdentifierData, string> OnApplyCursorState;
        public static event Action<CursorIdentifierData> OnReleaseAll;
        public static event Action<CursorIdentifierData, string> OnSetCursorState;
        public static event Action<CursorIdentifierData, BaseCursorData, object, string> OnRequestCursor;
        public static event Action<CursorIdentifierData, BaseCursorData, object> OnReleaseCursor;
        public static event Action<CursorIdentifierData, BaseCursorData> OnSetDefaultCursor;

        public static void ShowCursor(CursorIdentifierData player)
        {
            OnShowCursor?.Invoke(player);
        }

        public static void HideCursor(CursorIdentifierData player)
        {
            OnHideCursor?.Invoke(player);
        }

        public static void SetCursorToScreenPosition(CursorIdentifierData player, Vector2 position)
        {
            OnSetCursorToScreenPosition?.Invoke(player, position);
        }

        public static void ApplyCursorState(CursorIdentifierData player, string stateName)
        {
            OnApplyCursorState?.Invoke(player, stateName);
        }

        public static void ReleaseAll(CursorIdentifierData player)
        {
            OnReleaseAll?.Invoke(player);
        }

        public static void SetCursorState(CursorIdentifierData player, string stateName)
        {
            OnSetCursorState?.Invoke(player, stateName);
        }

        public static void RequestCursor(CursorIdentifierData player, BaseCursorData cursorData, object requester, string stateName = "Default")
        {
            OnRequestCursor?.Invoke(player, cursorData, requester, stateName);
        }

        public static void ReleaseCursor(CursorIdentifierData player, BaseCursorData cursorData, object requester)
        {
            OnReleaseCursor?.Invoke(player, cursorData, requester);
        }

        public static void SetDefaultCursor(CursorIdentifierData player, BaseCursorData cursorData)
        {
            OnSetDefaultCursor?.Invoke(player, cursorData);
        }
    }
}