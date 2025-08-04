using UnityEngine;

namespace Gamegaard
{
    public class OrdenedActions : MonoBehaviour
    {
        [SerializeField] private bool isFirstCalledOnStart;
        [SerializeField] private DelayedActionValue[] actions;

        private StepTracker loopBehaviour;
        private DelayedActionValue currentValue;

        private void Awake()
        {
            loopBehaviour = new StepTracker(actions.Length, true);
        }

        private void Start()
        {
            foreach (DelayedActionValue action in actions)
            {
                action.Initialize();
                action.SubscribeListner(OnCurrentCompleted);
            }

            SetToCurrentStep();

            if (isFirstCalledOnStart)
            {
                currentValue.ForceCall();
            }
        }

        private void OnDestroy()
        {
            currentValue?.OnActionEnd();
            foreach (DelayedActionValue action in actions)
            {
                action.UnsubscribeListner(OnCurrentCompleted);
            }
        }

        private void Update()
        {
            currentValue.Update();
        }

        private void OnCurrentCompleted()
        {
            loopBehaviour.NextStep();
            SetToCurrentStep();
        }

        private void SetToCurrentStep()
        {
            currentValue?.OnActionEnd();
            currentValue = actions[loopBehaviour.CurrentStepIndex];
            currentValue.OnActionStart();
        }
    }
}