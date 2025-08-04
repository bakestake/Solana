using System.Globalization;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Gamegaard.RuntimeDebug
{
    public abstract class LogCommandBase
    {
        [SerializeField] private string id;
        [SerializeField] private string format;
        [SerializeField] private string description;
        [SerializeField] private string tooltip;
        [SerializeField] private string callbackText;

        private const string regexPattern = @"^!(\w+)(?:\s+(.*))?$";
        private readonly Dictionary<string, SuggestorTagAttribute> suggestionsByParam = new Dictionary<string, SuggestorTagAttribute>();
        private readonly Regex regex = new Regex(regexPattern);
        protected readonly Delegate action;

        public bool HasCallbackText => !string.IsNullOrEmpty(callbackText);
        public bool HasTooltip => !string.IsNullOrEmpty(tooltip);
        public bool HasDescription => !string.IsNullOrEmpty(description);

        public string Id => id;
        public string Description => description;
        public string Tooltip => tooltip;
        public string Format => format;
        public string CommandCallbackText => callbackText;
        public string FormatedSuggestionText { get; set; }
        public string FormatedHelpText { get; set; }
        public string Command { get; set; }
        public string[] Params { get; set; }
        public int ParamAmount { get; set; }

        public LogCommandBase() { }

        public LogCommandBase(string id, string format, Delegate action, string description = "", string tooltip = "", string callback = "", Dictionary<string, SuggestorTagAttribute> suggestionsByParam = null)
        {
            this.id = id;
            this.description = description;
            this.tooltip = tooltip;
            this.format = format;
            this.action = action;

            callbackText = callback;
            FormatedHelpText = $"{id}: [{format}]";
            ParamAmount = CountFormatParameters(format);

            SetSugestionText(format);

            if (suggestionsByParam != null)
            {
                this.suggestionsByParam = suggestionsByParam;
            }
        }

        private void SetSugestionText(string format)
        {
            Match match = regex.Match(format);
            if (match.Success)
            {
                Command = match.Groups[1].Value;
                Params = match.Groups[2].Success ? match.Groups[2].Value.Split(' ') : new string[0];
            }

            string suggestionColor = ColorUtility.ToHtmlStringRGBA(CommandConfig.Instance.SugestionTextColor);
            string paramColor = ColorUtility.ToHtmlStringRGBA(CommandConfig.Instance.ParamTextColor);
            FormatedSuggestionText = $"<color={suggestionColor}>{CommandConfig.Instance.CommandPrefix}{Command}</color>";
            foreach (string param in Params)
            {
                FormatedSuggestionText += $"<color={paramColor}>{param}</Color>";
            }
        }

        private int CountFormatParameters(string commandFormat)
        {
            return Regex.Matches(commandFormat, @"\{[^{}]*\}").Count;
        }

        public virtual bool Validate(string message)
        {
            Match match = regex.Match(message);

            if (match.Success)
            {
                string commandName = match.Groups[1].Value;
                var parameters = match.Groups[2].Success ? match.Groups[2].Value.Split(' ') : new string[0];

                return commandName == Id && parameters.Length == ParamAmount;
            }
            return false;
        }

        public abstract void TryInvoke(string message);

        public bool TryGetSuggestions(string paramName, out string[] suggestions)
        {
            if (!suggestionsByParam.ContainsKey(paramName))
            {
                suggestions = null;
                return false;
            }

            SuggestorTagAttribute suggestor = suggestionsByParam[paramName];
            switch (suggestor)
            {
                case SceneNameSuggestionAttribute sceneNameSuggestion:
                    suggestions = new string[SceneManager.sceneCount];
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneAt(i);
                        if (!sceneNameSuggestion.loadedOnly || scene.isLoaded)
                        {
                            suggestions[i] = scene.name;
                        }
                    }
                    break;

                case SuggestionsAttribute suggestionsList:
                    suggestions = new string[suggestionsList.suggestions.Length];
                    for (int i = 0; i < suggestionsList.suggestions.Length; i++)
                    {
                        suggestions[i] = suggestionsList.suggestions[i].ToString();
                    }
                    break;

                default:
                    suggestions = null;
                    return false;
            }
            return true;
        }

        protected T GetValue<T>(string value)
        {
            Type type = typeof(T);

            return type switch
            {
                _ when type == typeof(int) => (T)(object)int.Parse(value),
                _ when type == typeof(float) => (T)(object)float.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture),
                _ when type == typeof(double) => (T)(object)double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture),
                _ when type == typeof(decimal) => (T)(object)decimal.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture),
                _ when type == typeof(long) => (T)(object)long.Parse(value),
                _ when type == typeof(ulong) => (T)(object)ulong.Parse(value),
                _ when type == typeof(short) => (T)(object)short.Parse(value),
                _ when type == typeof(ushort) => (T)(object)ushort.Parse(value),
                _ when type == typeof(byte) => (T)(object)byte.Parse(value),
                _ when type == typeof(sbyte) => (T)(object)sbyte.Parse(value),
                _ when type == typeof(char) => (T)(object)char.Parse(value),              
                _ when type == typeof(Type) => (T)(object)Type.GetType(value, throwOnError: true, ignoreCase: true),
                _ when type == typeof(GameObject) => (T)(object)GameObject.Find(value),
                _ when type.IsEnum => (T)Enum.Parse(type, value, ignoreCase: true),
                _ when type == typeof(bool) => (T)(object)bool.Parse(value),
                _ when type == typeof(string) => (T)(object)value,
                _ => ParserFinder.GetParser<T>().Parse(value),
            };
        }

        protected string[] GetArgs(string value)
        {
            return value.Split(',').Select(p => p.Trim()).ToArray();
        }
    }
}