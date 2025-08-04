using UnityEngine;
using UnityEngine.Events;

public class PushButtonPuzzleCenter : MonoBehaviour
{
    [SerializeField] private PushButtonPuzzle[] buttons;
    [SerializeField] private Health[] enemies;
    [SerializeField] private UnityEvent OnCompleted;
    [SerializeField] private UnityEvent OnWrongButton;
    [SerializeField] private UnityEvent OnCorretButton;

    private int currentIndex;
    private PushButtonPuzzle lastWrongButton;

    public void RegisterButton(PushButtonPuzzle button)
    {
        if (buttons[currentIndex] == button)
        {
            currentIndex++;
            OnCorretButton?.Invoke();

            if (lastWrongButton != null)
            {
                lastWrongButton.SetToggle(false);
                lastWrongButton = null;
            }

            if (currentIndex >= buttons.Length)
            {
                foreach (Health enemy in enemies)
                {
                    if (enemy == null) continue;
                    enemy.Die();
                }

                OnCompleted?.Invoke();
                currentIndex = 0;
            }
        }
        else
        {
            currentIndex = 0;
            OnWrongButton?.Invoke();
            lastWrongButton = button;
            foreach (PushButtonPuzzle pushButton in buttons)
            {
                if (button == pushButton) continue;
                pushButton.SetToggle(false);
            }
        }
    }
}