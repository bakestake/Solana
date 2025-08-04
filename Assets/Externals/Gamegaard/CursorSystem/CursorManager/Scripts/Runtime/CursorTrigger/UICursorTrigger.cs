using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamegaard.CursorSystem
{
    [DisallowMultipleComponent]
    public class UICursorTrigger : CursorTriggerBase, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Trigger(CursorIdentifierData.playerOne);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UnTrigger(CursorIdentifierData.playerOne);
        }
    }
}