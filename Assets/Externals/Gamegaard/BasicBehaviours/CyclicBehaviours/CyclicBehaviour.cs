using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public class CyclicBehaviour : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] protected CyclicBehaviourData data;

        [Header("Settings")]
        [SerializeField] private float offset;
        [SerializeField] private float frequency = 1;
        [SerializeField] private float amplitude = 1;
        [SerializeField] private bool isUnscaled;

        private float elapsedTime;
        protected float EvaluatedValue { get; private set; }

        protected virtual void Awake()
        {
            elapsedTime = offset;
        }

        protected virtual void Update()
        {
            elapsedTime += isUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            EvaluatedValue = data.Evaluate(elapsedTime, frequency, amplitude);
        }
    }
}