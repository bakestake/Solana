using Gamegaard.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard
{
    public abstract class SegmentBarBaseUI<T> : MonoBehaviour, IFillBar where T : Component
    {
        [SerializeField] protected int segmentAmount;
        [SerializeField] protected T segmentPrefab;

        protected List<T> segments = new List<T>();
        protected int SegmentCount => segments.Count;
        protected float maxValue;
        protected float value;

        public abstract void SetValue(float value);

        protected void DestroySegments(int amount)
        {
            int targetAmount = SegmentCount - amount;

            for (int i = segments.FinalIndex(); i >= targetAmount; i--)
            {
                Destroy(segments[i].gameObject);
                segments.RemoveAt(i);
            }
        }

        protected void CreateSegments(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                T newHeart = Instantiate(segmentPrefab, transform);
                segments.Add(newHeart);
            }
        }

        protected virtual void OnSegmentCreated(T segment)
        {

        }
    }
}