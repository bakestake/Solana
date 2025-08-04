using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard.RuntimeDebug
{
    public class AutoCompleteButton : MonoBehaviour
    {
        private Button button;
        private TextMeshProUGUI textMeshPro;
        private bool isInitialized;

        public string Text { get; private set; }
        public Button Button => button;

        public void Initialize(string text, Action<string> onSelected)
        {
            if (!isInitialized)
            {
                button = GetComponent<Button>();
                textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            }
            Text = text;
            textMeshPro.text = text;
            button.onClick.AddListener(() => onSelected?.Invoke(text));
        }

        public void Initialize(string text, int index, Action<int> onSelectedAsIndex)
        {
            if (!isInitialized)
            {
                button = GetComponent<Button>();
                textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            }

            Text = text;
            textMeshPro.text = text;
            button.onClick.AddListener(() => onSelectedAsIndex?.Invoke(index));
        }
    }
}