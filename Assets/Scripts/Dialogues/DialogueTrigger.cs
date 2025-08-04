using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue defaultDialogue;
    [SerializeField] private UnityEvent startEvent;
    [SerializeField] private UnityEvent endEvent;

    private bool isRandom;
    private bool advancePerInteraction;
    private Message[] messages;
    private Actor[] actors;
    private int currentMessageIndex = 0;

    public Dialogue DefaultDialogue => defaultDialogue;
    public Actor[] Actors => actors;
    public int CurrentMessageIndex
    {
        get => currentMessageIndex;
        set => currentMessageIndex = value;
    }

    public bool HasReachedEnd => currentMessageIndex >= messages.Length - 1;

    private void Awake()
    {
        ResetDialogue();
    }

    public void SetDialogue(Dialogue dialogue)
    {
        defaultDialogue = dialogue;
    }

    public void LoadDialogue(Dialogue dialogue)
    {
        if (dialogue != null)
        {
            isRandom = dialogue.isRandom;
            advancePerInteraction = dialogue.advancePerInteraction;
            messages = dialogue.messages;
            actors = dialogue.actors;
        }
        else
        {
            //Debug.Log(name + ":load dialogue is null");
            ResetDialogue();
        }
    }

    public void ResetDialogue()
    {
        if (defaultDialogue != null)
        {
            //Debug.Log(name + ":reset dialogue");
            isRandom = defaultDialogue.isRandom;
            advancePerInteraction = defaultDialogue.advancePerInteraction;
            messages = defaultDialogue.messages;
            actors = defaultDialogue.actors;
        }
    }

    public void StartDialogue()
    {
        DialogueManager.Instance.OpenDialogue(this, messages, actors, isRandom, advancePerInteraction);
        startEvent?.Invoke();
    }

    public void EndDialogue()
    {
        GameEventsManager.Instance.miscEvents.DialogueEnded(this);
        endEvent?.Invoke();
    }

    public Message GetCurrentMessage()
    {
        if (currentMessageIndex < messages.Length)
            return messages[currentMessageIndex];
        else
            return messages[^1];
    }
}

// [System.Serializable]
// public class Message
// {
//     public int actorId;
//     [TextArea(3, 7)]
//     public string message;
// }
// [System.Serializable]
// public class Actor
// {
//     public string Name;
//     public Sprite sprite;
// }