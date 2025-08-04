using System.Collections;
using System.Collections.Generic;
using Bakeland;
using Gamegaard.Utils;
using UnityEngine;

public class GottaGoPlacesStep : QuestStep
{
    [SerializeField] private Transform npc;
    [SerializeField] private DialogueTrigger npcDialogueTrigger;
    [SerializeField] private List<Transform> npcLocations;
    [SerializeField] private List<Dialogue> npcDialogues;
    [SerializeField] private List<string> questStatesText;
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private AudioClip smokeClip;
    [SerializeField] private RectTransform hud;
    [SerializeField] private RectTransform uiWaypointArrow;

    [Header("Tutorial info")]
    [SerializeField] TutorialPanel.TutorialPanelParams[] farmTutorialPanels;
    [SerializeField] TutorialPanel.TutorialPanelParams[] shopTutorialPanels;

    private const float uiArrowPosOffset = 30f;
    private const float uiArrowAngleOffset = 45f;
    private const float uiArrowSineSpeed = 5f;
    private const float uiArrowSineMagnitude = 5f;

    private int currentStep = -1;
    private string currentState = "Talk to Puff the Bear", currentStatus = "Talk to Puff the Bear";

    #region UNITY CALLBACKS
    private void Start()
    {
        NPCManager.GetNPC("Puff the Bear").SetActive(false);

        SpawnHUD();
        NextStep();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded += OnDialogueEnded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded -= OnDialogueEnded;
    }

    private void Update()
    {
#if UNITY_EDITOR
        // for quick debugging
        if (Input.GetKeyDown(KeyCode.F1)) PlayerController.Instance.transform.position = npc.position + Vector3.down;
#endif
        // waypoint arrow logic
        if (uiWaypointArrow != null && npc != null)
        {
            var direction = (PlayerController.Instance.transform.position - npc.transform.position).normalized;
            var offset = direction * (uiArrowPosOffset + Mathf.Sin(Time.time * uiArrowSineSpeed) * uiArrowSineMagnitude);
            var angle = uiArrowAngleOffset + (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            uiWaypointArrow.anchoredPosition = -offset;
            uiWaypointArrow.localEulerAngles = new Vector3(0f, 0f, angle);
        }
    }
    #endregion

    #region CUSTOM CALLBACKS
    private void OnDialogueEnded(DialogueTrigger trigger)
    {
        if (trigger == npcDialogueTrigger) NextStep();
    }
    #endregion

    #region CLASS METHODS
    private void SpawnHUD()
    {
        hud.SetParent(GameObject.Find("HUD").transform);

        // reset transform properties to adapt to the canvas
        hud.anchorMin = Vector2.zero;
        hud.anchorMax = Vector2.one;
        hud.offsetMin = Vector2.zero;
        hud.offsetMax = Vector2.zero;
        hud.anchoredPosition = Vector2.zero;
        hud.localScale = Vector3.one;
    }

    private void NextStep()
    {
        currentStep++;
        bool finalStep = true;

        if (currentStep == 0) ChangeState(currentState, currentStatus);
        else if (currentStep == 2) TutorialPopUp.Instance.GeneratePanels(farmTutorialPanels);
        else if (currentStep == 3) TutorialPopUp.Instance.GeneratePanels(shopTutorialPanels);

        if (currentStep < npcLocations.Count)
        {
            SpawnSmokeParticles(npc.position);
            npc.position = npcLocations[currentStep].position;
            finalStep = false;
        }

        if (currentStep < npcDialogues.Count)
        {
            npcDialogueTrigger.LoadDialogue(npcDialogues[currentStep]);
            finalStep = false;
        }

        if (currentStep < questStatesText.Count && currentStep > 0)
        {
            UpdateState(questStatesText[currentStep]);
            finalStep = false;
        }

        if (finalStep)
        {
            StartCoroutine(FinalStep());
        }
    }

    private IEnumerator FinalStep()
    {
        SpawnSmokeParticles(npc.position);
        npc.gameObject.SetActive(false);
        if (hud != null) Destroy(hud.gameObject);

        yield return new WaitForSeconds(3f);

        ChangeState($"<s>{currentState}\n", $"<s>{currentState}\n");
        FinishQuest();
    }

    private void SpawnSmokeParticles(Vector3 position)
    {
        smokeParticles.Emit(new ParticleSystem.EmitParams() { position = position }, 25);
        SoundManager.Instance.PlaySfx(smokeClip);
    }

    private void UpdateState(string textToAdd)
    {
        // show previous states with strikethrough but new one without it
        ChangeState($"<s>{currentState}</s>\n{textToAdd}", $"<s>{currentStatus}</s>\n{textToAdd}");

        // add the new step to cache
        currentState += "\n" + textToAdd;
        currentStatus += "\n" + textToAdd;
    }

    protected override void SetQuestStepState(string state)
    {

    }
    #endregion
}