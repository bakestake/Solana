using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bakeland;
using Gamegaard.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RamenDeliveryQuestStep : QuestStep
{
    private const float MAX_TIME = 60f;

    [SerializeField] private float maxTime;
    [SerializeField] private Transform npcQuestIcon;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Item ramenItem;
    [SerializeField] private int rewardPerDelivery;
    [SerializeField] private List<RuntimeAnimatorController> npcAnimators;
    [SerializeField] private AudioClip onDeliverySfx;
    [SerializeField] private AudioClip soundtrack;

    [Header("UI")]
    [SerializeField] private RectTransform hud;
    [SerializeField] private TMP_Text uiTimerText;
    [SerializeField] private UIShake uiTimerShake;
    [SerializeField] private RectTransform uiWaypointArrow;
    [SerializeField] private RectTransform uiDeliveryStamp;
    [SerializeField] private Animator uiDeliveryStampAnimator;

    [Header("Easter Egg")]
    [SerializeField] private bool isEasterEggEnabled;
    [SerializeField, Range(0f, 1f)] private float easterEggChance;
    [SerializeField] private RuntimeAnimatorController easterEggAnimator;
    [SerializeField] private Dialogue easterEggDialogue;

    private List<Interact> npcs;
    private bool isDelivered;
    private int timesDelivered;
    private Interact targetNpc;
    private bool isFinished;

    private float currentTime;
    private Coroutine timer;

    private const float uiArrowPosOffset = 30f;
    private const float uiArrowAngleOffset = 45f;
    private const float uiArrowSineSpeed = 5f;
    private const float uiArrowSineMagnitude = 5f;

    #region INITIALIZATION
    private void Awake()
    {
        npcs = new(GetComponentsInChildren<Interact>());
    }

    private IEnumerator Start()
    {
        PlayerController.canMove = false;
        PlayerController.canInteract = false;
        LoadingWheel.instance.EnableLoading();
        yield return new WaitForSeconds(1f);

        // load obstacles
        yield return StartCoroutine(LoadObstacles());

        LoadingWheel.instance.DisableLoading();

        // teleport player outside
        yield return StartCoroutine(TeleportPlayerOutside());

        // add permanent ramen item
        LocalGameManager.Instance.inventoryContainer.Add(ramenItem);

        // setup first delivery
        SetupDelivery();

        SpawnHUD();
        PlayerController.canMove = true;
        PlayerController.canInteract = true;
        SoundManager.Instance.ChangeMusic(soundtrack, persistent: true);
    }

    private IEnumerator LoadObstacles()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("RamenDeliveryObstacles", LoadSceneMode.Additive);
        while (!loading.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator UnloadObstacles()
    {
        AsyncOperation unloading = SceneManager.UnloadSceneAsync("RamenDeliveryObstacles");
        while (!unloading.isDone)
        {
            yield return null;
        }
    }

    // function to help performance when loading the obstacles scene
    // implement if obstacle loading is causing too much lag
    /* private IEnumerator OnObstaclesLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == "RamenDeliveryObstacles")
        {
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                obj.SetActive(true);
                yield return null;
            }
        }
    } 
    */

    private IEnumerator TeleportPlayerOutside()
    {
        var sceneLoader = FindObjectOfType<SceneLoader>();
        sceneLoader.Legacy_UnloadSceneInterior("RamenShop");
        sceneLoader.DisableReverb();

        // async await
        bool actionTriggered = false;
        void listener() => actionTriggered = true;
        LoadingWheel.instance.onFadeFinished += listener;

        yield return new WaitUntil(() => actionTriggered);

        LoadingWheel.instance.onFadeFinished -= listener;
    }
    #endregion

    #region UNITY CALLBACKS
    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.OnInteracted += OnInteracted;
        GameEventsManager.Instance.miscEvents.OnDialogueEnded += OnDialogueEnded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.OnInteracted -= OnInteracted;
        GameEventsManager.Instance.miscEvents.OnDialogueEnded -= OnDialogueEnded;
    }

    private void OnDestroy()
    {
        if (hud != null) Destroy(hud.gameObject);
    }

    private void Update()
    {
#if UNITY_EDITOR
        // for quick debugging
        if (Input.GetKeyDown(KeyCode.F1)) currentTime = 1f;
        if (Input.GetKeyDown(KeyCode.F2)) PlayerController.Instance.transform.position = targetNpc.transform.position + Vector3.down;
        if (Input.GetKeyDown(KeyCode.F3)) PlayerController.Instance.transform.position = GameObject.Find("SceneLoader [Ramen]").transform.position + Vector3.down;
#endif

        // waypoint arrow logic
        if (uiWaypointArrow != null && targetNpc != null)
        {
            var direction = (PlayerController.Instance.transform.position - targetNpc.transform.position).normalized;
            var offset = direction * (uiArrowPosOffset + Mathf.Sin(Time.time * uiArrowSineSpeed) * uiArrowSineMagnitude);
            var angle = uiArrowAngleOffset + (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            uiWaypointArrow.anchoredPosition = -offset;
            uiWaypointArrow.localEulerAngles = new Vector3(0f, 0f, angle);
        }
    }
    #endregion

    #region CUSTOM CALLBACKS
    private void OnInteracted(Interact interact)
    {
        // better to check here so the timer stops just as the user interacts with the NPC
        // preventing the timer running out while you are still in dialogue
        if (interact == targetNpc)
        {
            RamenDelivered(interact);
        }
    }

    private void OnDialogueEnded(DialogueTrigger trigger)
    {
        // override default quest rewards to reward gold based on total deliveries
        if (isFinished && trigger.name.Contains("BingBong"))
        {
            PlayerCurrency.instance.AddGold(timesDelivered * rewardPerDelivery);
            UserAndReferralApi.AddUserScore(WalletConnectScript.connectedWalletAddress, "1", timesDelivered);
        }
        // restart the quest logic to generate a new target NPC
        else if (isDelivered) SetupDelivery();
    }
    #endregion

    #region CLASS METHODS
    private void SpawnHUD()
    {
        hud.SetParent(GameObject.Find("HUD").transform);

        // hide delivery stamp at start
        uiDeliveryStamp.localScale = Vector3.zero;

        // reset transform properties to adapt to the canvas
        hud.anchorMin = Vector2.zero;
        hud.anchorMax = Vector2.one;
        hud.offsetMin = Vector2.zero;
        hud.offsetMax = Vector2.zero;
        hud.anchoredPosition = Vector2.zero;
        hud.localScale = Vector3.one;
    }

    public void SetupDelivery(bool startTimer = true)
    {
        isDelivered = false;

        if (startTimer) StartTimer();

        foreach (Interact npc in npcs) npc.enabled = false;
        targetNpc = GetNextNPC(timesDelivered);
        targetNpc.enabled = true;

        // easter egg
        bool useEasterEgg = isEasterEggEnabled && Random.Range(0f, 1f) < easterEggChance;

        targetNpc.GetComponent<DialogueTrigger>().LoadDialogue(useEasterEgg ? easterEggDialogue : dialogue);
        targetNpc.GetComponent<Animator>().runtimeAnimatorController = useEasterEgg ? easterEggAnimator : npcAnimators.GetRandom();
        npcQuestIcon.SetParent(targetNpc.transform);
        npcQuestIcon.localPosition = new Vector3(0f, 1f, 0f);

        UpdateState();
    }

    private Interact GetNextNPC(int timesDelivered)
    {
        // order npcs by distance to player
        var npcsByDistance = npcs.OrderBy(x => Vector2.Distance(PlayerController.Instance.transform.position, x.transform.position)).ToArray();

        // map difficulty to fixed values based on deliveries
        int difficulty =
            timesDelivered > 6 ? 6 :
            timesDelivered > 3 ? 3 :
            0;

        // setup array range based on difficulty * distance
        // for example, after the player finishes X deliveries, closer NPCs are no longer valid but the ones farther away are
        Dictionary<int, DifficultyRange> difficultyRanges = new()
        {
            { 0, new DifficultyRange {min = 0, max = 3 } },
            { 3, new DifficultyRange {min = 3, max = 6 } },
            { 6, new DifficultyRange {min = 6, max = npcs.Count} }
        };

        // prevent the last NPC from being a target
        Interact nextNpc = null;
        while (nextNpc == null || nextNpc == targetNpc)
            nextNpc = npcsByDistance[Random.Range(difficultyRanges[difficulty].min, difficultyRanges[difficulty].max)];

        return nextNpc;
    }

    // struct to be used with GetNextNPC
    public struct DifficultyRange
    {
        public int min, max;
    }

    private void StartTimer()
    {
        currentTime = Mathf.Clamp(maxTime - (timesDelivered * 3), 15, 60);
        if (timer != null) StopCoroutine(timer);
        timer = StartCoroutine(Timer());
    }

    private void StopTimer()
    {
        if (timer != null) StopCoroutine(timer);
    }

    private IEnumerator Timer()
    {
        var shakeTimer = .8f;
        while (currentTime >= 0f)
        {
            currentTime -= Time.deltaTime;
            uiTimerText.text = currentTime.ToString("00:00");

            shakeTimer += Time.deltaTime;
            if (shakeTimer >= 1f)
            {
                shakeTimer = 0f;
                // calculate the shake intensity based on remaining time * multiples of 15
                var shakeIntensityMultiplier = Mathf.InverseLerp(MAX_TIME - 15f, 0f, Mathf.Floor(currentTime / 15) * 15);
                uiTimerShake.Shake(uiTimerShake.duration, uiTimerShake.intensity * shakeIntensityMultiplier);
            }

            yield return null;
        }

        StartCoroutine(RanOutOfTime());
    }

    private void RamenDelivered(Interact npc)
    {
        // TO-DO: remove npc from possible npcs
        isDelivered = true;
        timesDelivered++;
        StopTimer();

        // play delivery stamp anim
        if (!uiDeliveryStamp.gameObject.activeInHierarchy) uiDeliveryStamp.gameObject.SetActive(true);
        uiDeliveryStamp.localEulerAngles = new Vector3(0f, 0f, Random.Range(-15f, 15f));
        uiDeliveryStampAnimator.SetTrigger("play");
        SoundManager.Instance.PlaySfx(onDeliverySfx);
    }

    private IEnumerator RanOutOfTime()
    {
        // reset everything
        isFinished = true;
        targetNpc = null;
        StopTimer();
        LocalGameManager.Instance.inventoryContainer.Remove(ramenItem);
        SoundManager.Instance.StopMusic(stopPersistent: true);
        if (hud != null) Destroy(hud.gameObject);
        if (npcQuestIcon != null) Destroy(npcQuestIcon.gameObject);

        // add loading wheel if needed?
        yield return UnloadObstacles();

        string state = $"<s>Deliveries are done</s>\nTOTAL DELIVERIES: {timesDelivered}\n";
        string status = $"<s>Deliveries are done</s>\nTOTAL DELIVERIES: {timesDelivered}\n";
        ChangeState(state, status);
        QuestManager.Instance.ChangeQuestState(questId, QuestState.CAN_FINISH);
    }

    private void UpdateState()
    {
        // updates text and quest state to show the next target
        string state = $"<b>TOTAL DELIVERIES: {timesDelivered}</b>\nDeliver ramen to client {targetNpc.name}";
        string status = $"<b>TOTAL DELIVERIES: {timesDelivered}</b>\nDeliver ramen to client {targetNpc.name}";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {

    }
    #endregion
}