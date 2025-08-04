using System;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class ChatMessageText : ZoomableText
    {
        private DateTime sendTime;
        private string messageText;
        private string prefix;

        public string MessageText => messageText;
        public string FormatedTime => $"{sendTime:HH:mm:ss}";
        public string Prefix => prefix;
        public bool UsePrefix { get; protected set; }

        public void InitializeText(string message, string prefix, float currentZoom, bool showCurrentTime, Color color)
        {
            sendTime = DateTime.Now;
            this.prefix = prefix;
            UsePrefix = !string.IsNullOrEmpty(prefix);
            messageText = message;

            SetZoom(currentZoom);
            SetColor(color);
            SetCurrentTimeVisibility(showCurrentTime);
        }

        public void SetColor(Color color)
        {
            TextComponent.color = color;
        }

        public void SetCurrentTimeVisibility(bool isTimeVisible)
        {
            TextComponent.text = isTimeVisible ? $"[{FormatedTime}] {GetTextWithPrefix()}" : GetTextWithPrefix();
        }

        public string GetTextWithPrefix()
        {
            return UsePrefix ? prefix + messageText : messageText;
        }
    }
}