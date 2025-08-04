using System.Collections;
using UnityEngine;

namespace Gamegaard.Commons
{
    public abstract class DelayedButtonBehaviour : ButtonBehaviour
    {
        [SerializeField] protected bool allowConsecutive;
        [SerializeField] protected float delay;

        private Coroutine activeCoroutine;

        public override void OnClick()
        {
            if (!allowConsecutive && activeCoroutine != null)
            {
                StopCoroutine(WaitTime());
            }
            activeCoroutine = StartCoroutine(WaitTime());
        }

        private IEnumerator WaitTime()
        {
            yield return new WaitForSeconds(delay);
            OnDelayCompleted();
        }

        protected abstract void OnDelayCompleted();
    }
}