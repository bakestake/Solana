using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard
{
    public class SegmentBarUI : SegmentBarBaseUI<Image>
    {
        [SerializeField] protected Gradient color;
        [SerializeField] protected Color emptyColor = Color.gray;

        private void Start()
        {
            CreateSegments(segmentAmount);

            for (int i = 0; i < segments.Count; i++)
            {
                Image segment = segments[i];
                segment.color = color.Evaluate(i / (float)SegmentCount);
            }
        }

        public override void SetValue(float value)
        {
            this.value = value;
            for (int i = 0; i < segments.Count; i++)
            {
                Image segment = segments[i];
                segment.color = i > value ? emptyColor : color.Evaluate(i / (float)SegmentCount);
            }
        }
    }
}