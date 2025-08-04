using TMPro;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class TextChat : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected bool isMessageTimeVisible = true;
        [SerializeField] protected TrackedPrefabInstantiator<ChatMessageText> instantiator;
        [SerializeField] protected TMP_InputField inputField;
        [SerializeField] private ChatHistory chatHistory;

        [Header("Colors")]
        [SerializeField] protected Color manualColor = Color.white;

        private float currentZoom = 1;

        public ChatHistory ChatHistory => chatHistory;
        public int CurrentMessageCount => chatHistory.CurrentMessageCount;
        public bool HasAnyMessage => chatHistory.HasAnyMessage;

        private void OnEnable()
        {
            inputField.onSubmit.AddListener(x => chatHistory.ResetHistoryIndex());
        }

        private void OnDisable()
        {
            inputField.onSubmit.RemoveListener(x => chatHistory.ResetHistoryIndex());
        }

        public virtual void SendInputMessage(string message, bool saveToHistory, string customPrefix = null)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            CreateTextMessage(message, saveToHistory, manualColor, customPrefix);
        }

        public virtual void CreateTextMessage(string message, bool saveToHistory, Color color, string customPrefix = null)
        {
            ChatMessageText newText = instantiator.Instantiate();
            newText.InitializeText(message, customPrefix, currentZoom, isMessageTimeVisible, color);
            
            if (saveToHistory)
            {
                chatHistory.AddMessage(newText);
            }
        }

        public ChatMessageText GetNextMessage()
        {
            return chatHistory.GetNextMessage();
        }

        public ChatMessageText GetPreviousMessage()
        {
            return chatHistory.GetPreviousMessage();
        }

        public void SetZoom(float value)
        {
            currentZoom = value;
            foreach (var item in chatHistory.GetAllMessages())
            {
                item.SetZoom(currentZoom);
            }
        }

        public void Clear()
        {
            instantiator.DestroyInstantiatedObjects();
            chatHistory.Clear();
        }
    }
}