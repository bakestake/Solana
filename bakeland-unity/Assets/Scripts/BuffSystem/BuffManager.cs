using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;
    public List<BuffInstance> activeBuffs = new List<BuffInstance>();  // List of active buffs
    public Transform buffUIParent;                                     // UI parent for the buff icons
    public GameObject buffUIPrefab;                                    // Prefab for each buff's UI element
    public float buffUIJumpAmount = 1.05f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        UpdateBuffs(Time.deltaTime);
    }

    // Adds a new buff, or extends the duration if already applied
    public void AddBuff(Buff newBuff)
    {
        BuffInstance existingBuff = activeBuffs.Find(b => b.buff == newBuff);

        if (existingBuff != null)
        {
            existingBuff.ExtendDuration();
            existingBuff.uiElement.transform.DOComplete();
            existingBuff.uiElement.transform.DOPunchScale(Vector3.one * buffUIJumpAmount, .5f);
        }
        else
        {
            BuffInstance newBuffInstance = new BuffInstance(newBuff);
            activeBuffs.Add(newBuffInstance);
            newBuffInstance.Apply();  // Apply the effect
            CreateBuffUI(newBuffInstance);  // Create the UI element
        }
    }

    // Updates the timers and checks for expired buffs
    public void UpdateBuffs(float deltaTime)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            BuffInstance buffInstance = activeBuffs[i];
            buffInstance.UpdateDuration(deltaTime);

            if (buffInstance.IsExpired())
            {
                RemoveBuff(buffInstance);
            }
        }
    }

    // Removes a buff and cleans up
    public void RemoveBuff(BuffInstance buffInstance)
    {
        buffInstance.Expire();  // Remove effect
        activeBuffs.Remove(buffInstance);
        RemoveBuffUI(buffInstance);  // Remove UI element
    }

    private void CreateBuffUI(BuffInstance buffInstance)
    {
        GameObject buffUI = Instantiate(buffUIPrefab, buffUIParent);
        BuffUI buffUIScript = buffUI.GetComponent<BuffUI>();

        buffUI.transform.DOComplete();
        buffUI.transform.DOPunchScale(Vector3.one * buffUIJumpAmount, .5f);

        // Initialize the UI element with the buff information
        buffUIScript.Initialize(buffInstance);

        // Link the UI element to the BuffInstance for future updates
        buffInstance.uiElement = buffUI;
    }

    private void RemoveBuffUI(BuffInstance buffInstance)
    {
        if (buffInstance.uiElement != null)
        {
            Destroy(buffInstance.uiElement);
            buffInstance.uiElement = null;

            // Hide tooltip if buff UI is being destroyed
            TooltipSystem tooltipSystem = FindObjectOfType<TooltipSystem>();
            tooltipSystem.HideTooltipIfActive(buffInstance.uiElement);  // Pass the buff UI prefab or instance to check
        }
    }


}
