using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    private int coinsCollected = 0;
    public int coinsToComplete = 200;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.goldEvents.onGoldChange += GoldCollected;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.goldEvents.onGoldChange -= GoldCollected;
    }

    private void GoldCollected(int gold)
    {
        if (coinsCollected < coinsToComplete)
        {
            coinsCollected += gold;
            // coinsCollected++;
            UpdateState();
        }

        if (coinsCollected >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = coinsCollected.ToString();
        string status = "Collected " + coinsCollected + " / " + coinsToComplete + " coins.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        this.coinsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
