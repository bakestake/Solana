using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Febucci.UI;
using TMPro;
using UnityEngine.UI;

public class DialogueManagerMultiplayer : MonoBehaviour
{
    public static DialogueManagerMultiplayer instance;

    public Image actorImage;
    public TextMeshProUGUI actorName;
    public TextMeshProUGUI messageText;
    public RectTransform backgroundBox;
    public Animator animator;
    public TypewriterByCharacter typewriter;
    public CanvasGroup dialogueSkip;

    DialogueTrigger currentDialogue;
    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive = false;
    private bool canSkip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        activeMessage = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isActive)
        {
            if (!canSkip)
            {
                typewriter.SkipTypewriter();
                canSkip = true;
                dialogueSkip.alpha = 1;
            }
            else
            {
                NextMessage();
                canSkip = false;
                dialogueSkip.alpha = 0;
            }
        }

        if (canSkip)
        {
            dialogueSkip.alpha = 1;
        }
        else
        {
            dialogueSkip.alpha = 0;
        }
    }

    public void TextShown()
    {
        canSkip = true;
    }

    public void OpenDialogue(DialogueTrigger dialogue, Message[] messages, Actor[] actors, bool isRandom)
    {
        canSkip = false;
        isActive = true;
        currentDialogue = dialogue;
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;

        PlayerController.canMove = false;

        animator.SetTrigger("in");

        if (isRandom)
        {
            DisplayRandomMessage();
        }
        else
        {
            DisplayMessage();
        }
    }

    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        messageText.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.Name;
        actorName.color = actorToDisplay.color;
        // actorImage.sprite = actorToDisplay.sprite;
        SoundManager.instance.PlayRandomFromList(SoundManager.instance.popSounds);
    }

    void DisplayRandomMessage()
    {
        int rand = Random.Range(0, currentMessages.Length);
        Message messageToDisplay = currentMessages[rand];
        messageText.text = messageToDisplay.message;

        Actor actorToDisplay = currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.Name;
        actorName.color = actorToDisplay.color;
        // actorImage.sprite = actorToDisplay.sprite;

        activeMessage = currentMessages.Length;

        SoundManager.instance.PlayRandomFromList(SoundManager.instance.popSounds);
    }

    public void NextMessage()
    {
        activeMessage++;
        Debug.Log("Next message");

        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            isActive = false;
            PlayerController.canMove = true;
            Debug.Log("Can Move true");
            animator.SetTrigger("out");
            currentDialogue.EndDialogue();
            activeMessage = 0;
            SoundManager.instance.PlaySfx(SoundManager.instance.dialogueOut);
        }
    }
}
