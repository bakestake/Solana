using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bakeland
{
    public class ButtonAudio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] private AudioClip onClickClip, onHoverClip;
        private float hoverClipDelay = 0f;

        private void OnEnable()
        {
            if (TryGetComponent(out Button button) == false)
                Debug.LogWarning($"{name}: no Button attached!");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClickClip != null)
            {
                SoundManager.Instance.PlaySfx(onClickClip);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (onHoverClip != null && Time.time > hoverClipDelay)
            {
                SoundManager.Instance.PlaySfx(onHoverClip);
                hoverClipDelay = Time.time + 1f;
            }
        }
    }
}