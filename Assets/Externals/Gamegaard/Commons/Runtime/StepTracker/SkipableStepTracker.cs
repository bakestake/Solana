using Gamegaard;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specialized step tracker that allows skipping specific step indexes during progression.
/// This ensures that certain steps are never selected when moving forward or backward.
/// </summary>
public class SkipableStepTracker : StepTracker
{
    [SerializeField] private List<int> skippedIndexes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SkipableStepTracker"/> class.
    /// </summary>
    /// <param name="stepAmount">The total number of steps available.</param>
    /// <param name="isLoopAllowed">Indicates whether looping is enabled when reaching the last step.</param>
    /// <param name="initialStep">The starting index of the step tracker.</param>
    public SkipableStepTracker(int stepAmount, bool isLoopAllowed, int initialStep = 0)
        : base(stepAmount, isLoopAllowed, initialStep)
    {
    }

    /// <summary>
    /// Adds an index to the list of skipped indexes, preventing it from being selected.
    /// </summary>
    /// <param name="index">The index to skip.</param>
    public void AddSkippedIndex(int index)
    {
        if (index >= 0 && index < totalSteps && !skippedIndexes.Contains(index))
        {
            skippedIndexes.Add(index);
        }
    }

    /// <summary>
    /// Removes an index from the list of skipped indexes, allowing it to be selected again.
    /// </summary>
    /// <param name="index">The index to remove from the skipped list.</param>
    public void RemoveSkippedIndex(int index)
    {
        skippedIndexes.Remove(index);
    }

    /// <summary>
    /// Gets the next valid step index while skipping any defined skipped indexes.
    /// </summary>
    /// <param name="steps">The number of steps to move forward or backward.</param>
    /// <returns>The next valid step index that is not skipped.</returns>
    public int GetFurtherStep(int steps)
    {
        if (totalSteps <= 1 || steps == 0)
            return stepIndex;

        int targetStep = stepIndex;

        for (int i = 0; i < Mathf.Abs(steps); i++)
        {
            targetStep = steps > 0
                ? (targetStep + 1) % totalSteps
                : (targetStep - 1 + totalSteps) % totalSteps;

            while (skippedIndexes.Contains(targetStep))
            {
                targetStep = steps > 0
                    ? (targetStep + 1) % totalSteps
                    : (targetStep - 1 + totalSteps) % totalSteps;
            }
        }
        return targetStep;
    }

    /// <summary>
    /// Moves a specific number of steps forward, skipping any defined skipped indexes.
    /// </summary>
    /// <param name="steps">The number of steps to move forward.</param>
    /// <returns>True if a loop occurred, otherwise false.</returns>
    public override bool MoveStepForward(int steps)
    {
        if (totalSteps <= 1 || steps == 0)
            return false;

        int targetStep = GetFurtherStep(steps);
        bool looped = SetIndexAndCheckLoop(targetStep, steps);

        return looped;
    }

    /// <summary>
    /// Moves a specific number of steps backward, skipping any defined skipped indexes.
    /// </summary>
    /// <param name="steps">The number of steps to move backward.</param>
    /// <returns>True if a loop occurred, otherwise false.</returns>
    public bool MoveStepBackwardWithSkip(int steps)
    {
        if (totalSteps <= 1 || steps == 0)
            return false;

        int targetStep = GetFurtherStep(-steps);
        bool looped = SetIndexAndCheckLoop(targetStep, -steps);

        return looped;
    }

    /// <summary>
    /// Randomizes the current step index while ensuring the result is not in the skipped indexes.
    /// </summary>
    public new void Randomize()
    {
        if (skippedIndexes.Count >= totalSteps)
        {
            Debug.LogWarning("All steps are skipped. Randomization is not possible.");
            return;
        }

        int step;
        do
        {
            step = Random.Range(0, LastStep + 1);
        } while (skippedIndexes.Contains(step));

        SetIndex(step);
    }

    /// <summary>
    /// Moves to the next valid step, skipping any defined skipped indexes.
    /// </summary>
    /// <returns>True if a loop occurred, otherwise false.</returns>
    public override bool NextStep()
    {
        return MoveStepForward(1);
    }

    /// <summary>
    /// Moves to the previous valid step, skipping any defined skipped indexes.
    /// </summary>
    /// <returns>True if a loop occurred, otherwise false.</returns>
    public override bool PreviousStep()
    {
        return MoveStepBackwardWithSkip(1);
    }

    /// <summary>
    /// Sets the current step index while ensuring skipped indexes are not selected.
    /// If the given index is in the skipped list, it advances to the next valid step.
    /// </summary>
    /// <param name="index">The step index to set.</param>
    protected override void SetIndex(int index)
    {
        while (skippedIndexes.Contains(index))
        {
            index = (index + 1) % totalSteps;
        }
        base.SetIndex(index);
    }

    /// <summary>
    /// Sets the step index while checking for loops and triggering the loop event if needed.
    /// </summary>
    /// <param name="newIndex">The new step index to set.</param>
    /// <param name="stepDirection">The direction of movement (positive for forward, negative for backward).</param>
    /// <returns>True if a loop occurred, otherwise false.</returns>
    private bool SetIndexAndCheckLoop(int newIndex, int stepDirection)
    {
        int loopCount = Mathf.Abs(newIndex - stepIndex) / totalSteps;

        SetIndex(newIndex);

        if (loopCount > 0)
        {
            TriggerLoopEvent(stepDirection > 0 ? loopCount : -loopCount);
            return true;
        }

        return false;
    }
}
