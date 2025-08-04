using TMPro;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public abstract class InputFieldSuggestor<T> : AutoCompleteBehaviour<T, string>
    {
        [Header("Input Field References")]
        [SerializeField] protected TMP_InputField inputField;
        [SerializeField] protected TextMeshProUGUI suggestionText;
        [SerializeField] protected SuggestionList dropdown;
        [SerializeField] protected SuggestionMaker suggestionMaker;

        public TMP_InputField InputField => inputField;
        public TextMeshProUGUI SuggestionText => suggestionText;
        public SuggestionList Dropdown => dropdown;

        protected virtual void Awake()
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
            inputField.onSubmit.AddListener(x => Submit());
        }

        protected virtual void OnEnable()
        {
            dropdown.OptionsStep.OnStepChanged += OnStepChanged;
        }

        protected virtual void OnDisable()
        {
            dropdown.OptionsStep.OnStepChanged -= OnStepChanged;
        }

        protected void Start()
        {
            dropdown.OnOptionSelected += OnOptionSelected;
        }

        private void OnDestroy()
        {
            dropdown.OnOptionSelected -= OnOptionSelected;
        }

        public override void Submit()
        {
            base.Submit();
            inputField.SetTextWithoutNotify("");
        }

        protected override void ClearSuggestions()
        {
            dropdown.Hide();
            bestMatch = default;
            hasBestMatch = false;
            IsShowing = false;
            ClearSuggestionMaker();
        }

        protected void ClearSuggestionMaker()
        {
            if (suggestionMaker == null) return;
            suggestionMaker.Clear();
        }

        protected void SetSuggestionMaker(string text)
        {
            if (suggestionMaker == null) return;
            suggestionMaker.SetSuggestion(text);
        }
    }
}