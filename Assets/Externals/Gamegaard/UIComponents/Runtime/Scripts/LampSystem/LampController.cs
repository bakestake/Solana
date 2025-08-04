using UnityEngine;

namespace Gamegaard
{
    public class LampController : MonoBehaviour
    {
        [SerializeField] private Color disabledColor = Color.gray;
        [SerializeField] private ImageLamp[] lamps;
        [SerializeField] private LampsBehaviour currentBehaviour;

        private Coroutine currentPatternCoroutine;
        public int CurrentCycle { get; private set; }

        private void Start()
        {
            foreach (ILamp lamp in lamps)
            {
                lamp.SetDeactiveColor(disabledColor);
                lamp.SetActiveColor(currentBehaviour.color);
                lamp.Deactivate();
            }

            if (currentBehaviour.pattern != null)
            {
                PlayBehaviour(currentBehaviour);
            }
        }

        public void SetBehaviour(LampsBehaviour behaviour, bool isCleared = true)
        {
            if (isCleared && currentPatternCoroutine != null)
            {
                CurrentCycle = 0;
                StopCoroutine(currentPatternCoroutine);

                foreach (ILamp lamp in lamps)
                {
                    lamp.SetActiveColor(behaviour.color);
                    lamp.Deactivate();
                }
            }
            currentBehaviour= behaviour;
            PlayBehaviour(currentBehaviour);
        }

        private void PlayBehaviour(LampsBehaviour behaviour)
        {
            currentPatternCoroutine = StartCoroutine(behaviour.pattern.ExecutePattern(lamps, behaviour.MaxCycles));
        }

        public void SetDisabledColor(Color color)
        {
            disabledColor = color;
        }
    }
}