using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.RuntimeDebug
{
    [Serializable]
    public class UnityEventLogCommand : LogCommandBase
    {
        [SerializeField] private UnityEvent Callback;

        public UnityEventLogCommand() : base() { }

        public UnityEventLogCommand(string commandID, string commandFormat, UnityEvent callback, string commandDescription = "", string commandTooltip = "", string commandCallback = "") : base(commandID, commandFormat, new Action(callback.Invoke), commandDescription, commandTooltip, commandCallback)
        {
            Callback = callback;
        }

        public void Invoke()
        {
            Callback.Invoke();
        }

        public override void TryInvoke(string message)
        {
            Invoke();
        }
    }
}