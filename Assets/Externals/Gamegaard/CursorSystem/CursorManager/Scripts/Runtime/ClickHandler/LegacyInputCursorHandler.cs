using System;
using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public class LegacyInputCursorHandler : ICursorInputHandler
    {
        private readonly Action<bool> onClick;

        private bool lastState = false;

        public LegacyInputCursorHandler(Action<bool> onClick)
        {
            this.onClick = onClick;
        }

        public void Enable() { }
        public void Disable() { }
        public void Update()
        {
            bool newState = Input.GetMouseButton(0);
            if (lastState != newState)
            {
                lastState = newState;
                onClick(Input.GetMouseButton(0));
            }
        }
    }
}