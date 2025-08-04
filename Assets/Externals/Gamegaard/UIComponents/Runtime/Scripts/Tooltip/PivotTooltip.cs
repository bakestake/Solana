using UnityEngine;

namespace Gamegaard
{
    public class PivotTooltip : Tooltip
    {
        [SerializeField] protected Vector2 rightDownPivot;
        [SerializeField] protected Vector2 rightUpPivot;
        [SerializeField] protected Vector2 leftDownPivot;
        [SerializeField] protected Vector2 leftUpPivot;
        private float screenPosition = 0.25f;

        /// <summary>
        /// Sets the current tooltip position based on pivot.
        /// </summary>
        protected override void SetPosition()
        {
            Vector2 pivot = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            Vector2 finalPivot = rightUpPivot;

            if (pivot.x < screenPosition && pivot.y > screenPosition)
            {
                finalPivot = leftUpPivot;
            }
            else if (pivot.x < screenPosition && pivot.y < screenPosition)
            {
                finalPivot = leftDownPivot;
            }
            else if (pivot.x > screenPosition && pivot.y < screenPosition)
            {
                finalPivot = rightDownPivot;
            }

            stretchBackground.pivot = finalPivot;

            base.SetPosition();
        }
    }
}