using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.Utilities;

namespace Gamegaard.CursorSystem
{
    public enum CursorModeFix
    {
        SoftwareCursor,
        HardwareCursorIfAvailable,
    }

    public class VirtualMouseFix : MonoBehaviour
    {
        [Header("Cursor")]
        [SerializeField] private CursorModeFix cursorMode;
        [SerializeField] private Graphic cursorGraphic;
        [SerializeField] private RectTransform cursorTransform;

        [Header("Motion")]
        [SerializeField] private float cursorSpeed = 400;
        [SerializeField] private float scrollSpeed = 45;

        [Space(10)]
        [SerializeField] private InputActionProperty stickAction;
        [SerializeField] private InputActionProperty pointerAction;
        [SerializeField] private InputActionProperty leftButtonAction;
        [SerializeField] private InputActionProperty middleButtonAction;
        [SerializeField] private InputActionProperty rightButtonAction;
        [SerializeField] private InputActionProperty forwardButtonAction;
        [SerializeField] private InputActionProperty backButtonAction;
        [SerializeField] private InputActionProperty scrollWheelAction;

        private Canvas canvas;
        private Mouse virtualMouse;
        private Mouse systemMouse;
        private Action afterInputUpdateDelegate;
        private Action<InputAction.CallbackContext> buttonActionTriggeredDelegate;
        private double lastTime;
        private Vector2 lastStickValue;

        public Mouse VirtualMouse => virtualMouse;

        public RectTransform CursorTransform
        {
            get => cursorTransform;
            set => cursorTransform = value;
        }

        public float CursorSpeed
        {
            get => cursorSpeed;
            set => cursorSpeed = value;
        }

        public CursorModeFix CursorMode
        {
            get => cursorMode;
            set
            {
                if (cursorMode == value) return;

                if (cursorMode == CursorModeFix.HardwareCursorIfAvailable && systemMouse != null)
                {
                    InputSystem.EnableDevice(systemMouse);
                    systemMouse = null;
                }

                cursorMode = value;

                if (cursorMode == CursorModeFix.HardwareCursorIfAvailable)
                {
                    TryEnableHardwareCursor();
                }
                else if (cursorGraphic != null)
                {
                    cursorGraphic.enabled = true;
                }
            }
        }

        public Graphic CursorGraphic
        {
            get => cursorGraphic;
            set
            {
                cursorGraphic = value;
                TryFindCanvas();
            }
        }

        public float ScrollSpeed
        {
            get => scrollSpeed;
            set => scrollSpeed = value;
        }

        public InputActionProperty StickAction
        {
            get => stickAction;
            set => SetAction(ref stickAction, value);
        }

        public InputActionProperty PointerAction
        {
            get => pointerAction;
            set => SetAction(ref pointerAction, value);
        }

        public InputActionProperty LeftButtonAction
        {
            get => leftButtonAction;
            set => SetActionProperty(ref leftButtonAction, value);
        }

        public InputActionProperty RightButtonAction
        {
            get => rightButtonAction;
            set => SetActionProperty(ref rightButtonAction, value);
        }

        public InputActionProperty MiddleButtonAction
        {
            get => middleButtonAction;
            set => SetActionProperty(ref middleButtonAction, value);
        }

        public InputActionProperty ForwardButtonAction
        {
            get => forwardButtonAction;
            set => SetActionProperty(ref forwardButtonAction, value);
        }

        public InputActionProperty BackButtonAction
        {
            get => backButtonAction;
            set => SetActionProperty(ref backButtonAction, value);
        }

        public InputActionProperty ScrollWheelAction
        {
            get => scrollWheelAction;
            set => SetAction(ref scrollWheelAction, value);
        }

        protected void OnEnable()
        {
            if (cursorMode == CursorModeFix.HardwareCursorIfAvailable)
            {
                TryEnableHardwareCursor();
            }

            if (virtualMouse == null)
            {
                virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            }
            else if (!virtualMouse.added)
            {
                InputSystem.AddDevice(virtualMouse);
            }

            if (cursorTransform != null)
            {
                Vector2 position = cursorTransform.anchoredPosition;
                InputState.Change(virtualMouse.position, position);
                systemMouse?.WarpCursorPosition(position);
            }

            afterInputUpdateDelegate ??= OnAfterInputUpdate;
            InputSystem.onAfterUpdate += afterInputUpdateDelegate;

            buttonActionTriggeredDelegate ??= OnButtonActionTriggered;
            SetActionCallback(leftButtonAction, buttonActionTriggeredDelegate, true);
            SetActionCallback(rightButtonAction, buttonActionTriggeredDelegate, true);
            SetActionCallback(middleButtonAction, buttonActionTriggeredDelegate, true);
            SetActionCallback(forwardButtonAction, buttonActionTriggeredDelegate, true);
            SetActionCallback(backButtonAction, buttonActionTriggeredDelegate, true);

            stickAction.action?.Enable();
            pointerAction.action?.Enable();
            leftButtonAction.action?.Enable();
            rightButtonAction.action?.Enable();
            middleButtonAction.action?.Enable();
            forwardButtonAction.action?.Enable();
            backButtonAction.action?.Enable();
            scrollWheelAction.action?.Enable();
        }

        protected void OnDisable()
        {
            if (virtualMouse != null && virtualMouse.added)
            {
                InputSystem.RemoveDevice(virtualMouse);
            }

            if (systemMouse != null)
            {
                InputSystem.EnableDevice(systemMouse);
                systemMouse = null;
            }

            if (afterInputUpdateDelegate != null)
            {
                InputSystem.onAfterUpdate -= afterInputUpdateDelegate;
            }

            stickAction.action?.Disable();
            pointerAction.action?.Disable();
            leftButtonAction.action?.Disable();
            rightButtonAction.action?.Disable();
            middleButtonAction.action?.Disable();
            forwardButtonAction.action?.Disable();
            backButtonAction.action?.Disable();
            scrollWheelAction.action?.Disable();

            if (buttonActionTriggeredDelegate != null)
            {
                SetActionCallback(leftButtonAction, buttonActionTriggeredDelegate, false);
                SetActionCallback(rightButtonAction, buttonActionTriggeredDelegate, false);
                SetActionCallback(middleButtonAction, buttonActionTriggeredDelegate, false);
                SetActionCallback(forwardButtonAction, buttonActionTriggeredDelegate, false);
                SetActionCallback(backButtonAction, buttonActionTriggeredDelegate, false);
            }

            lastTime = default;
            lastStickValue = default;
        }

        private void TryFindCanvas()
        {
            if (canvas == null)
            {
                canvas = cursorGraphic.GetComponentInParent<Canvas>();
            }
        }

        private void TryEnableHardwareCursor()
        {
            ReadOnlyArray<InputDevice> devices = InputSystem.devices;
            for (var i = 0; i < devices.Count; ++i)
            {
                InputDevice device = devices[i];
                if (device.native && device is Mouse mouse)
                {
                    systemMouse = mouse;
                    break;
                }
            }

            if (systemMouse == null)
            {
                if (cursorGraphic != null)
                {
                    cursorGraphic.enabled = true;
                }
                return;
            }

            InputSystem.DisableDevice(systemMouse);

            if (virtualMouse != null)
            {
                systemMouse.WarpCursorPosition(virtualMouse.position.value);
            }

            if (cursorGraphic != null)
            {
                cursorGraphic.enabled = false;
            }
        }

        private void UpdateMotion()
        {
            if (virtualMouse == null) return;

            if (canvas == null)
            {
                TryFindCanvas();
            }

            InputAction stickAction = this.stickAction.action;
            InputAction pointerAction = this.pointerAction.action;

            if (stickAction != null && stickAction.bindings.Count > 0)
            {
                Vector2 stickValue = stickAction.ReadValue<Vector2>();
                UpdateStickMotion(stickValue);
            }
            else if (pointerAction != null && pointerAction.bindings.Count > 0)
            {
                Vector2 pointerValue = pointerAction.ReadValue<Vector2>();
                UpdatePointerMotion(pointerValue);
            }
            else Debug.LogWarning("No Stick or Pointer bindings, not moving the cursor.");
        }

        private void OnButtonActionTriggered(InputAction.CallbackContext context)
        {
            if (virtualMouse == null)
            {
                return;
            }

            InputAction action = context.action;
            MouseButton? button = null;

            if (action == leftButtonAction.action)
                button = MouseButton.Left;
            else if (action == rightButtonAction.action)
                button = MouseButton.Right;
            else if (action == middleButtonAction.action)
                button = MouseButton.Middle;
            else if (action == forwardButtonAction.action)
                button = MouseButton.Forward;
            else if (action == backButtonAction.action)
                button = MouseButton.Back;

            if (button != null)
            {
                bool isPressed = context.control.IsPressed();
                virtualMouse.CopyState<MouseState>(out MouseState mouseState);
                mouseState.WithButton(button.Value, isPressed);

                InputState.Change(virtualMouse, mouseState);
            }
        }

        private void SetActionProperty(ref InputActionProperty inputActionProperty, InputActionProperty newValue)
        {
            if (buttonActionTriggeredDelegate != null)
            {
                SetActionCallback(leftButtonAction, buttonActionTriggeredDelegate, false);
            }

            SetAction(ref inputActionProperty, newValue);

            if (buttonActionTriggeredDelegate != null)
            {
                SetActionCallback(leftButtonAction, buttonActionTriggeredDelegate, true);
            }
        }

        private static void SetActionCallback(InputActionProperty field, Action<InputAction.CallbackContext> callback, bool install = true)
        {
            InputAction action = field.action;
            if (action == null) return;

            if (install)
            {
                action.started += callback;
                action.canceled += callback;
            }
            else
            {
                action.started -= callback;
                action.canceled -= callback;
            }
        }

        private static void SetAction(ref InputActionProperty field, InputActionProperty value)
        {
            InputActionProperty oldValue = field;
            field = value;

            if (oldValue.reference == null)
            {
                InputAction oldAction = oldValue.action;
                if (oldAction != null && oldAction.enabled)
                {
                    oldAction.Disable();
                    if (value.reference == null && value.action != null)
                    {
                        value.action.Enable();
                    }
                }
            }
        }

        private void OnAfterInputUpdate()
        {
            UpdateMotion();
        }

        #region NEW
        private void UpdateStickMotion(Vector2 stickValue)
        {
            if (Mathf.Approximately(0, stickValue.x) && Mathf.Approximately(0, stickValue.y))
            {
                lastTime = default;
                lastStickValue = default;
            }
            else
            {
                double currentTime = InputState.currentTime;
                if (Mathf.Approximately(0, lastStickValue.x) && Mathf.Approximately(0, lastStickValue.y))
                {
                    lastTime = currentTime;
                }

                float deltaTime = (float)(currentTime - lastTime);
                Vector2 delta = new Vector2(cursorSpeed * stickValue.x * deltaTime, cursorSpeed * stickValue.y * deltaTime);

                Vector2 currentPosition = virtualMouse.position.value;
                Vector2 newPosition = currentPosition + delta;

                if (canvas != null)
                {
                    Rect pixelRect = canvas.pixelRect;
                    newPosition.x = Mathf.Clamp(newPosition.x, pixelRect.xMin, pixelRect.xMax);
                    newPosition.y = Mathf.Clamp(newPosition.y, pixelRect.yMin, pixelRect.yMax);
                }

                InputState.Change(virtualMouse.position, newPosition);
                InputState.Change(virtualMouse.delta, delta);

                if (cursorTransform != null && (cursorMode == CursorModeFix.SoftwareCursor || (cursorMode == CursorModeFix.HardwareCursorIfAvailable && systemMouse == null)))
                {
                    cursorTransform.position = newPosition;
                }

                lastStickValue = stickValue;
                lastTime = currentTime;

                systemMouse?.WarpCursorPosition(newPosition);
            }

            InputAction scrollAction = scrollWheelAction.action;
            if (scrollAction != null)
            {
                Vector2 scrollValue = scrollAction.ReadValue<Vector2>();
                scrollValue.x *= scrollSpeed;
                scrollValue.y *= scrollSpeed;

                InputState.Change(virtualMouse.scroll, scrollValue);
            }
        }

        private void UpdatePointerMotion(Vector2 pointerValue)
        {
            Vector2 newPosition = pointerValue;

            if (canvas != null)
            {
                Rect pixelRect = canvas.pixelRect;
                newPosition.x = Mathf.Clamp(newPosition.x, pixelRect.xMin, pixelRect.xMax);
                newPosition.y = Mathf.Clamp(newPosition.y, pixelRect.yMin, pixelRect.yMax);
            }

            InputState.Change(virtualMouse.position, newPosition);

            if (cursorTransform != null)
            {
                cursorTransform.position = newPosition;
            }

            systemMouse?.WarpCursorPosition(newPosition);
        }
        #endregion
    }
}