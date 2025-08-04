using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class QuestLogButton : MonoBehaviour, ISelectHandler
{
    public Button button { get; private set; }
    private TextMeshProUGUI buttonText;
    private UnityAction onSelectAction;
    public string questId { get; private set; }
    public QuestState questState;

    [Header("Colors")]
    public Color canStartColor;
    public Color canFinishColor;
    public Color finishedColor;

    // because we're instantiating the button and it may be disabled when we
    // instantiate it, we need to manually initialize anything here.
    public void Initialize(string displayName, UnityAction selectAction)
    {
        this.button = this.GetComponent<Button>();
        this.buttonText = this.GetComponentInChildren<TextMeshProUGUI>();

        this.buttonText.text = displayName;
        this.onSelectAction = selectAction;
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelectAction();
    }

    public void SetState(QuestState state)
    {
        questState = state;

        switch (questState)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
            case QuestState.CAN_START:
                buttonText.color = canStartColor;
                break;
            case QuestState.IN_PROGRESS:
            case QuestState.CAN_FINISH:
                buttonText.color = canFinishColor;
                break;
            case QuestState.FINISHED:
                buttonText.color = finishedColor;
                break;
            default:
                Debug.LogWarning("Quest State not recognized by switch statement: " + state);
                break;
        }
    }

    public void Setup(Quest quest, UnityAction onClickAction)
    {
        questId = quest.info.id;
        buttonText.text = quest.info.displayName;
        button.onClick.AddListener(onClickAction);
        SetState(quest.state);
    }
}
