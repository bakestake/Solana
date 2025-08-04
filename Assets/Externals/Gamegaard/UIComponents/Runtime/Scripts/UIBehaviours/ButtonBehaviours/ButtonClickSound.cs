using UnityEngine;
using UnityEngine.EventSystems;

namespace COT
{
    public class ButtonClickSound : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AudioClip clickSound;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickSound == null) return;
            AudioManager.Instance.PlaySound(clickSound);
        }
    }
}