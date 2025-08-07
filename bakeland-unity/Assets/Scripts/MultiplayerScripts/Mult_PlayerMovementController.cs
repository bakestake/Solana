using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using Fusion.Addons.Physics;
using UnityEngine.UIElements;
using System;

//This class controls the Player's Movement
public class Mult_PlayerMovementController : NetworkBehaviour
{
    [Header("Components")]
    //Game Session Agnostic Settings
    [SerializeField] private const float maxFastSpeed = 10.0f;
    private float currentSpeed = 7.0f;
    //private Vector2 _calculatedVelocity;
    //private bool isWalking = false;

    // Local Runtime references
    private Rigidbody2D _rigidbody; // The Unity Rigidbody (RB) is automatically synchronised across the network thanks to the NetworkRigidbody (NRB) component.

    private Mult_PlayerVisualController _visualController = null;
    private Mult_PlayerDataNetworked _playerDataNetworked = null;
    private Mult_PlayerController _playerController = null;
    private NetworkTransform _networkTransform = null;

    //Variables which need to be replicated for movement of the player and animations
    [Networked] private Vector2 _calculatedVelocity { get; set; }
    [Networked] private bool isWalking { get; set; } = false;
    [Networked] private float SpeedMultiplier { get; set; } = 1.0f;
    [Networked] private NetworkBool IsTeleport { get; set; } = false;
    [Networked] private Vector2 TeleportPosition { get; set; } = Vector2.zero;
    [Networked] private NetworkButtons _buttonsPrevious { get; set; }

    private Animator _animator;


    public override void Spawned()
    {
        // --- Host & Client
        // Set the local runtime references.
        LoadingWheel.instance.DisableLoading();
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerDataNetworked = GetComponent<Mult_PlayerDataNetworked>();
        _visualController = GetComponent<Mult_PlayerVisualController>();
        _playerController = GetComponent<Mult_PlayerController>();
        _networkTransform = GetComponent<NetworkTransform>();

        //Host
        //The GameSession Specific Session are Initialised
        if (Object.HasStateAuthority == false) return;
    }

    public void SetAnimator(Animator animator)
    {
        DebugUtils.Log("Animator is set");
        _animator = animator;
    }

    public override void FixedUpdateNetwork()
    {
        //if (IsTeleport)
        //{
        //    _rigidbody.Teleport(TeleportPosition);
        //    if (Object.HasInputAuthority)
        //    {
        //        RpcSetPosition(false, Vector2.zero);
        //    }
        //}
        // This will only return true on the Client with InputAuthority for this Object and the Host.
        if (GetInput(out Mult_NetworkInputData input))
        {
            Move(input);
        }
    }

    // This Move function is being executed on the server
    private void Move(Mult_NetworkInputData input)
    {
        Vector2 inputDirection = new Vector2(input.HorizontalInput, input.VerticalInput).normalized;
        Vector2 desiredVelocity = inputDirection * (currentSpeed * SpeedMultiplier);

        // Apply velocity to rigid Body
        _rigidbody.velocity = desiredVelocity;

        //For Fast Walking
        isWalking = _rigidbody.velocity.sqrMagnitude > 0.01f;
        _calculatedVelocity = desiredVelocity;

        if (input.Buttons.WasPressed(_buttonsPrevious, MultiplayerButtons.SpeedWalking))
        {
            SpeedMultiplier = 2.0f;
        }
        else if (input.Buttons.WasReleased(_buttonsPrevious, MultiplayerButtons.SpeedWalking))
        {
            SpeedMultiplier = 1.0f;
        }
        _buttonsPrevious = input.Buttons;
    }

    public override void Render()
    {
        _animator?.SetFloat("SpeedX", _calculatedVelocity.x);
        _animator?.SetFloat("SpeedY", _calculatedVelocity.y);
        _animator?.SetBool("IsWalking", isWalking);
    }

    public void PlayEffects()
    {
        if (isWalking)
        {
            SoundManager.instance.PlayRandomFromList(SoundManager.instance.footstepSounds);
            _visualController.TriggerDustParticle();
        }
    }

    public void PlayerFastTravel(Vector3 FastTravelPosition)
    {
        if (Runner.IsServer)
        {
            PerformFastTravel(FastTravelPosition);
        }
        else if (Object.HasInputAuthority)
        {
            RpcRequestFastTravel(FastTravelPosition);
        }
    }
    //Request RPC: This is sent by the client to the server to initiate fast travel.
    [Rpc(sources:RpcSources.InputAuthority, targets:RpcTargets.StateAuthority)]
    private void RpcRequestFastTravel(Vector3 fastTravelPosition)
    {
        PerformFastTravel(fastTravelPosition);
    }

    //Update RPC: This is sent by the server to all clients to synchronize the position.
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdateFastTravel(Vector3 fastTravelPosition)
    {
        if (!Object.HasStateAuthority)
        {
            _networkTransform.Teleport(fastTravelPosition); // Ensures consistent position
        }
    }

    private void PerformFastTravel(Vector3 fastTravelPosition)
    {
        _rigidbody.velocity = Vector2.zero;
        _networkTransform.Teleport(fastTravelPosition);
        //Notify other players of the fast Travel
        if (Runner.IsServer)
        {
            RpcUpdateFastTravel(fastTravelPosition);
        }
    }
}
