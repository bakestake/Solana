namespace Gamegaard.RuntimeDebug
{
    public class SuggestionsAttribute : SuggestorTagAttribute
    {
        public readonly object[] suggestions;

        public SuggestionsAttribute(params object[] suggestions)
        {
            this.suggestions = suggestions;
        }
    }
}