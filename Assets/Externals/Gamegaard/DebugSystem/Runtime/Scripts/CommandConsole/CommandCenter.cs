using Gamegaard.Singleton;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class CommandCenter : MonoBehaviourSingleton<CommandCenter>
    {
        public readonly Dictionary<string, LogCommandBase> commandByID = new Dictionary<string, LogCommandBase>();
        private readonly List<string> commandDisplayList = new List<string>();

        public int CommandCount => commandByID.Count;
        public ReadOnlyCollection<string> CommandDisplayList { get; private set; }

        public event Action<LogCommandBase> OnCommandRegistered;
        public event Action<LogCommandBase> OnCommandRemoved;
        public event Action OnCommandsCleared;

        protected override void Awake()
        {
            base.Awake();
            CommandDisplayList = new ReadOnlyCollection<string>(commandDisplayList);
            RegisterAttributedCommandMethods();
        }

        public bool TryRegister(LogCommandBase command)
        {
            if (!ContainsCommand(command))
            {
                commandDisplayList.Add(command.Format);
                commandByID.Add(command.Format, command);
                OnCommandRegistered?.Invoke(command);
                return true;
            }
            return false;
        }

        public bool RemoveCommand(LogCommandBase command)
        {
            commandDisplayList.Remove(command.Format);

            if (commandByID.Remove(command.Id))
            {
                OnCommandRemoved?.Invoke(command);
                return true;
            }

            return false;
        }

        public bool TryGetCommand(string CommandFormat, out LogCommandBase logCommandBase)
        {
            if (ContainsCommand(CommandFormat))
            {
                logCommandBase = commandByID[CommandFormat];
                return true;
            }
            logCommandBase = null;
            return false;
        }

        public bool RemoveCommand(string CommandFormat)
        {
            return ContainsCommand(CommandFormat) && RemoveCommand(commandByID[CommandFormat]);
        }

        public bool ContainsCommand(LogCommandBase command)
        {
            return commandByID.ContainsKey(command.Format);
        }

        public bool ContainsCommand(string CommandFormat)
        {
            return commandByID.ContainsKey(CommandFormat);
        }

        public bool ContainsCommand(string CommandFormat, out LogCommandBase command)
        {
            if (!commandByID.ContainsKey(CommandFormat))
            {
                command = null;
                return false;
            }

            command = commandByID[CommandFormat];
            return true;
        }

        public void ClearCommands()
        {
            commandByID.Clear();
            commandDisplayList.Clear();
            OnCommandsCleared?.Invoke();
        }

        #region AutomaticRegister
        private void RegisterAttributedCommandMethods()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            sw.Start();

            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null).ToArray();
                }
                catch
                {
                    continue;
                }

                foreach (Type type in types)
                {
                    if (ShouldSkipType(type)) continue;

                    MethodInfo[] methods;
                    try
                    {
                        methods = type.GetMethods(flags);
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (MethodInfo method in methods)
                    {
                        CommandAttribute commandAttr = method.GetCustomAttribute<CommandAttribute>();
                        if (commandAttr == null || !commandAttr.validPlatforms.CheckPlatform()) continue;

                        Delegate action = CreateDelegateForMethod(method);
                        if (action == null) continue;

                        LogCommandBase logCommand = CreateCommandInstance(method, commandAttr, method.GetParameters(), action);
                        if (logCommand == null) continue;

                        if (!TryRegister(logCommand))
                        {
                            Debug.LogError("Failed to add command for method: " + method.Name);
                        }
                    }
                }
            }

            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds}ms");
        }

        private bool ShouldSkipType(Type type)
        {
            string ns = type.Namespace;
            return ns == null || ns.StartsWith("UnityEngine") || ns.StartsWith("UnityEditor") || ns.StartsWith("TMPro") || ns.StartsWith("Unity") || ns.StartsWith("System");
        }

        private Delegate CreateDelegateForMethod(MethodInfo method)
        {
            Type delegateType = GetDelegateType(method.GetParameters());
            try
            {
                if (method.IsStatic)
                {
                    return Delegate.CreateDelegate(delegateType, null, method);
                }
                else
                {
                    var instance = Activator.CreateInstance(method.DeclaringType);
                    return Delegate.CreateDelegate(delegateType, instance, method);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create delegate for method {method.Name} in type {method.DeclaringType.Name}: {ex.Message}");
                return null;
            }
        }

        private LogCommandBase CreateCommandInstance(MethodInfo method, CommandAttribute commandAttr, ParameterInfo[] parameters, Delegate action)
        {
            if (parameters.Length > 3)
            {
                Debug.LogError("Unsupported number of arguments for command method.");
                return null;
            }

            CommandValues commandValues = GenerateCommandValues(commandAttr, method, parameters);

            Type commandType = GetGenericCommandType(parameters.Length);
            if (parameters.Length > 0)
            {
                commandType = commandType.MakeGenericType(parameters.Select(p => p.ParameterType).ToArray());
            }

            return (LogCommandBase)Activator.CreateInstance(commandType, new object[] { commandValues, action });
        }

        private CommandValues GenerateCommandValues(CommandAttribute commandAttr, MethodInfo method, ParameterInfo[] parameters)
        {
            CommandValues values = new CommandValues
            {
                id = string.IsNullOrEmpty(commandAttr.id) ? method.Name : commandAttr.id,
                format = string.IsNullOrEmpty(commandAttr.format) ? GenerateFormat(method, parameters) : commandAttr.format,
                description = commandAttr.description,
                tooltip = commandAttr.tooltip,
                callbackText = commandAttr.callbackText,
                suggestionsByParam = CreateSuggestions(parameters)
            };

            return values;
        }

        private string GenerateFormat(MethodInfo method, ParameterInfo[] parameters)
        {
            string text = $"!{method.Name}";
            if (parameters.Length == 0)
            {
                return text;
            }
            else
            {
                return $"{text} {string.Join(" ", parameters.Select(p => "{" + p.ParameterType.Name + "}"))}";
            }
        }

        private Dictionary<string, SuggestorTagAttribute> CreateSuggestions(ParameterInfo[] parameterInfos)
        {
            var suggestionsByParamName = new Dictionary<string, SuggestorTagAttribute>();

            foreach (ParameterInfo parameter in parameterInfos)
            {
                var suggestor = parameter.GetCustomAttribute<SuggestorTagAttribute>();
                if (suggestor == null) continue;
                suggestionsByParamName.Add(parameter.Name, suggestor);
            }
            return suggestionsByParamName;
        }

        private Type GetGenericCommandType(int argumentLength)
        {
            switch (argumentLength)
            {
                case 0:
                    return typeof(ActionLogCommand);
                case 1:
                    return typeof(ActionLogCommand<>);
                case 2:
                    return typeof(ActionLogCommand<,>);
                case 3:
                    return typeof(ActionLogCommand<,,>);
            }
            Debug.LogError("Unsupported number of arguments for command method.");
            return null;
        }

        private Type GetDelegateType(ParameterInfo[] parameters)
        {
            return parameters.Length switch
            {
                0 => typeof(Action),
                1 => typeof(Action<>).MakeGenericType(parameters[0].ParameterType),
                2 => typeof(Action<,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType),
                3 => typeof(Action<,,>).MakeGenericType(parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType),
                _ => throw new InvalidOperationException("Unsupported number of arguments for command method."),
            };
        }
        #endregion
    }
}