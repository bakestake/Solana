using System;
using UnityEngine;

public class MiscEvents
{
    public event Action<Interact> onInteracted;
    public void Interacted(Interact interactGo)
    {
        if (onInteracted != null)
        {
            onInteracted(interactGo);
        }
    }

    public event Action<DialogueTrigger> onDialogueEnded;
    public void DialogueEnded(DialogueTrigger dialogueTrigger)
    {
        if (onDialogueEnded != null)
        {
            onDialogueEnded(dialogueTrigger);
        }
    }
}

