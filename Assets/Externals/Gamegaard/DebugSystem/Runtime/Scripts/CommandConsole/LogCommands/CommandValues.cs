using System.Collections.Generic;

namespace Gamegaard.RuntimeDebug
{
    public struct CommandValues
    {
        public string id;
        public string format;
        public string description;
        public string tooltip;
        public string callbackText;
        public MonoBehaviourTarget target;
        public Dictionary<string, SuggestorTagAttribute> suggestionsByParam;

        public override string ToString()
        {
            return $"Id:[{id}], Format:[{format}], Description:[{description}], Tooltip:[{tooltip}], Callback:[{callbackText}]";
        }
    }
}