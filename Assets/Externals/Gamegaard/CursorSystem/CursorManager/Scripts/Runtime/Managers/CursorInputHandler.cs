using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gamegaard.CursorSystem
{
    [Serializable]
    public class CursorInputHandler
    {
        [SerializeField] private InputModeData inputModeData;
        private ICursorInputHandler inputHandler;

        public event Action<bool> OnClick;

        public InputModeData InputModeData => inputModeData;

        public void Initialize()
        {
            ConfigureInputHandler();
        }

        public void Enable()
        {
            inputHandler?.Enable();
        }

        public void Disable()
        {
            inputHandler?.Disable();
        }

        public void Update()
        {
            inputHandler?.Update();
        }

        public void SetInputMode(InputMode newInputMode, InputActionProperty newInputActionReference)
        {
            inputModeData.InputMode = newInputMode;
            inputModeData.InputActionReference = newInputActionReference;
            ConfigureInputHandler();
        }

        private void ConfigureInputHandler()
        {
            switch (inputModeData.InputMode)
            {
                case InputMode.LegacyInputManager:
                    inputHandler = new LegacyInputCursorHandler(HandleClick);
                    break;

                case InputMode.NewInputSystem:
                    inputHandler = new NewInputCursorHandler(inputModeData.InputActionReference, HandleClick);
                    break;

                default:
                    inputHandler = null;
                    break;
            }
        }

        private void HandleClick(bool isClicking)
        {
            OnClick?.Invoke(isClicking);
        }

        internal void TryAssignDefaultLeftClickAction()
        {
#if ENABLE_INPUT_SYSTEM
            InputActionAsset[] assets = Resources.FindObjectsOfTypeAll<InputActionAsset>();
            foreach (InputActionAsset asset in assets)
            {
                foreach (InputActionMap actionMap in asset.actionMaps)
                {
                    foreach (InputAction action in actionMap.actions)
                    {
                        if (action.bindings.Any(b => b.path != null && b.path.Contains("<Mouse>/leftButton")))
                        {
                            inputModeData.InputActionReference = new InputActionProperty(InputActionReference.Create(action));
                            return;
                        }
                    }
                }
            }
#endif
        }
    }
}