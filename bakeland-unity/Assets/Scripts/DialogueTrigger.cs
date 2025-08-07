using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue defaultDialogue;
    public UnityEvent endEvent;

    private bool isRandom;
    private Message[] messages;
    private Actor[] actors;

    private void Start()
    {
        ResetDialogue();
    }

    public void LoadDialogue(Dialogue dialogue)
    {
        if (dialogue != null)
        {
            isRandom = dialogue.isRandom;
            messages = dialogue.messages;
            actors = dialogue.actors;
        }
        else
        {
            ResetDialogue();
        }
    }

    public void ResetDialogue()
    {
        if (defaultDialogue != null)
        {
            isRandom = defaultDialogue.isRandom;
            messages = defaultDialogue.messages;
            actors = defaultDialogue.actors;
        }
    }

    public void StartDialogue()
    {
        DialogueManager.instance.OpenDialogue(this, messages, actors, isRandom);
    }

    public void EndDialogue()
    {
        GameEventsManager.instance.miscEvents.DialogueEnded(this);
        endEvent?.Invoke();
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