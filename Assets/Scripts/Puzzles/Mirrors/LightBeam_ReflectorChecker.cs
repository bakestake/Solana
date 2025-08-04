using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LightBeam_ReflectorChecker : MonoBehaviour
{
    [SerializeField] private LightBeam_Reflector _reflector;
    [SerializeField] protected Direction8[] validDirections;
    [SerializeField] protected UnityEvent OnEnterCorrectDirection;
    [SerializeField] protected UnityEvent OnExitCorrectDirection;

    private bool isCorrect;

    private void OnEnable()
    {
        _reflector.OnReflectionStateChanged += OnStateChanged;
        _reflector.OnRotate += OnRotated;
    }

    private void OnDisable()
    {
        _reflector.OnReflectionStateChanged -= OnStateChanged;
        _reflector.OnRotate -= OnRotated;
    }

    private void OnRotated(Direction8 direction)
    {
        bool isValid = validDirections.Contains(direction) && _reflector.IsReflecting;
        CheckState(isValid);
    }

    private void OnStateChanged(bool isReflecting)
    {
        Direction8 direction = _reflector.CurrentDirection;
        bool isValid = validDirections.Contains(direction) && isReflecting;
        CheckState(isValid);
    }

    private void CheckState(bool isValid)
    {
        if (isValid && !isCorrect)
        {
            isCorrect = true;
            OnEnterCorrectDirection?.Invoke();
        }
        else if (!isValid && isCorrect)
        {
            isCorrect = false;
            OnExitCorrectDirection?.Invoke();
        }
    }
}
