using Gamegaard.CustomValues;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Interact : MonoBehaviour
{
    [SerializeField] private bool canHighlight = true;

    [Header("Item Check")]
    [SerializeField] private bool itemCheck;
    [SerializeField] private bool continuousInteraction;
    [SerializeField] private Item itemToCheck;
    [SerializeField] private int itemAmount;
    [SerializeField] private Dialogue checkSuccessDialogue;
    [SerializeField] private Dialogue checkFailDialogue;

    [Header("Condition Check")]
    [SerializeField] private SerializableList<IInteractionCondition> conditions;
    [SerializeField] private Dialogue conditionFailDialogue;

    [Header("Quest Check")]
    [SerializeField] private bool questCheck;

    [Header("Other")]
    [SerializeField] private AudioClip sfx;
    [SerializeField] private UnityEvent interactEvent;
    [SerializeField] private UnityEvent interactFailEvent;

    public static Interact currentInteraction;
    private DialogueTrigger dialogueTrigger;
    private GameObject interactIndicator;
    private Material mat;
    private bool isHighlighted;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        mat = GetComponent<SpriteRenderer>().material;
        Transform interactIndicatorTransform = transform.Find("InteractIndicator");
        if (interactIndicatorTransform != null)
        {
            interactIndicator = interactIndicatorTransform.gameObject;
        }
    }

    private void OnEnable()
    {
        RemoveHighlight();
    }

    private void OnDisable()
    {
        ForceRemoveHighlight();
    }

#if UNITY_EDITOR
    [ContextMenu("Interact", true)]
    private bool ForceInteraction()
    {
        return UnityEditor.EditorApplication.isPlaying;
    }
#endif

    [ContextMenu("Interact")]
    public void TryInteract()
    {
        if (!enabled || !isHighlighted || !PlayerController.canInteract) return;

        if (!continuousInteraction)
        {
            RemoveHighlight();
        }

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

        foreach (IInteractionCondition condition in conditions)
        {
            if (!condition.CanInteract())
            {
                if (conditionFailDialogue != null && dialogueTrigger != null)
                {
                    dialogueTrigger.LoadDialogue(conditionFailDialogue);
                }

                dialogueTrigger.StartDialogue();
                return;
            }
        }

        foreach (IInteractionCondition condition in conditions)
        {
            condition.OnInteracted();
        }

        GameEventsManager.Instance.miscEvents.Interacted(this);
        interactEvent?.Invoke();
        if (sfx != null)
        {
            SoundManager.Instance.PlaySfx(sfx);
        }
    }

    private void Highlight()
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentInteraction != null && currentInteraction != this)
        {
            currentInteraction.RemoveHighlight();
        }

        currentInteraction = this;

        if (mat != null && mat.HasProperty("_FlashAmount") && this.enabled)
        {
            StartCoroutine(ChangeFlashAmount(mat, 0.5f));
        }

        if (interactIndicator != null && !interactIndicator.activeSelf && this.enabled)
        {
            interactIndicator.SetActive(true);
        }

        isHighlighted = true;
    }

    private void RemoveHighlight()
    {
        if (!gameObject.activeInHierarchy) return;
        InternalRemoveHighlight(useCoroutine: true);
    }

    private void ForceRemoveHighlight()
    {
        InternalRemoveHighlight(useCoroutine: false);
    }

    private void InternalRemoveHighlight(bool useCoroutine)
    {
        if (currentInteraction == this)
        {
            currentInteraction = null;
        }

        if (mat != null && mat.HasProperty("_FlashAmount"))
        {
            if (useCoroutine)
            {
                StartCoroutine(ChangeFlashAmount(mat, 0f));
            }
            else
            {
                mat.SetFloat("_FlashAmount", 0f);
            }
        }

        if (interactIndicator != null)
        {
            interactIndicator.SetActive(false);
        }

        isHighlighted = false;
    }


    private IEnumerator ChangeFlashAmount(Material mat, float targetValue)
    {
        float currentValue = mat.GetFloat("_FlashAmount");
        float duration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(currentValue, targetValue, elapsedTime / duration);
            mat.SetFloat("_FlashAmount", newValue);
            yield return null;
        }

        mat.SetFloat("_FlashAmount", targetValue);
    }

    private void ItemCheck()
    {
        int totalItem = 0;

        foreach (ItemSlot slot in LocalGameManager.Instance.inventoryContainer.slots)
        {
            if (slot.item == itemToCheck)
            {
                totalItem += slot.count;

                if (slot.item.Unique)
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