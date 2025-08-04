using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

//This class Controls the lifecycle of the Player
public class Mult_PlayerController : NetworkBehaviour
{
    //Variables for Assigning Character
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator _animator;

    [HideInInspector]
    [Networked] public int characterIndex { get; set; }

    private ChangeDetector _changeDetector;


    private Mult_PlayerMovementController movementController = null;
    private Mult_PlayerDataNetworked dataNetworked;
    private GameObject overlappingPlayer;

    public override void Spawned()
    {
        movementController = GetComponent<Mult_PlayerMovementController>();
        dataNetworked = GetComponent<Mult_PlayerDataNetworked>();
        if (Object.HasInputAuthority)
        {
            var Index = FindObjectOfType<Mult_playerData>().GetCharacter();
            RpcSetIndex(Index);

            NotifyCamera();
        }

        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        UpdateCharacterData();
    }

    public override void Render()
    {
        if (_changeDetector == null)
        {
            Debug.Log("Change Detector is null");
        }
        foreach (var change in _changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(characterIndex):
                    UpdateCharacterData();
                    break;
            }
        }
    }

    private void UpdateCharacterData()
    {
        var characterData = ServerGameManager.Instance.GetCharacterData(characterIndex);
        if (characterData != null)
        {
            ApplyCharacterData(characterData);
        }
    }

    private void ApplyCharacterData(CharacterSelectorChar data)
    {
        DebugUtils.Log("Values of Data: " + data.characterName);
        spriteRenderer.sprite = data.front;
        _animator.runtimeAnimatorController = data.animatorOverrideController;
        movementController.SetAnimator(_animator);
    }

    public void SetVariables()
    {
        if (_animator != null)
        {
            movementController.SetAnimator(_animator);
        }
        else
        {
            DebugUtils.Log("_animator is null");
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Mult_LocalcameraHandler cameraManager = FindObjectOfType<Mult_LocalcameraHandler>();
        if (cameraManager != null)
        {
            cameraManager.SetCameraManager(null);
        }

        if (!runner.IsShutdown)
        {
            //Audio player death track
        }
    }

    private void NotifyCamera()
    {
        Mult_LocalcameraHandler cameraManager = FindObjectOfType<Mult_LocalcameraHandler>();
        if (cameraManager != null)
        {
            cameraManager.SetCameraManager(gameObject.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is a player
        if (collision.CompareTag("Player"))
        {
            overlappingPlayer = collision.gameObject;

            if (Object.HasInputAuthority)
            {
                DebugUtils.Log($"Overlapping with {overlappingPlayer.name}");
                Mult_PlayerDataNetworked playerScript = collision.GetComponent<Mult_PlayerDataNetworked>();
                NetworkObject playerNetObj = collision.GetComponent<NetworkObject>();
                if (dataNetworked && playerScript && playerNetObj)
                {
                    dataNetworked.AddToConsumePanel(playerNetObj.InputAuthority, playerScript.NickName.ToString(), playerScript.localPeerID.ToString());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the exiting object is a player
        if (collision.CompareTag("Player") && collision.gameObject == overlappingPlayer)
        {
            overlappingPlayer = null;

            if (Object.HasInputAuthority)
            {
                DebugUtils.Log($"No longer overlapping with any player.");
                NetworkObject playerNetObj = collision.GetComponent<NetworkObject>();
                if (dataNetworked && playerNetObj)
                {
                    dataNetworked.RemoveFromConsume(playerNetObj.InputAuthority);
                }
            }
        }
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetIndex(int selectedIndex)
    {
        characterIndex = selectedIndex;
    }
}
