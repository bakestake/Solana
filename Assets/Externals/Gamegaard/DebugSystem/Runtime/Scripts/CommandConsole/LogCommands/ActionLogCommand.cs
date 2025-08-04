using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class ActionLogCommand : LogCommandBase
    {
        public ActionLogCommand(string commandID, string commandFormat, Action action, string commandDescription = "", string commandTooltip = "", string commandCallback = "", Dictionary<string, SuggestorTagAttribute> suggestionsByParam = null)
            : base(commandID, commandFormat, action, commandTooltip, commandDescription, commandCallback, suggestionsByParam)
        { }

        public ActionLogCommand(CommandValues commandValues, Action action) : this(commandValues.id, commandValues.format, action, commandValues.tooltip, commandValues.description, commandValues.callbackText, commandValues.suggestionsByParam)
        {

        }

        public void Invoke()
        {
            action?.DynamicInvoke();
        }

        public override void TryInvoke(string message)
        {
            Invoke();
        }
    }

    public abstract class ParameterActionLogCommand : LogCommandBase
    {
        private readonly Regex regex = new Regex(@"^!(\w+)\s+(.*)$");

        protected ParameterActionLogCommand(CommandValues commandValues, Delegate action)
            : base(commandValues.id, commandValues.format, action, commandValues.description, commandValues.tooltip, commandValues.callbackText, commandValues.suggestionsByParam)
        { }

        public void Invoke(params object[] args)
        {
            action.DynamicInvoke(args);
        }

        public override void TryInvoke(string message)
        {
            Match match = regex.Match(message);
            if (match.Success)
            {
                string commandName = match.Groups[1].Value;
                if (commandName == Id)
                {
                    try
                    {
                        var parameters = match.Groups[2].Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        object[] convertedParameters = ConvertParameters(parameters);
                        Invoke(convertedParameters);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error invoking command: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogError("Command not found.");
                }
            }
            else
            {
                Debug.LogError("Invalid command format.");
            }
        }

        protected abstract object[] ConvertParameters(string[] stringParameters);
    }

    public class ActionLogCommand<T1> : ParameterActionLogCommand
    {
        public ActionLogCommand(CommandValues commandValues, Action<T1> action) : base(commandValues, action) { }

        protected override object[] ConvertParameters(string[] stringParameters)
        {
            T1 param1 = GetValue<T1>(stringParameters[0]);
            return new object[] { param1 };
        }
    }

    public class ActionLogCommand<T1, T2> : ParameterActionLogCommand
    {
        public ActionLogCommand(CommandValues commandValues, Action<T1, T2> action) : base(commandValues, action) { }

        protected override object[] ConvertParameters(string[] stringParameters)
        {
            T1 param1 = GetValue<T1>(stringParameters[0]);
            T2 param2 = GetValue<T2>(stringParameters[1]);
            return new object[] { param1, param2 };
        }
    }

    public class ActionLogCommand<T1, T2, T3> : ParameterActionLogCommand
    {
        public ActionLogCommand(CommandValues commandValues, Action<T1, T2, T3> action) : base(commandValues, action) { }

        protected override object[] ConvertParameters(string[] stringParameters)
        {
            T1 param1 = GetValue<T1>(stringParameters[0]);
            T2 param2 = GetValue<T2>(stringParameters[1]);
            T3 param3 = GetValue<T3>(stringParameters[2]);
            return new object[] { param1, param2, param3 };
        }
    }
}