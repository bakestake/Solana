using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.RuntimeDebug
{
    public abstract class AutoCompleteBehaviour<T, G> : MonoBehaviour
    {
        [SerializeField] protected SearchMode searchMode;

        [Header("Events")]
        [SerializeField] public UnityEvent<G> OnSubmit;

        protected List<T> allSuggestions = new List<T>();
        protected bool hasBestMatch;
        protected T bestMatch;
        protected T[] showingSuggestions;

        public bool IsShowing { get; protected set; }
        public bool HasBestMatch => hasBestMatch;

        public virtual void Submit()
        {
            OnSubmit?.Invoke(GetSelectedValue());
            ClearSuggestions();
        }

        public abstract void OnOptionSelected(string text);
        protected abstract G GetSelectedValue();
        protected abstract void OnStepChanged(int index);
        protected abstract void OnValueChanged(string value);
        protected abstract bool GetValidOptions(string value, out T[] options, SearchMode searchMode = SearchMode.LevenshteinSearch);

        public void SetAutoCompleteOptions(IEnumerable<T> options)
        {
            allSuggestions = options.ToList();
        }

        public void AddAutoCompleteOption(T option)
        {
            allSuggestions.Add(option);
        }

        public void RemoveAutoCompleteOption(T option)
        {
            allSuggestions.Remove(option);
        }

        public void ClearAutoCompleteOptions()
        {
            allSuggestions.Clear();
        }

        protected abstract void ClearSuggestions();
    }
}