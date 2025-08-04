using System;
using System.Linq;

namespace Gamegaard.RuntimeDebug
{
    public class StringFieldSuggestor : InputFieldSuggestor<string>
    {
        protected override void OnStepChanged(int index)
        {
            if (showingSuggestions.Length == 0 || showingSuggestions.Length <= index) return;
            SetSuggestionMaker(showingSuggestions[index]);
            hasBestMatch = true;
            bestMatch = showingSuggestions[index];
        }

        public override void OnOptionSelected(string text)
        {
            inputField.SetTextWithoutNotify(text);
            inputField.ActivateInputField();
            inputField.caretPosition = text.Length;
            ClearSuggestions();
        }

        protected override void OnValueChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                ClearSuggestions();
            }
            else
            {
                if (GetValidOptions(value, out showingSuggestions, searchMode))
                {
                    hasBestMatch = true;
                    bestMatch = showingSuggestions[0];
                    SetSuggestionMaker(bestMatch);
                    dropdown.ShowOptions(showingSuggestions);
                    IsShowing = true;
                }
                else
                {
                    ClearSuggestions();
                }
            }
        }

        protected override string GetSelectedValue()
        {
            return inputField.text;
        }

        protected override bool GetValidOptions(string value, out string[] options, SearchMode searchMode = SearchMode.LevenshteinSearch)
        {
            options = allSuggestions
                .Where(x => x.StartsWith(value, StringComparison.OrdinalIgnoreCase) && x != value)
                .OrderBy(x => x)
                .ToArray();

            return options.Length > 0;
        }
    }
}