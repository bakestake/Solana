using UnityEngine;
using System.Collections.Generic;

namespace Gamegaard.RadialMenu
{
    public class RadialMenu : MonoBehaviour
    {
        [Header("Menu Attributes")]
        [Min(0)]
        [SerializeField] private float radius = 300f;

        [Header("References")]
        [SerializeField] private RadialMenuButton buttonPrefab;

        private List<RadialMenuButton> buttons = new List<RadialMenuButton>();
        private Transform selectedObject;
        private bool isActive;
        private int optionCount;

        // Transform of Trigger Object, List of buttons.
        public static System.Action<Transform, List<RadialMenuButtonInfo>> OnRadialMenuTrigger;
        public static System.Action OnCloseRadialMenu;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void Awake()
        {
            OnRadialMenuTrigger += Trigger;
            OnCloseRadialMenu += Close;
        }

        private void OnDestroy()
        {
            OnRadialMenuTrigger -= Trigger;
            OnCloseRadialMenu -= Close;
        }

        private void Trigger(Transform caller, List<RadialMenuButtonInfo> radialButtons)
        {
            if (!isActive)
            {
                Open(caller, radialButtons);
            }
            else
            {
                Close();

                if (selectedObject != caller)
                {
                    Open(caller, radialButtons);
                }
            }

            selectedObject = caller;
        }

        private void Open(Transform caller, List<RadialMenuButtonInfo> radialButtons)
        {
            optionCount = radialButtons.Count;
            if (optionCount == 0) return;
            transform.position = caller.position;
            isActive = true;
            for (int i = 0; i < radialButtons.Count; i++)
            {
                RadialMenuButtonInfo newButtonInfo = radialButtons[i];
                AddButton(caller, newButtonInfo, i);
            }
        }

        public void Close()
        {
            isActive = false;
            for (int i = 0; i < buttons.Count; i++) 
            {
                buttons[i].OnClose();
            }

            buttons.Clear();
        }

        private void AddButton(Transform caller, RadialMenuButtonInfo buttonInfo, int index)
        {
            float angle = index * (360f / optionCount);

            float x = caller.position.x + radius * Mathf.Cos(Mathf.Deg2Rad * angle);
            float y = caller.position.y + radius * Mathf.Sin(Mathf.Deg2Rad * angle);
            Vector3 position = new Vector3(x, y, caller.position.z);

            RadialMenuButton entry = Instantiate(buttonPrefab, position, Quaternion.identity, transform);
            entry.InitializeValues(buttons.Count, buttonInfo.Icon, buttonInfo.callback, buttonInfo.LabelText);
            buttons.Add(entry);
        }
    }
}
