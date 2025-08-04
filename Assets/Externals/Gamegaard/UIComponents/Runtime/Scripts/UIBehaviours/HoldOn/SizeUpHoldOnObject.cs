using UnityEngine;

namespace Game.UI.InLevelUpgrades
{
    public class SizeUpHoldOnObject : TimedHoldOnObject
    {
        [SerializeField] protected AnimationCurve sizeCurve;
        [SerializeField] private float targetScale = 1.5f;
        private Vector3 originalSize;

        protected override void Awake()
        {
            base.Awake();
            originalSize = transform.localScale;
        }

        protected override void Update()
        {
            base.Update();
            transform.localScale = originalSize + sizeCurve.Evaluate(Percentage) * targetScale * originalSize;
        }
    }
}