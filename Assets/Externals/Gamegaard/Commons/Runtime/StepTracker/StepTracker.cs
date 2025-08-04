using UnityEngine;

namespace Gamegaard
{
    /// <summary>
    /// Represents an object that tracks a series of steps, supporting both looped and linear progression.
    /// It allows moving forward and backward through steps, with optional looping behavior.
    /// </summary>
    [System.Serializable]
    public class StepTracker
    {
        [SerializeField] protected bool isLoopAllowed;

        protected int totalSteps;
        protected int stepIndex;

        /// <summary>
        /// Gets the total number of steps in the tracker.
        /// </summary>
        public int TotalSteps => totalSteps;

        /// <summary>
        /// Gets the current step index.
        /// </summary>
        public int CurrentStepIndex => stepIndex;

        /// <summary>
        /// Gets or sets whether looping is allowed when reaching the last step.
        /// </summary>
        public bool IsLoopAllowed
        {
            get => isLoopAllowed;
            set => isLoopAllowed = value;
        }

        /// <summary>
        /// Gets the index of the last valid step in the sequence.
        /// </summary>
        public int LastStep => Mathf.Max(0, totalSteps - 1);

        /// <summary>
        /// Event triggered when the current step index changes.
        /// The event passes the new step index as a parameter.
        /// </summary>
        public event System.Action<int> OnStepChanged;

        /// <summary>
        /// Event triggered when a loop occurs (either forward or backward).
        /// The event passes an integer indicating the number of times the loop was completed.
        /// A positive value means looping forward, and a negative value means looping backward.
        /// </summary>
        public event System.Action<int> OnLooped;

        /// <summary>
        /// Initializes a new instance of the <see cref="StepTracker"/> class.
        /// </summary>
        /// <param name="stepAmount">The total number of steps available.</param>
        /// <param name="isLoopAllowed">Indicates whether looping is enabled when reaching the last step.</param>
        /// <param name="initialStep">The starting index of the step tracker.</param>
        public StepTracker(int stepAmount, bool isLoopAllowed, int initialStep = 0)
        {
            this.isLoopAllowed = isLoopAllowed;
            this.totalSteps = Mathf.Max(stepAmount, 1);
            SetStep(initialStep);
        }

        /// <summary>
        /// Updates the total number of steps.
        /// If the new amount is smaller than the current step index, the step index is adjusted.
        /// </summary>
        /// <param name="value">The new total number of steps.</param>
        public void SetAmount(int value)
        {
            totalSteps = Mathf.Max(value, 1);
            if (stepIndex > value)
            {
                SetIndex(value);
            }
        }

        /// <summary>
        /// Moves forward by one step.
        /// If looping is enabled and the last step is reached, it loops back to the first step.
        /// </summary>
        /// <returns>True if a loop occurred, otherwise false.</returns>
        public virtual bool NextStep()
        {
            if (totalSteps <= 1 || (!isLoopAllowed && stepIndex >= LastStep))
                return false;

            int newIndex = (stepIndex + 1) % totalSteps;
            bool looped = newIndex == 0 && isLoopAllowed;

            SetIndex(newIndex);

            if (looped)
            {
                TriggerLoopEvent(1);
            }
            return looped;
        }

        /// <summary>
        /// Moves backward by one step.
        /// If looping is enabled and the first step is reached, it loops back to the last step.
        /// </summary>
        /// <returns>True if a loop occurred, otherwise false.</returns>
        public virtual bool PreviousStep()
        {
            if (totalSteps <= 1 || (!isLoopAllowed && stepIndex <= 0))
                return false;

            int newIndex = (stepIndex - 1 + totalSteps) % totalSteps;
            bool looped = newIndex == LastStep && isLoopAllowed;

            SetIndex(newIndex);

            if (looped)
            {
                TriggerLoopEvent(-1);
            }
            return looped;
        }

        /// <summary>
        /// Moves a specific number of steps forward.
        /// If looping is enabled, it calculates how many loops occurred and triggers the event accordingly.
        /// </summary>
        /// <param name="steps">The number of steps to move forward.</param>
        /// <returns>True if at least one loop occurred, otherwise false.</returns>
        public virtual bool MoveStepForward(int steps)
        {
            if (totalSteps <= 1 || steps == 0) return false;

            if (!isLoopAllowed)
            {
                int finalTargetStep = Mathf.Clamp(stepIndex + steps, 0, LastStep);
                SetIndex(finalTargetStep);
                return false;
            }

            int totalLoops = (stepIndex + steps) / totalSteps;
            int targetStep = (stepIndex + steps) % totalSteps;

            SetIndex(targetStep);

            if (totalLoops > 0)
            {
                TriggerLoopEvent(totalLoops);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves a specific number of steps backward.
        /// If looping is enabled, it calculates how many loops occurred and triggers the event accordingly.
        /// </summary>
        /// <param name="steps">The number of steps to move backward.</param>
        /// <returns>True if at least one loop occurred, otherwise false.</returns>
        public bool MoveStepBackward(int steps)
        {
            if (totalSteps <= 1 || steps == 0) return false;

            if (!isLoopAllowed)
            {
                int finalTargetStep = Mathf.Clamp(stepIndex - steps, 0, LastStep);
                SetIndex(finalTargetStep);
                return false;
            }

            int totalLoops = Mathf.Abs(stepIndex - steps) / totalSteps;
            int targetStep = (stepIndex - steps) % totalSteps;

            if (targetStep < 0) targetStep += totalSteps;

            SetIndex(targetStep);

            if (totalLoops > 0)
            {
                TriggerLoopEvent(-totalLoops);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the current step to a specific target step.
        /// </summary>
        /// <param name="targetStep">The target step index to set.</param>
        public virtual void SetStep(int targetStep)
        {
            if (totalSteps <= 1 || targetStep < 0 || targetStep >= totalSteps)
                return;

            SetIndex(targetStep);
        }

        /// <summary>
        /// Sets the current step index and triggers the <see cref="OnStepChanged"/> event.
        /// </summary>
        /// <param name="index">The new step index.</param>
        protected virtual void SetIndex(int index)
        {
            stepIndex = index;
            TriggerStepChanged(index);
        }

        /// <summary>
        /// Resets the current step to the first step in the sequence.
        /// </summary>
        public void SetToFirstStep()
        {
            SetStep(0);
        }

        /// <summary>
        /// Sets the current step to the last step in the sequence.
        /// </summary>
        public void SetToLastStep()
        {
            SetStep(LastStep);
        }

        /// <summary>
        /// Randomizes the current step index.
        /// </summary>
        public void Randomize()
        {
            int step = Random.Range(0, LastStep);
            SetIndex(step);
        }

        /// <summary>
        /// Checks if the current step is the first step in the sequence.
        /// </summary>
        /// <returns>True if the current step is the first step; otherwise, false.</returns>
        public bool IsFirstStep()
        {
            return stepIndex == 0;
        }

        /// <summary>
        /// Checks if the current step is the last step in the sequence.
        /// </summary>
        /// <returns>True if the current step is the last step; otherwise, false.</returns>
        public bool IsLastStep()
        {
            return stepIndex == LastStep;
        }

        /// <summary>
        /// Clears all steps, resets the tracker, and sets the step amount to 1.
        /// </summary>
        public void ClearSteps()
        {
            totalSteps = 1;
            SetStep(0);
        }

        protected void TriggerStepChanged(int currentStep)
        {
            OnStepChanged?.Invoke(currentStep);
        }

        protected void TriggerLoopEvent(int loopDirectionCount)
        {
            OnLooped?.Invoke(loopDirectionCount);
        }
    }
}
