using UnityEngine;

namespace Gamegaard.CustomValues
{
    [System.Serializable]
    public class WeightValue<T> : IWeightValueResult<T>
    {
        [SerializeField] private T obj;

        [field: Min(0.0001f), SerializeField]
        public float Weight { get; set; } = 1;

        public WeightValue()
        {
            Weight = 1;
        }

        public WeightValue(T obj, float weight = 1)
        {
            this.obj = obj;
            Weight = weight;
        }

        public T GetValue()
        {
            return obj;
        }
    }
}