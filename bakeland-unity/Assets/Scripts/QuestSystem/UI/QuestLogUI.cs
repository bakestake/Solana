using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class QuestLogUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI goldRewardsText;
    [SerializeField] private TextMeshProUGUI itemRewardsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void Update()
    {
        if (LocalGameManager.Instance.canUseKeybinds && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleQuestlog();
        }
    }

    public void ToggleQuestlog()
    {
        if (contentParent.activeInHierarchy)
        {
            SoundManager.instance.PlaySfx(SoundManager.instance.bulletinBoardOut);
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
        PlayerController.canMove = false;
        // note - this needs to happen after the content parent is set active,
        // or else the onSelectAction won't work as expected
        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }

        SoundManager.instance.PlaySfx(SoundManager.instance.bulletinBoardIn);
    }

    private void HideUI()
    {
        contentParent.SetActive(false);
        PlayerController.canMove = true;
        Debug.Log("Can Move true");
        EventSystem.current.SetSelectedGameObject(null);

        // SoundManager.instance.PlaySfx(SoundManager.instance.bulletinBoardOut);
    }

    private void QuestStateChange(Quest quest)
    {
        Debug.Log($"QuestLogUI: State change for quest {quest.info.displayName}, State: {quest.state}");

        // add the button to the scrolling list if not already added
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () =>
        {
            // Get the current quest from QuestManager instead of using the captured quest
            Quest currentQuest = QuestManager.instance.GetQuestById(quest.info.id);
            SetQuestLogInfo(currentQuest);
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
            Quest currentQuest = QuestManager.instance.GetQuestById(quest.info.id);
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
            itemRewardsText.text = quest.info.itemReward.Name + " x" + quest.info.itemRewardAmount.ToString();
        }
        else
        {
            itemRewardsText.text = " ";
        }
    }
}
