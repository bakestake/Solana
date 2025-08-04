using System;
using Febucci.UI;
using Gamegaard.Singleton;
using Gamegaard.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    [SerializeField] private Image actorImage;
    [SerializeField] private TextMeshProUGUI actorName;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private RectTransform backgroundBox;
    [SerializeField] private Animator animator;
    [SerializeField] private TypewriterByCharacter typewriter;
    [SerializeField] private CanvasGroup dialogueSkip;
    [SerializeField] private AudioClip audioClip;

    private DialogueTrigger currentDialogue;
    private Message[] currentMessages;
    private Actor[] currentActors;
    private int activeMessage = 0;
    private bool canSkip;
    private bool advancePerInteraction;
    public static bool isActive;

    protected override void Awake()
    {
        base.Awake();
        typewriter.onTextShowed.AddListener(TextShown);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (typewriter == null) return;
        typewriter.onTextShowed.RemoveListener(TextShown);
    }

    private void OnEnable()
    {
        typewriter.onCharacterVisible.AddListener(OnCharacterWritten);
    }

    private void OnDisable()
    {
        typewriter.onCharacterVisible.RemoveListener(OnCharacterWritten);
    }

    private void Update()
    {
        dialogueSkip.alpha = canSkip ? 1 : 0;
    }

    public void TryInteract()
    {
        if (!isActive) return;

        if (typewriter.isShowingText || !canSkip)
        {
            typewriter.SkipTypewriter();
            canSkip = true;
            dialogueSkip.alpha = 1;
        }
        else if (canSkip)
        {
            NextMessage();
            canSkip = false;
            dialogueSkip.alpha = 0;
        }
    }

    public void TextShown()
    {
        canSkip = true;
    }

    private void OnCharacterWritten(char arg0)
    {
        if (!isActive || SoundManager.Instance == null) return;

        SoundManager.Instance.PlaySfx(audioClip);
    }

    public void OpenDialogue(DialogueTrigger dialogue, Message[] messages, Actor[] actors, bool isRandom, bool advancePerInteraction)
    {
        canSkip = false;
        isActive = true;
        currentDialogue = dialogue;
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        this.advancePerInteraction = advancePerInteraction;

        PlayerController.canMove = false;

        animator.SetTrigger("in");

        if (advancePerInteraction)
        {
            ShowMessage(dialogue.GetCurrentMessage());
        }
        else if (isRandom)
        {
            DisplayRandomMessage();
        }
        else
        {
            DisplayMessage();
        }
    }

    private void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        ShowMessage(currentMessages[activeMessage]);
    }

    private void DisplayRandomMessage()
    {
        Message messageToDisplay = currentMessages.GetRandom();
        activeMessage = currentMessages.Length;
        ShowMessage(messageToDisplay);
    }

    private void ShowMessage(Message messageToDisplay)
    {
        messageText.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.Name;
        actorName.color = actorToDisplay.color;
        actorImage.sprite = actorToDisplay.sprite;
        actorImage.gameObject.SetActive(actorToDisplay.HasSprite);
        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.popSounds);
    }

    public void NextMessage()
    {
        if (advancePerInteraction)
        {
            if (!currentDialogue.HasReachedEnd)
                currentDialogue.CurrentMessageIndex++;

            Close();
            return;
        }

        activeMessage++;

        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            Close();
        }
    }

    private void Close()
    {
        isActive = false;
        PlayerController.canMove = true;
        animator.SetTrigger("out");
        currentDialogue.EndDialogue();
        SoundManager.Instance.PlaySfx(SoundManager.Instance.dialogueOut);
    }
}