namespace Gamegaard.RuntimeDebug
{
    public class SceneNameSuggestionAttribute : SuggestorTagAttribute
    {
        public readonly bool loadedOnly;

        public SceneNameSuggestionAttribute(bool loadedOnly = true)
        {
            this.loadedOnly = loadedOnly;
        }
    }
}