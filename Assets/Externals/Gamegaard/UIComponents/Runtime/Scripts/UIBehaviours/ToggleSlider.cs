using Gamegaard;
using UnityEngine;
using UnityEngine.UI;

namespace Rucoy
{
    public class ToggleSlider : MonoBehaviour
    {
        [SerializeField] private StepTracker stepTracker;
        [SerializeField] private Toggle[] toggles;

        private void Awake()
        {
            stepTracker = new StepTracker(toggles.Length, true);
            OnStepChanged(stepTracker.TotalSteps - 1);

            for (int i = 0; i < toggles.Length; i++)
            {
                int index = i;
                Toggle toggle = toggles[i];
                toggle.onValueChanged.AddListener(x => OnToggleChanged(x, index));
            }
        }

        private void OnEnable()
        {
            stepTracker.OnStepChanged += OnStepChanged;
        }

        private void OnDisable()
        {
            stepTracker.OnStepChanged -= OnStepChanged;
        }

        public void Next()
        {
            stepTracker.NextStep();
        }

        public void Previous()
        {
            stepTracker.PreviousStep();
        }

        public void OnToggleChanged(bool value, int i)
        {
            if (!value) return;
            stepTracker.SetStep(i);
        }

        private void OnStepChanged(int index)
        {
            toggles[index].SetIsOnWithoutNotify(true);
        }
    }
}