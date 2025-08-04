using UnityEngine;
using TMPro;
using System;

namespace Bakeland
{
    public class QuestLogHUD : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text questTitle, questSteps;

        private Quest currentShowingQuest;

        private void Start()
        {
            GameEventsManager.Instance.questEvents.onFinishQuest += OnFinishQuest;
            GameEventsManager.Instance.questEvents.onQuestStateChange += OnQuestStateChange;
            GameEventsManager.Instance.questEvents.onStartQuest += OnStartQuest;

            Hide();
        }

        private void OnFinishQuest(string obj)
        {
            Hide();
            currentShowingQuest = null;
        }

        private void OnQuestStateChange(Quest quest)
        {
            if ((currentShowingQuest == null || currentShowingQuest == quest) && (quest.state == QuestState.CAN_FINISH || quest.state == QuestState.IN_PROGRESS))
            {
                Show();
                SetQuest(quest);
            }
        }

        private void OnStartQuest(string obj)
        {
            Quest quest = QuestManager.Instance.GetQuestById(obj);
            SetQuest(quest, show: true);
        }

        public void SetQuest(Quest quest, bool show = false)
        {
            if (show) Show();

            currentShowingQuest = quest;
            questTitle.text = quest.info.displayName;
            questSteps.text = quest.GetFullStatusText();
        }

        public void Show()
        {
            panel.SetActive(true);
        }

        public void Hide()
        {
            panel.SetActive(false);
        }
    }
}