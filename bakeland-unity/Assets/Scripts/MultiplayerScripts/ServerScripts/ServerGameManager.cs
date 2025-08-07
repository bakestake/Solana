using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.Events;
using System.Threading.Tasks;
using Huddle01.Sample;

public class ServerGameManager : NetworkBehaviour
{
    public static ServerGameManager Instance { get; private set; }

    [HideInInspector]
    [Networked] public string RoomId { get; private set; } = "null";

    [HideInInspector]
    public string AccessToken;

    public CreateRoomHuddle CreateRoomHuddlePrefab;
    public GameObject audioMeetingExample;

    public Animator transitionAnimator;
    public Animator firstTimeSceneAnimator;
    public bool firstTime;
    public DialogueTrigger welcomeDialogue;

    [SerializeField] private List<CharacterSelectorChar> Characters = new List<CharacterSelectorChar>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGameManager()
    {
        CheckFirstTime();

        _ = HuddleRoomCreation();
    }

    private async Task HuddleRoomCreation()
    {
        if (Runner.IsServer)
        {
            DebugUtils.Log("This is the server, spawning the Create Huddle Room prefab");
            CreateRoomHuddle createRoomHuddle = Runner.Spawn(CreateRoomHuddlePrefab);
            if (createRoomHuddle != null)
            {
                //Create room in Huddle
                if (RoomId == "null")
                {
                    var (HuddleRoomId, HuddleMessage) = await createRoomHuddle.CreateRoom();
                    RoomId = HuddleRoomId;
                    if (string.Equals(HuddleMessage, "Room Created Successfully"))
                    {
                        DebugUtils.Log("The Huddle Room ID is: " + RoomId);

                    }
                    else
                    {
                        DebugUtils.Log("The Huddle RoomID: " + HuddleRoomId);
                        DebugUtils.Log("The Huddle Message: " + HuddleMessage);
                    }
                }
                else
                {
                    DebugUtils.Log("RoomId is Not null The Huddle Room ID is: " + RoomId);
                }
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Instance = null;
    }

    public void TransitionIn()
    {
        StartCoroutine(TransitionInRoutine());
    }

    private IEnumerator TransitionInRoutine()
    {
        transitionAnimator.SetTrigger("in");
        yield return new WaitForSeconds(0.5f);
    }

    public void TransitionOut()
    {
        StartCoroutine(TransitionOutRoutine());
    }

    private IEnumerator TransitionOutRoutine()
    {
        transitionAnimator.SetTrigger("out");
        yield return new WaitForSeconds(0.5f);
    }

    public void CheckFirstTime()
    {
        if (PlayerPrefs.HasKey("first"))
        {
            WelcomeDialogue();
        }
        else
        {
            PlayerPrefs.SetString("first", "1");
            FirstTimeSceneIn();
        }
    }

    public void WelcomeDialogue()
    {
        welcomeDialogue.StartDialogue();
    }

    public void FirstTimeSceneIn()
    {
        //PlayerController.canMove = false;
        firstTimeSceneAnimator.SetTrigger("in");
    }

    public void FirstTimeSceneOut()
    {
        firstTimeSceneAnimator.SetTrigger("out");
        //PlayerController.canMove = true;
    }

    public void SpawnHuddleAudioManager()
    {
        if (Runner.IsServer)
        {
            DebugUtils.Log("The Audio Meeting Example is Spawned");
            Runner.Spawn(audioMeetingExample);
        }
    }

    public CharacterSelectorChar GetCharacterData(int Index)
    {
        return Characters[Index];
    }
}
