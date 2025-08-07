using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { get; private set; }

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;
    public ES3Cloud cloud;
    public string walletAddress;

    public Dictionary<string, Quest> questMap;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one QuestManager in the scene.");
        }
        instance = this;
        questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
    }

    private void Start()
    {
        // SetupQuests();
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;

        // check quest prerequisites for completion
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void Update()
    {
        // loop through ALL quests
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void StartQuest(string id)
    {
        Debug.Log($"Attempting to start quest with ID: {id}");

        if (!questMap.ContainsKey(id))
        {
            Debug.LogError($"Cannot start quest. Quest with ID '{id}' not found in questMap.");
            return;
        }

        Quest quest = GetQuestById(id);
        if (quest != null && quest.state == QuestState.CAN_START)
        {
            quest.state = QuestState.IN_PROGRESS;
            quest.InstantiateCurrentQuestStep(this.transform);

            GameEventsManager.instance.questEvents.QuestStateChange(quest);
            Debug.Log($"Quest {id} started. New state: {quest.state}");
        }
        else
        {
            Debug.LogWarning($"Cannot start quest {id}. Current state: {quest?.state}");
        }
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        // Destroy the current quest step GameObject before moving to next step
        QuestStep currentStep = GetCurrentQuestStep(quest);
        if (currentStep != null)
        {
            Destroy(currentStep.gameObject);
        }

        // move on to the next step
        quest.MoveToNextStep();

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private QuestStep GetCurrentQuestStep(Quest quest)
    {
        // Find the current quest step GameObject
        QuestStep[] questSteps = GetComponentsInChildren<QuestStep>();
        foreach (QuestStep step in questSteps)
        {
            if (step.QuestId == quest.info.id)  // You'll need to add this property to QuestStep
            {
                return step;
            }
        }
        return null;
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        // GiveItemsAndGold(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest)
    {
        PlayerCurrency.instance.AddGold(quest.info.goldReward);
        if (quest.info.itemReward != null)
        {
            LocalGameManager.Instance.inventoryContainer.Add(quest.info.itemReward, quest.info.itemRewardAmount);
        }
    }

    // private void GiveItemsAndGold(Quest quest)
    // {
    //     PlayerCurrency.instance.SubtractGold(quest.info.goldToLose);
    //     if (quest.info.itemToLose != null)
    //     {
    //         LocalGameManager.Instance.inventoryContainer.Remove(quest.info.itemToLose, quest.info.itemToLoseAmount);
    //     }
    // }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();

        Debug.Log($"Loading quests. Found {allQuests.Length} quests.");

        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (questInfo != null)
            {
                Debug.Log($"Processing quest: {questInfo.name} with ID: {questInfo.id}");
                if (!string.IsNullOrEmpty(questInfo.id))
                {
                    idToQuestMap.Add(questInfo.id, new Quest(questInfo));
                }
                else
                {
                    Debug.LogError($"Quest {questInfo.name} has no ID!");
                }
            }
        }

        return idToQuestMap;
    }

    public void ResetQuestMap()
    {
        questMap = CreateQuestMap();
    }

    public Quest GetQuestById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Attempting to get quest with null or empty ID");
            return null;
        }

        if (!questMap.ContainsKey(id))
        {
            Debug.LogError($"Quest with ID '{id}' not found in questMap. Available quests: {string.Join(", ", questMap.Keys)}");
            return null;
        }

        return questMap[id];
    }

    public bool CheckQuestStartedById(string id, List<QuestState> questStates)
    {
        Quest quest = questMap[id];
        if (questStates.Contains(quest.state))
        {
            Debug.Log("This quest state matches the asked state");
            return true;
        }
        return false;
    }

    public void SetupQuests()
    {
        foreach (Quest quest in questMap.Values)
        {
            Debug.Log($"Setting up quest: {quest.info.id}, State: {quest.state}, CurrentQuestStepIndex: {quest.currentQuestStepIndex}");

            // Initialize any loaded quest steps that are in progress
            if (quest.state == QuestState.IN_PROGRESS)
            {
                if (quest.CurrentStepExists())
                {
                    quest.InstantiateCurrentQuestStep(this.transform);
                    Debug.Log($"Instantiated quest step for {quest.info.id} at step {quest.currentQuestStepIndex}");
                }
                else
                {
                    Debug.LogWarning($"Quest {quest.info.id} is IN_PROGRESS but has no current step! CurrentQuestStepIndex: {quest.currentQuestStepIndex}");
                }
            }

            // Force a state change event to update UI
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    // private void OnApplicationQuit()
    // {
    //     foreach (Quest quest in questMap.Values)
    //     {
    //         SaveQuest(quest);
    //     }
    // }

    public void SaveQuests()
    {
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);

        }
    }

    private void SaveQuest(Quest quest)
    {
        try
        {
            QuestData questData = quest.GetQuestData();
            string serializedData = JsonUtility.ToJson(questData);
            ES3.Save(quest.info.id, serializedData, "Save/" + quest.info.id + ".es3");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    public void LoadQuests()
    {
        // First create the base quest map
        Dictionary<string, Quest> newQuestMap = CreateQuestMap();
        Dictionary<string, Quest> loadedQuestMap = new Dictionary<string, Quest>();

        // Then load saved states
        foreach (var questEntry in newQuestMap)
        {
            string questId = questEntry.Key;
            try
            {
                if (ES3.FileExists("Save/" + questId + ".es3") && loadQuestState)
                {
                    string serializedData = ES3.Load<string>(questId, "Save/" + questId + ".es3");
                    QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);

                    // Create new quest with loaded state
                    Quest loadedQuest = new Quest(questEntry.Value.info, questData.state, questData.questStepIndex, questData.questStepStates);
                    loadedQuestMap[questId] = loadedQuest;

                    Debug.Log($"Loaded quest {questId} with state: {questData.state}");
                }
                else
                {
                    // If no save data exists, keep the original quest
                    loadedQuestMap[questId] = questEntry.Value;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load quest {questId}: {e}");
                // On error, keep the original quest
                loadedQuestMap[questId] = questEntry.Value;
            }
        }

        // Update the questMap with our loaded quests
        questMap = loadedQuestMap;

        // Setup quests after loading
        SetupQuests();
    }

    public Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try
        {

            if (ES3.FileExists("Save/" + questInfo.id + ".es3") && loadQuestState)
            {
                string serializedData = ES3.Load<string>(questInfo.id, "Save/" + questInfo.id + ".es3"); ;
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            else
            {
                quest = new Quest(questInfo);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }
}
