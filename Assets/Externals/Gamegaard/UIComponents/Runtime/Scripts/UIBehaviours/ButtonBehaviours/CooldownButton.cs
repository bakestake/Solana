using Gamegaard.Timer;
using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard.Commons
{
    public class CooldownButton : ButtonBehaviour
    {
        [SerializeField] protected float cooldownTime;
        [SerializeField] protected Image cooldownBar;

        protected BasicTimer cooldownTimer;

        protected override void Awake()
        {
            base.Awake();
            cooldownTimer = new BasicTimer(cooldownTime, 0);
        }

        private void Update()
        {
            if (cooldownTimer.CheckAndUpdateTimer())
            {
                OnCooldownEnd();
            }
            cooldownBar.fillAmount = cooldownTimer.InverseProgress;
        }

        public override void OnClick()
        {
            if (!cooldownTimer.IsCompleted) return;
            targetComponent.interactable = false;
            cooldownBar.gameObject.SetActive(true);
            cooldownTimer.Reset();
        }

        private void OnCooldownEnd()
        {
            targetComponent.interactable = true;
            cooldownBar.gameObject.SetActive(false);
        }
    }
}