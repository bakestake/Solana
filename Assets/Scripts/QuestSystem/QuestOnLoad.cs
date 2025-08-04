using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bakeland
{
    public class QuestOnLoad : MonoBehaviour
    {
        [SerializeField] private QuestInfoSO questInfoSO;
        [SerializeField] private List<QuestState> questStates;
        [SerializeField] private UnityEvent OnTriggered;

        private IEnumerator Start()
        {
            while (QuestManager.Instance.GetQuestById(questInfoSO.id) == null)
            {
                Debug.LogWarning($"{name}:could not find initialized {questInfoSO.displayName}");
                yield return null;
            }

            if (QuestManager.Instance.CheckQuestStartedById(questInfoSO.id, questStates))
            {
                OnTriggered?.Invoke();
            }
        }
    }
}