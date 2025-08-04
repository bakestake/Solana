using UnityEngine;

namespace Gamegaard.CursorSystem
{
    [DisallowMultipleComponent]
    public class CursorTrigger : CursorTriggerBase
    {
        private void OnMouseEnter()
        {
            Trigger(CursorIdentifierData.playerOne);
        }

        private void OnMouseExit()
        {
            UnTrigger(CursorIdentifierData.playerOne);
        }

        public void OnMultiplayerMouseEnter(CursorIdentifierData id)
        {
            Trigger(id);
        }

        public void OnMultiplayerMouseExit(CursorIdentifierData id)
        {
            UnTrigger(id);
        }
    }
}