using Gamegaard.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public abstract class TimedAttributeBarUi : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float decreaseSpeed = 1;
        [SerializeField] private float decreaseWaitingTime = 1;

        [Header("References")]
        [SerializeField] private Image attrFillBar;
        [SerializeField] private Image attrDecreaseBar;
        [SerializeField] private TextMeshProUGUI attrText;

        private BasicTimer decreaseTime;

        protected virtual void Awake()
        {
            decreaseTime = new BasicTimer(decreaseWaitingTime);
        }

        private void Update()
        {
            UpdateAttrDecreaseBar();
        }

        /// <summary>
        /// Atualiza a barra de atributo.
        /// </summary>
        protected void UpdateAttrDecreaseBar()
        {
            decreaseTime.UpdateTimer();
            if (decreaseTime.IsCompleted && attrFillBar.fillAmount < attrDecreaseBar.fillAmount)
                attrDecreaseBar.fillAmount -= decreaseSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Atualiza a barra de atributo.
        /// </summary>
        protected void UpdateAttrBar(IBarUser attr)
        {
            attrText.SetText($"{attr.ActualValue.ToString("00")}/{attr.MaxValue.ToString("00")}");
            float lastFillAmount = attrFillBar.fillAmount;
            attrFillBar.fillAmount = attr.Percentage;

            if (attrFillBar.fillAmount > attrDecreaseBar.fillAmount)
                attrDecreaseBar.fillAmount = attrFillBar.fillAmount;
            else if (attr.Percentage < lastFillAmount)
                decreaseTime.Reset();
        }
    }
}
