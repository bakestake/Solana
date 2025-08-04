using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard.RuntimeDebug
{
    public class SuggestionList : MonoBehaviour
    {
        [Header("Settings")]
        [Min(1)]
        [SerializeField] private int maxSuggestionAmount = 10;
        [Min(1)]
        [SerializeField] private int maxContentShowButtonSize = 10;

        [Header("References")]
        [SerializeField] private AutoCompleteButton optionButtonPrefab;
        [SerializeField] private RectTransform contentArea;
        [SerializeField] private StepTracker optionsStep;
        [SerializeField] private GameObject moreOptionsText;

        private readonly List<AutoCompleteButton> currentButtons = new List<AutoCompleteButton>();
        private SmartScrollRect autoScroll;
        private RectTransform rectTransform;
        private bool isSelecting;
        private Vector2 buttonSize;

        public StepTracker OptionsStep => optionsStep;

        public event Action<string> OnOptionSelected;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            autoScroll = GetComponent<SmartScrollRect>();
            buttonSize = optionButtonPrefab.GetComponent<RectTransform>().sizeDelta;
            OnOptionSelected += x => Hide();
        }

        private void OnDestroy()
        {
            OnOptionSelected -= x => Hide();
        }

        private void OnEnable()
        {
            OptionsStep.OnStepChanged += OnStepChanged;
        }

        private void OnDisable()
        {
            OptionsStep.OnStepChanged -= OnStepChanged;
        }

        private void Update()
        {
            if (!isSelecting) return;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                OptionsStep.PreviousStep();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                OptionsStep.NextStep();
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                OnOptionSubmit(optionsStep.CurrentStepIndex);
            }
        }

        private void OnOptionSubmit(int index)
        {
            OnOptionSelected?.Invoke(currentButtons[index].Text);
        }

        private void OnStepChanged(int index)
        {
            if (!isSelecting)
            {
                isSelecting = true;
            }

            if (currentButtons.Count == 0) return;
            currentButtons[index].Button.Select();
        }

        public void ShowOptions(string[] options)
        {
            bool hasExtraSuggestions = options.Length > maxSuggestionAmount;
            if (hasExtraSuggestions)
            {
                options = options.Take(maxSuggestionAmount).ToArray();
            }

            gameObject.SetActive(true);
            foreach (AutoCompleteButton button in currentButtons)
            {
                Destroy(button.gameObject);
            }
            currentButtons.Clear();

            List<Selectable> selectables = new List<Selectable>();
            for (int i = 0; i < options.Length; i++)
            {
                string option = options[i];
                AutoCompleteButton button = Instantiate(optionButtonPrefab, contentArea);
                button.Initialize(option, i, OnOptionSubmit);
                selectables.Add(button.GetComponent<Selectable>());
                currentButtons.Add(button);
            }

            moreOptionsText.transform.SetAsLastSibling();
            moreOptionsText.SetActive(hasExtraSuggestions);
            OptionsStep.SetAmount(options.Length);
            RecalculateSize();
            autoScroll.SetSelectables(selectables);
        }

        private void RecalculateSize()
        {
            float ySize = buttonSize.y;
            int maxSize = Mathf.Min(currentButtons.Count, maxContentShowButtonSize);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, ySize * maxSize);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            OptionsStep.SetAmount(0);
        }
    }
}