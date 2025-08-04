using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Gamegaard.RadialMenu
{
    public class RadialMenuTrigger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private List<RadialMenuButtonInfo> options = new List<RadialMenuButtonInfo>();
        [SerializeField] private UnityEvent OnCallMenu;

        public void CloseMenu()
        {
            RadialMenu.OnCloseRadialMenu?.Invoke();
        }

        private void OnClickCloseMenu(bool hasClickedOnUi)
        {
            if (hasClickedOnUi) return;
            CloseMenu();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;

            RadialMenu.OnRadialMenuTrigger?.Invoke(transform, options);
            OnCallMenu?.Invoke();
        }
    }
}