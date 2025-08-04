using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using Bakeland;
using System.Collections.Generic;
using System.Linq;

public class QuestLogUI : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private QuestLogHUD hud;

    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI goldRewardsText;
    [SerializeField] private TextMeshProUGUI itemRewardsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;

    private void Awake()
    {
        HideUI();
    }

    private void OnEnable()
    {
        GameEventsManager.NotNullInstance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        if (!GameEventsManager.HasInstance) return;
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && PlayerController.canInteract)
        {
            ToggleQuestlog();
        }
    }

    public void ToggleQuestlog()
    {
        if (contentParent.activeInHierarchy)
        {
            SoundManager.Instance.PlaySfx(SoundManager.Instance.bulletinBoardOut);
            contentParent.GetComponent<CanvasGroup>().DOComplete();
            contentParent.GetComponent<CanvasGroup>().DOFade(0, 0.25f).OnComplete(HideUI);
            // HideUI();
        }
        else
        {
            ShowUI();
            contentParent.GetComponent<CanvasGroup>().DOComplete();
            contentParent.GetComponent<CanvasGroup>().DOFade(1, 0.25f);
        }
    }

    private void ShowUI()
    {
        contentParent.SetActive(true);
        PlayerController.canInteract = false;
        PlayerController.canMove = false;
        SortButtonsByTextColor();

        // note - this needs to happen after the content parent is set active,
        // or else the onSelectAction won't work as expected
        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }

        SoundManager.Instance.PlaySfx(SoundManager.Instance.bulletinBoardIn);
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
        PlayerController.canInteract = true;
        PlayerController.canMove = true;
        EventSystem.current.SetSelectedGameObject(null);

        SoundManager.Instance.PlaySfx(SoundManager.Instance.bulletinBoardOut);
    }

    private void QuestStateChange(Quest quest)
    {
        // add the button to the scrolling list if not already added
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () =>
        {
            // Get the current quest from QuestManager instead of using the captured quest
            Quest currentQuest = QuestManager.Instance.GetQuestById(quest.info.id);
            SetQuestLogInfo(currentQuest);
            hud.SetQuest(quest, true);
        });

        // initialize the first selected button if not already so that it's
        // always the top button
        if (firstSelectedButton == null)
        {
            firstSelectedButton = questLogButton.button;
        }

        // set the button color based on quest state
        questLogButton.SetState(quest.state);

        // Update the quest info if this quest is currently selected
        if (questDisplayNameText.text == quest.info.displayName)
        {
            Quest currentQuest = QuestManager.Instance.GetQuestById(quest.info.id);
            SetQuestLogInfo(currentQuest);
        }
    }

    private void SetQuestLogInfo(Quest quest)
    {
        Debug.Log($"Setting quest log info for {quest.info.displayName}, State: {quest.state}");

        // quest name
        questDisplayNameText.text = quest.info.displayName;

        // status
        questStatusText.text = quest.GetFullStatusText();

        // requirements
        questRequirementsText.text = "";
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            questRequirementsText.text += prerequisiteQuestInfo.displayName + "\n";
        }

        if (quest.info.questPrerequisites.Length == 0)
        {
            questRequirementsText.text += "None";
        }

        // rewards
        if (quest.info.goldReward == 0)
        {
            goldRewardsText.text = " ";
            goldRewardsText.gameObject.SetActive(false);
        }
        else
        {
            goldRewardsText.gameObject.SetActive(true);
            goldRewardsText.text = quest.info.goldReward + " Gold";
        }

        if (quest.info.itemReward != null)
        {
            itemRewardsText.text = quest.info.itemReward.ItemName + " x" + quest.info.itemRewardAmount.ToString();
        }
        else
        {
            itemRewardsText.text = " ";
        }
    }

    // TO-DO: remake this
    private void SortButtonsByTextColor()
    {
        // define color priority
        Dictionary<QuestState, int> questStatePriority = new()
        {
            { QuestState.CAN_FINISH, 0 },
            { QuestState.IN_PROGRESS, 1 },
            { QuestState.CAN_START, 2 },
            { QuestState.FINISHED, 3 }
        };

        // get all buttons
        QuestLogButton[] buttons = buttonsParent.GetComponentsInChildren<QuestLogButton>();

        // sort based on color order
        var sortedButtons = buttons.OrderBy(button =>
        {
            QuestState buttonQuestState = button.questState;
            return questStatePriority.ContainsKey(buttonQuestState) ? questStatePriority[buttonQuestState] : int.MaxValue;
        }).ToArray();

        // reorder buttons in hierarchy
        for (int i = 0; i < sortedButtons.Length; i++)
        {
            sortedButtons[i].transform.SetSiblingIndex(i);
        }
    }
}
