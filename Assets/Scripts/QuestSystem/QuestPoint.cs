using UnityEngine;
using UnityEngine.Events;

public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    [Header("Events")]
    [SerializeField] private UnityEvent OnQuestStart;
    [SerializeField] private UnityEvent OnQuestEnd;

    private string questId;
    private QuestState currentQuestState;
    private QuestIcon questIcon;
    private DialogueTrigger dialogueTrigger;
    private bool hasDialogueTrigger;

    private void Start()
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
        currentQuestState = QuestManager.Instance.GetQuestById(questId).state;
        hasDialogueTrigger = TryGetComponent(out dialogueTrigger);
        UpdateIconAndDialogue();

        //  Debug.Log(name + ":start");
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.Instance.miscEvents.OnDialogueEnded += DialogueEnded;

        // Debug.Log(name + ":on enable");
    }

    private void OnDisable()
    {
        if (!GameEventsManager.HasInstance) return;
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.Instance.miscEvents.OnDialogueEnded -= DialogueEnded;

        //  Debug.Log(name + ":on disable");
    }

    private void DialogueEnded(DialogueTrigger trigger)
    {
        if (!string.IsNullOrEmpty(questId) && startPoint && currentQuestState == QuestState.CAN_START && trigger.gameObject == this.gameObject)
        {
            GameEventsManager.Instance.questEvents.StartQuest(questId);
            OnQuestStart?.Invoke();
            //  Debug.Log(name + ":start quest");
        }
        else if (!string.IsNullOrEmpty(questId) && finishPoint && currentQuestState == QuestState.CAN_FINISH && trigger.gameObject == this.gameObject)
        {
            GameEventsManager.Instance.questEvents.FinishQuest(questId);
            OnQuestEnd?.Invoke();
            // Debug.Log(name + ":finish quest quest");
        }
    }

    private void QuestStateChange(Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            //Debug.Log(name + ":quest state change");

            currentQuestState = quest.state;
            UpdateIconAndDialogue();
        }
    }

    private void UpdateIconAndDialogue()
    {
        //Debug.Log(name + ":update icon and dialogue");

        questIcon.SetState(currentQuestState, startPoint, finishPoint);

        Quest quest = QuestManager.Instance.GetQuestById(questId);
        bool isQuestRepeatable = QuestManager.Instance.IsQuestRepeatable(quest);
        int questRepeatedCount = isQuestRepeatable ? QuestManager.Instance.RepeatableQuestCount(quest) : 0;

        if (!hasDialogueTrigger) return;

        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            //Debug.Log(name + ":load start dialogue");

            Dialogue startDialogue = questInfoForPoint.questStartDialogue;
            // is a repeatable quest? if so, load the "repeat" dialogues
            if (isQuestRepeatable && questRepeatedCount > 0)
            {
                startDialogue = (questInfoForPoint as QuestInfoSORepeatable).startDialogueRepeat;
            }
            dialogueTrigger.LoadDialogue(startDialogue);
        }
        else if (currentQuestState.Equals(QuestState.IN_PROGRESS) && startPoint)
        {
            //Debug.Log(name + ":load progress dialogue");

            Dialogue progressDialogue = questInfoForPoint.questProgressDialogue;
            // is a repeatable quest? if so, load the "repeat" dialogues
            if (isQuestRepeatable && questRepeatedCount > 0)
            {
                progressDialogue = (questInfoForPoint as QuestInfoSORepeatable).questProgressDialogue;
            }
            dialogueTrigger.LoadDialogue(progressDialogue);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            Dialogue finishDialogue = questInfoForPoint.questFinishDialogue;
            // is a repeatable quest? if so, load the "repeat" dialogues
            if (isQuestRepeatable && questRepeatedCount > 0)
            {
                finishDialogue = (questInfoForPoint as QuestInfoSORepeatable).finishDialogueRepeat;
            }
            dialogueTrigger.LoadDialogue(finishDialogue);
        }
        else if (currentQuestState.Equals(QuestState.FINISHED) && finishPoint)
        {
            //Debug.Log(name + ":reset dialogue");
            dialogueTrigger.ResetDialogue();
        }
    }

    /* TO-DO: check if this quest is now available when another quest is finished from event
    private void OtherQuestFinished(string questId)
    {
        bool isOtherQuestRelevant = questInfoForPoint.questPrerequisites.Any(x => x.questPrerequisites.Any(x => x.id == questId));
        if (isOtherQuestRelevant == false) return;

        // TO-DO: add multiple quest prerequisites check here
    }
    */
}
