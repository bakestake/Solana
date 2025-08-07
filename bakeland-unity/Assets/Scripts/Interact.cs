using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Interact : MonoBehaviour
{
    public bool canHighlight = true;
    public bool isHighlighted;

    [Header("Item Check")]
    public bool itemCheck = false;
    public Item itemToCheck;
    public int itemAmount;
    public Dialogue checkSuccessDialogue;
    public Dialogue checkFailDialogue;

    [Header("Quest Check")]
    public bool questCheck = false;

    [Header("Other")]
    public KeyCode interactKey = KeyCode.Space;
    public AudioClip sfx;
    public UnityEvent interactEvent;
    public UnityEvent interactFailEvent;

    [Header("Indicator")] 
    public GameObject interactIndicator;

    Collider2D col2D;

    private void Awake()
    {
        col2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey) && isHighlighted)
        {
            OnInteract();
        }
    }

    private void OnInteract()
    {
        if (itemCheck) { ItemCheck(); }
        if (questCheck)
        {
            QuestChecker questChecker = GetComponent<QuestChecker>();
            if (questChecker != null)
            {
                bool canInteract = questChecker.CheckQuest();

                if (!canInteract)
                {
                    interactFailEvent?.Invoke();
                    RemoveHighlight();
                    return;
                }
            }
        }
        GameEventsManager.instance.miscEvents.Interacted(this);
        interactEvent?.Invoke();
        if (sfx != null)
        {
            SoundManager.instance.PlaySfx(sfx);
        }
        RemoveHighlight();
    }

    private void Highlight()
    {
        Material mat = GetComponent<SpriteRenderer>().material;
        mat.SetFloat("_Active", 1);
        isHighlighted = true;

        if (interactIndicator && !interactIndicator.activeSelf)
        {
            interactIndicator.SetActive(true);
        }
    }

    private void RemoveHighlight()
    {
        Material mat = GetComponent<SpriteRenderer>().material;
        mat.SetFloat("_Active", 0);
        isHighlighted = false;

        if (interactIndicator && interactIndicator.activeSelf)
        {
            interactIndicator.SetActive(false);
        }
    }

    private void ItemCheck()
    {
        DialogueTrigger dialogueTrigger = GetComponent<DialogueTrigger>();
        int totalItem = 0;

        foreach (ItemSlot slot in LocalGameManager.Instance.inventoryContainer.slots)
        {
            if (slot.item == itemToCheck)
            {
                totalItem += slot.count;

                if (slot.item.unique)
                {
                    totalItem = 99;
                }
            }


        }
        if (totalItem >= itemAmount)
        {
            if (dialogueTrigger != null)
            {
                dialogueTrigger.LoadDialogue(checkSuccessDialogue);
            }
        }
        else
        {
            if (dialogueTrigger != null)
            {
                dialogueTrigger.LoadDialogue(checkFailDialogue);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Highlight();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (canHighlight)
            {
                RemoveHighlight();
            }
        }
    }
}
