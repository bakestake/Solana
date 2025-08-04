using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamegaard.RuntimeDebug
{
    public class CommandFieldSuggestor : InputFieldSuggestor<LogCommandBase>
    {
        protected override void OnStepChanged(int index)
        {
            if (showingSuggestions.Length == 0 || showingSuggestions.Length <= index) return;
            SetSuggestionMaker(showingSuggestions[index].Format);
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
                    SetSuggestionMaker(bestMatch.Format);
                    dropdown.ShowOptions(showingSuggestions.Select(c => c.Format).ToArray());
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

        protected override bool GetValidOptions(string value, out LogCommandBase[] options, SearchMode searchMode = SearchMode.LevenshteinSearch)
        {
            IEnumerable<LogCommandBase> filteredSuggestions = allSuggestions;

            switch (searchMode)
            {
                default:
                case SearchMode.Contains:
                    filteredSuggestions = filteredSuggestions.Where(x => x.Format.Contains(value, StringComparison.OrdinalIgnoreCase));
                    break;
                case SearchMode.LevenshteinSearch:
                    filteredSuggestions = SearchUtils.ApplyLevenshteinSearch(filteredSuggestions, value);
                    break;
                case SearchMode.DamerauLevenshtein:
                    filteredSuggestions = SearchUtils.ApplyDamerauLevenshteinSearch(filteredSuggestions, value);
                    break;
                case SearchMode.Sift4:
                    filteredSuggestions = SearchUtils.ApplySift4CalculatorSearch(filteredSuggestions, value);
                    break;
            }

            options = filteredSuggestions
                .OrderBy(x => x.Format)
                .ToArray();

            return options.Length > 0;
        }
    }
}