using System;
using UnityEngine;

public class MiscEvents
{
    public event Action<Interact> OnInteracted;
    public void Interacted(Interact interact)
    {
        OnInteracted?.Invoke(interact);
    }

    public event Action<DialogueTrigger> OnDialogueEnded;
    public void DialogueEnded(DialogueTrigger dialogueTrigger)
    {
        OnDialogueEnded?.Invoke(dialogueTrigger);
    }
}