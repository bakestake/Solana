using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class ConsoleInputHandler : MonoBehaviour
    {
        [SerializeField] private CommandFieldSuggestor suggestor;
        [SerializeField] private TextChat textChat;

        private void Update()
        {
            ProcessInputs();
        }

        public void ProcessInputs()
        {
            if (!suggestor.InputField.isFocused) return;

            if (suggestor.IsShowing)
            {
                if (Input.GetKeyDown(KeyCode.Tab) && suggestor.HasBestMatch)
                {
                    suggestor.OnOptionSelected(suggestor.SuggestionText.text);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    suggestor.Dropdown.OptionsStep.PreviousStep();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    suggestor.Dropdown.OptionsStep.NextStep();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    NavigateTextHistory(-1);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    NavigateTextHistory(1);
                }
            }
        }

        private void NavigateTextHistory(int direction)
        {
            if (textChat.CurrentMessageCount == 0) return;

            string text;

            if (direction == 1)
            {
                text = textChat.ChatHistory.GetNextMessage().MessageText;
            }
            else
            {
                text = textChat.ChatHistory.GetPreviousMessage().MessageText;
            }
            suggestor.OnOptionSelected(text);
        }
    }
}