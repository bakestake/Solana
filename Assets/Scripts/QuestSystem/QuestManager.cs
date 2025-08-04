using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bakeland;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [SerializeField] private AudioClip questStartClip;

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;
    public ES3Cloud cloud;

    public Dictionary<string, Quest> questMap;
    public Dictionary<Quest, int> questRepeatedMap;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one QuestManager in the scene.");
        }
        Instance = this;

        questMap = CreateQuestMap();
        questRepeatedMap = CreateRepeatableQuestMap();
    }

    private void OnEnable()
    {
        GameEventsManager.NotNullInstance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.NotNullInstance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.NotNullInstance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.NotNullInstance.questEvents.onQuestStepStateChange += QuestStepStateChange;
    }

    private void OnDisable()
    {
        if (!GameEventsManager.HasInstance) return;
        GameEventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;
    }

    public void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;

        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
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

    // TO-DO: for the love of everything that is holy change this to event based
    private void LateUpdate()
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

            GameEventsManager.Instance.questEvents.QuestStateChange(quest);
            SoundManager.Instance.PlaySfx(questStartClip);
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

    private async void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        QuestStep questInstance = GetCurrentQuestStep(quest);
        if (questInstance != null)
        {
            Destroy(questInstance.gameObject);
        }

        ConsumeItems(quest);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        await UserAndReferralApi.MarkQuestComplete(WalletConnectScript.connectedWalletAddress, quest.info.uid.ToString());

        // TO-DO: this is a super dumb quick fix for Ramen Rush to be replayable, REFACTOR LATER
        if (IsQuestRepeatable(quest))
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_START);

            // update how many times this quest was done
            if (questRepeatedMap.TryGetValue(quest, out int currentCount))
            {
                questRepeatedMap[quest] = currentCount + 1;
            }
            else
            {
                questRepeatedMap[quest] = 1;
            }

            Debug.Log($"quest manager:{quest.info.displayName} repeated count: {questRepeatedMap[quest]}");
        }
    }

    private void ClaimRewards(Quest quest)
    {
        PlayerCurrency.instance.AddGold(quest.info.goldReward);
        if (quest.info.itemReward != null)
        {
            LocalGameManager.Instance.inventoryContainer.Add(quest.info.itemReward, quest.info.itemRewardAmount);
        }
    }

    private void ConsumeItems(Quest quest)
    {
        if (quest.info.itemDelete != null) LocalGameManager.Instance.inventoryContainer.Remove(quest.info.itemDelete, quest.info.itemDeleteAmount);
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

        //Debug.Log($"Loading quests. Found {allQuests.Length} quests.");

        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (questInfo != null)
            {
                //Debug.Log($"Processing quest: {questInfo.name} with ID: {questInfo.id}");
                if (!string.IsNullOrEmpty(questInfo.id))
                {
                    idToQuestMap.Add(questInfo.id, new Quest(questInfo));
                }
            }
        }

        return idToQuestMap;
    }

    private Dictionary<Quest, int> CreateRepeatableQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<Quest, int> questMap = new();

        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (questInfo != null && questInfo is QuestInfoSORepeatable)
            {
                if (!string.IsNullOrEmpty(questInfo.id))
                {
                    questMap.Add(GetQuestById(questInfo.id), 0);
                }
            }
        }

        return questMap;
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

    public Quest GetQuestById(int uid)
    {
        if (!questMap.Values.Any(quest => quest.info.uid == uid))
        {
            Debug.LogError($"Quest with ID '{uid}' not found in questMap. Available quests: {string.Join(", ", questMap.Keys)}");
            return null;
        }

        return questMap.Values.First(quest => quest.info.uid == uid);
    }

    public bool CheckQuestStartedById(string id, List<QuestState> questStates = null)
    {
        Quest quest = questMap[id];
        return questStates == null || questStates.Contains(quest.state);
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
            GameEventsManager.Instance.questEvents.QuestStateChange(quest);
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

    public bool IsQuestRepeatable(Quest quest)
    {
        return quest.info is QuestInfoSORepeatable;
    }

    public int RepeatableQuestCount(Quest quest)
    {
        if (IsQuestRepeatable(quest)) return questRepeatedMap[quest];
        else
        {
            Debug.Log("quest manager: asked for repeated quest count but the quest is NOT repeatable");
            return -1;
        }
    }

    public async Task API_DisableCompletedQuests()
    {
        bool isTutorialCompleted = false;
        var userQuestCompletions = await UserAndReferralApi.GetCompletedQuestsByUser(WalletConnectScript.connectedWalletAddress);
        foreach (var apiQuest in userQuestCompletions.questCompletions)
        {
            Quest quest = GetQuestById(int.Parse(apiQuest.questId));
            if (quest != null)
            {
                Debug.Log($"{quest.info.name} in API is set as done - setting to FINISHED state");
                quest.state = QuestState.FINISHED;

                if (IsQuestRepeatable(quest)) ChangeQuestState(quest.info.id, QuestState.CAN_START);

                if (quest.info.uid == 0) isTutorialCompleted = true;
            }
        }

        if (isTutorialCompleted == false) FindObjectOfType<TutorialManager>(true).gameObject.SetActive(true);
    }
}