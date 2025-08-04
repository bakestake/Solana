using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoNavigateByTab : MonoBehaviour
{
#if UNITY_STANDALONE
    private EventSystem system;

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
                bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                Selectable next = isShiftPressed ? GetNext(actual) : GetLast(actual);
                SetSelected(next);
            }
            else if (FindFirstSelectable(out Selectable selectable))
            {
                SetSelected(selectable);
            }
        }
    }

    private void SetSelected(Selectable next)
    {
        Debug.Log(next, next);

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

    private Selectable GetLast(Selectable actual)
    {
        Selectable next = actual.navigation.selectOnDown;
        if (next == null)
        {
            next = actual.FindSelectableOnDown();

            if (next == null)
            {
                next = actual.FindSelectableOnLeft();

                if (next == null)
                {
                    FindFirstSelectable(out next);
                }
            }
        }

        return next;
    }

    private Selectable GetNext(Selectable actual)
    {
        Selectable next = actual.navigation.selectOnUp;

        if (next == null)
        {
            next = actual.FindSelectableOnUp();

            if (next == null)
            {
                next = actual.FindSelectableOnRight();

                if (next == null)
                {
                    FindFirstSelectable(out next);
                }
            }
        }

        return next;
    }

    private bool FindFirstSelectable(out Selectable selectable)
    {
        Debug.Log("Find");

        Selectable[] selectables = Selectable.allSelectablesArray;
        if (selectables.Length > 0)
        {
            selectable = selectables[0];
            return true;
        }
        selectable = null;
        return false;
    }
#endif
}