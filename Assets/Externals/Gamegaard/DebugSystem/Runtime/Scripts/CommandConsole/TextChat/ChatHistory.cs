using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    [Serializable]
    public class ChatHistory
    {
        [Min(1)]
        [SerializeField] private int historyLimit = 10;
        [SerializeField] private StepTracker historySelection;

        private readonly Queue<ChatMessageText> messageLog = new Queue<ChatMessageText>();

        public int CurrentMessageCount => messageLog.Count;
        public bool HasAnyMessage => messageLog.Count > 0;
        public StepTracker HistorySelection => historySelection;

        public event Action<ChatMessageText> OnMessageRemoved;

        public void AddMessage(ChatMessageText message)
        {
            if (CurrentMessageCount >= historyLimit)
            {
                ChatMessageText oldMessage = messageLog.Dequeue();
                oldMessage.gameObject.SetActive(false);
                OnMessageRemoved?.Invoke(oldMessage);
            }

            messageLog.Enqueue(message);
            historySelection.SetAmount(CurrentMessageCount);
        }

        public ChatMessageText GetNextMessage()
        {
            if (!HasAnyMessage) return null;

            historySelection.NextStep();
            return messageLog.ToArray()[historySelection.CurrentStepIndex];
        }

        public ChatMessageText GetPreviousMessage()
        {
            if (!HasAnyMessage) return null;

            historySelection.PreviousStep();
            return messageLog.ToArray()[historySelection.CurrentStepIndex];
        }

        public IEnumerable<ChatMessageText> GetAllMessages()
        {
            return messageLog.ToArray();
        }

        public void ResetHistoryIndex()
        {
            historySelection.SetStep(CurrentMessageCount - 1);
        }

        public void Clear()
        {
            messageLog.Clear();
            historySelection.SetAmount(0);
        }
    }
}