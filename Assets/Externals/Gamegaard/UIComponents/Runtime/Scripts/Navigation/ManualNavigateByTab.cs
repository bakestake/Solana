using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gamegaard.Utils;

public class ManualNavigateByTab : MonoBehaviour
{
#if UNITY_STANDALONE
    [SerializeField] private Selectable[] selectableOrder;
    private EventSystem system;
    private int _index;

    public int Index
    {
        get => _index;
        set
        {
            int maxIndex = selectableOrder.Length - 1;
            if (value > maxIndex)
            {
                value = 0;
            }
            else if (value < 0)
            {
                value = maxIndex;
            }
            _index = value;
        }
    }

    private void Awake()
    {
        system = EventSystem.current;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (system.currentSelectedGameObject != null && system.currentSelectedGameObject.TryGetComponent(out Selectable actual))
            {
                if (selectableOrder.FindIndex(actual, out int index) && index != _index)
                {
                    _index = index;
                }

                bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                if (isShiftPressed)
                {
                    GoPrevious();
                }
                else
                {
                    GoNext();
                }
            }
            else
            {
                SetSelected(0);
            }
        }
    }

    private void SetSelected(int nexIndex)
    {
        Index = nexIndex;
        Selectable next = selectableOrder[_index];
        if (next != null)
        {
            if (next.TryGetComponent(out TMP_InputField field))
            {
                field.OnPointerClick(new PointerEventData(system));
            }
            system.SetSelectedGameObject(next.gameObject);
        }
        else
        {
            next = Selectable.allSelectablesArray[0];
            system.SetSelectedGameObject(next.gameObject);
        }
    }

    private void GoPrevious()
    {
        SetSelected(_index - 1);
    }

    private void GoNext()
    {
        SetSelected(_index + 1);
    }
#endif
}