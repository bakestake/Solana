using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gamegaard
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected float timeToShow;
        [SerializeField] protected string title;
        [TextArea(5, 10)]
        [SerializeField] protected string description;
        protected WaitForSeconds waitForSeconds;

        private void Awake()
        {
            if (timeToShow > 0)
            {
                waitForSeconds = new WaitForSeconds(timeToShow);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (timeToShow > 0)
            {
                StartCoroutine(nameof(WaitTimer));
            }
            else
            {
                TooltipManager.Instance.CallTooltip(title, description);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.Instance.EndTooltip();
            StopCoroutine(nameof(WaitTimer));
        }

        public void SetText(string title, string description)
        {
            this.title = title;
            this.description = description;
        }

        public void CloseToolTip()
        {
            TooltipManager.Instance.EndTooltip();
        }

        IEnumerator WaitTimer()
        {
            yield return waitForSeconds;
            TooltipManager.Instance.CallTooltip(title, description);
        }
    }
}
