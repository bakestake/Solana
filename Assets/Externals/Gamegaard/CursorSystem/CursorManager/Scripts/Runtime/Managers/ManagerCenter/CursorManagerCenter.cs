using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public class CursorManagerCenter : MonoBehaviour
    {
        [SerializeField] private List<CursorManagerID> cursorManagers;

        private string currentID = null;

        public CursorManagerBase CurrentCursorManager { get; private set; }

        private void OnEnable()
        {
            CursorEvents.OnShowCursor += ShowCursor;
            CursorEvents.OnHideCursor += HideCursor;
            CursorEvents.OnSetCursorToScreenPosition += SetCursorToScreenPosition;
            CursorEvents.OnApplyCursorState += ApplyCursorState;
            CursorEvents.OnReleaseAll += ReleaseAll;
            CursorEvents.OnSetCursorState += SetCursorState;
            CursorEvents.OnRequestCursor += RequestCursor;
            CursorEvents.OnReleaseCursor += ReleaseCursor;
            CursorEvents.OnSetDefaultCursor += SetDefaultCursor;
        }

        private void OnDisable()
        {
            CursorEvents.OnShowCursor -= ShowCursor;
            CursorEvents.OnHideCursor -= HideCursor;
            CursorEvents.OnSetCursorToScreenPosition -= SetCursorToScreenPosition;
            CursorEvents.OnApplyCursorState -= ApplyCursorState;
            CursorEvents.OnReleaseAll -= ReleaseAll;
            CursorEvents.OnSetCursorState -= SetCursorState;
            CursorEvents.OnRequestCursor -= RequestCursor;
            CursorEvents.OnReleaseCursor -= ReleaseCursor;
            CursorEvents.OnSetDefaultCursor -= SetDefaultCursor;
        }

        private void Start()
        {
            if (cursorManagers.Count == 0)
            {
                Debug.LogWarning("Cursor Manager Center has no valid Cursor Manager.");
                return;
            }
            SetActiveManager(cursorManagers[0].id);
        }

        public void SetActiveManager(string id)
        {
            if (currentID == id) return;

            foreach (CursorManagerID cursorManager in cursorManagers)
            {
                if (cursorManager.id == id)
                {
                    if (CurrentCursorManager != null)
                    {
                        CurrentCursorManager.gameObject.SetActive(false);
                    }
                    CurrentCursorManager = cursorManager.GetOrCreate(transform);
                    CurrentCursorManager.gameObject.SetActive(true);
                    currentID = id;
                    break;
                }
            }
        }

        public void ApplyCursorState(CursorIdentifierData player, string stateName)
        {
            CurrentCursorManager.SetCursorState(stateName, player.CursorID);
        }

        public void SetCursorToScreenPosition(CursorIdentifierData player, Vector2 position)
        {
            CurrentCursorManager.SetCursorToScreenPosition(position, player.CursorID);
        }

        public void HideCursor(CursorIdentifierData player)
        {
            CurrentCursorManager.HideCursor(player.CursorID);
        }

        public void ShowCursor(CursorIdentifierData player)
        {
            CurrentCursorManager.ShowCursor(player.CursorID);
        }

        public void ReleaseAll(CursorIdentifierData player)
        {
            CurrentCursorManager.ReleaseAll(player.CursorID);
        }

        public void SetCursorState(CursorIdentifierData player, string stateName)
        {
            CurrentCursorManager.SetCursorState(stateName, player.CursorID);
        }

        public void RequestCursor(CursorIdentifierData player, BaseCursorData cursorData, object requester, string stateName = "Default")
        {
            CurrentCursorManager.RequestCursorAsGeneric(cursorData, requester, stateName, player.CursorID);
        }

        public void ReleaseCursor(CursorIdentifierData player, BaseCursorData cursorData, object requester)
        {
            CurrentCursorManager.ReleaseCursorAsGeneric(cursorData, requester, player.CursorID);
        }

        public void SetDefaultCursor(CursorIdentifierData player, BaseCursorData cursorData)
        {
            CurrentCursorManager.SetDefaultCursorAsGeneric(cursorData, player.CursorID);
        }
    }
}