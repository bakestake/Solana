using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

namespace Gamegaard.RadialMenu
{
    public class RadialMenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image Icon;

        private bool isDestroying;

        public Action OnButtonClick;
        protected int buttonIndex;

        public virtual void InitializeValues(int buttonIndex, Sprite sprite, UnityEvent callback, string text = "")
        {
            this.buttonIndex = buttonIndex;
            Icon.sprite = sprite;
            OnButtonClick += callback.Invoke;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isDestroying) return;

            OnButtonClick?.Invoke();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {

        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {

        }

        public virtual void OnOpen(Vector3 anchoredPostion)
        {

        }

        public virtual void OnClose()
        {
            isDestroying = true;
            Destroy(gameObject);
        }
    }
}