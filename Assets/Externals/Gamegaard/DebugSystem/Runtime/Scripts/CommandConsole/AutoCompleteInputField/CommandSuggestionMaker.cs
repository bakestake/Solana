using TMPro;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class CommandSuggestionMaker : SuggestionMaker
    {
        [SerializeField] private TextMeshProUGUI[] paramTexts;

        private int paramTextCount;
        private string previousText;

        private void Awake()
        {
            paramTextCount = paramTexts.Length;
            suggestionText.color = CommandConfig.Instance.SugestionTextColor;
            foreach (TextMeshProUGUI param in paramTexts)
            {
                param.color = CommandConfig.Instance.ParamTextColor;
            }
            previousText = "";
        }

        public override void SetSuggestion(string text)
        {
            if (CommandCenter.Instance.TryGetCommand(text, out LogCommandBase command))
            {
                if (CommandConfig.Instance.HasCommandPrefix)
                {
                    suggestionText.text = $"{CommandConfig.Instance.CommandPrefix}{command.Command}";
                }
                else
                {
                    suggestionText.text = command.Command;
                }

                int spaceCount = text.Split(' ').Length - 1;

                for (int i = 0; i < paramTextCount; i++)
                {
                    if (i < spaceCount)
                    {
                        paramTexts[i].text = "";
                    }
                    else if (i < command.ParamAmount)
                    {
                        paramTexts[i].text = command.Params[i];
                    }
                    else
                    {
                        paramTexts[i].text = "";
                    }
                }

                previousText = text;
            }
        }

        public override void Clear()
        {
            base.Clear();
            foreach (TextMeshProUGUI paramText in paramTexts)
            {
                paramText.text = "";
            }
        }
    }
}
