using TMPro;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public abstract class SuggestionMaker : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI suggestionText;
        public abstract void SetSuggestion(string text);
        public virtual void Clear()
        {
            suggestionText.text = "";
        }
    }
}