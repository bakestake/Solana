using System;

namespace Gamegaard.RuntimeDebug
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public readonly string id;
        public readonly string format;
        public readonly string description;
        public readonly string tooltip;
        public readonly string callbackText;
        public readonly bool isDefault;
        public readonly Platforms validPlatforms = Platforms.All;

        public CommandAttribute()
        {
            isDefault = true;
        }

        public CommandAttribute(string commandID)
        {
            this.id = commandID;
            format = $"!{commandID}";
            isDefault = false;
        }

        public CommandAttribute(string commandID, string commandFormat, string description = "", string tooltip = "", string callbackText = "", Platforms validPlatforms = Platforms.All)
        {
            this.id = commandID;
            this.format = commandFormat;
            this.validPlatforms = validPlatforms;
            this.description = description;
            this.tooltip = tooltip;
            this.callbackText = callbackText;
            isDefault = false;
        }
    }
}