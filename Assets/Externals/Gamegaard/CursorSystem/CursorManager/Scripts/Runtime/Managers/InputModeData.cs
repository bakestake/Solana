using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gamegaard.CursorSystem
{
    [Serializable]
    public class InputModeData
    {
        [SerializeField] private InputMode inputMode;
        [SerializeField] private InputActionProperty inputActionReference;

        public InputMode InputMode { get => inputMode; set => inputMode = value; }
        public InputActionProperty InputActionReference { get => inputActionReference; set => inputActionReference = value; }
    }
}