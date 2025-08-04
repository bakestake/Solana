using UnityEngine;

namespace Gamegaard
{
    //TODO: Melhorar performance geral (SetMaxLife e SetValue)
    public class ContainerBarUI : SegmentBarBaseUI<ValueContainerUI>
    {
        [Min(1)]
        [SerializeField] private float segmentValue = 20;

        private float SegmentAmount => maxValue / segmentValue;

        private void Start()
        {
            Initialize(150, 75);
        }

        public void Initialize(float maxValue, float currentValue)
        {
            SetMaxValue(maxValue);
            SetValue(currentValue);
        }

        public void SetMaxValue(float value)
        {
            maxValue = value;

            int heartAmount = Mathf.CeilToInt(SegmentAmount);
            int overAmount = SegmentCount - heartAmount;

            if (overAmount > 0)
            {
                DestroySegments(overAmount);
            }
            else
            {
                CreateSegments(Mathf.Abs(overAmount));
            }
        }

        public override void SetValue(float value)
        {
            this.value = value;
            float percentage = value / maxValue;
            int target = Mathf.FloorToInt(SegmentAmount * percentage);
            float heartLife = value % segmentValue;
            float heartMaxLife = maxValue % segmentValue;

            for (int i = 0; i < SegmentCount; i++)
            {
                ValueContainerUI heart = segments[i];

                float heartValue;
                if (i == target)
                {
                    heartValue = QuarterValue(heartLife);
                }
                else
                {
                    heartValue = i < target ? 1 : 0;
                }

                float maxHeartValue = 0;
                if (i == SegmentCount - 1 && heartMaxLife != 0)
                {
                    maxHeartValue = QuarterValue(segmentValue - heartMaxLife);
                }

                heart.SetValue(heartValue, maxHeartValue);
            }
        }

        float QuarterValue(float value)
        {
            return 0.25f * Mathf.Round(value / 0.25f) / segmentValue;
        }
    }
}