using System;
using UnityEngine.InputSystem;

namespace Gamegaard.CursorSystem
{
    public class NewInputCursorHandler : ICursorInputHandler
    {
        private readonly InputAction inputAction;
        private readonly Action<bool> onClick;

        public NewInputCursorHandler(InputActionProperty actionReference, Action<bool> onClick)
        {
            inputAction = actionReference.action;
            this.onClick = onClick;
        }

        public void Enable()
        {
            inputAction.performed += HandleInput;
            inputAction.canceled += HandleInput;
        }

        public void Disable()
        {
            inputAction.performed -= HandleInput;
            inputAction.canceled -= HandleInput;
        }

        private void HandleInput(InputAction.CallbackContext context)
        {
            onClick(context.ReadValueAsButton());
        }

        public void Update() { }
    }
}