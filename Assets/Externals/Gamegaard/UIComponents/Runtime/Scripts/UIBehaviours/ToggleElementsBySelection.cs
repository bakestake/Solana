using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class ToggleElementsBySelection : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private GameObject[] visibleElements;

        private void OnEnable()
        {
            SetElementsActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SetElementsActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetElementsActive(false);
        }

        private void SetElementsActive(bool isActive)
        {
            foreach (var element in visibleElements)
            {
                element.SetActive(isActive);
            }
        }
    }
}