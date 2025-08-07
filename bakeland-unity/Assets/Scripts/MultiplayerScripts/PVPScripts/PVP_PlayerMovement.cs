using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using Fusion.Addons.Physics;

public class PVP_PlayerMovement : NetworkBehaviour
{
    [Header("MOVEMENT")]
    private bool FlipEvent = false;
    private float moveSpeed = 12f;
    private bool IsFacingRight = false;
    private float attackDuration = 1.0f;
    private float BlockDuration = 1.0f;
    private float UltimateDuration = 1.5f;

    [Networked]
    private bool isRunning { get; set; } = false;

    [Networked]
    private float horizontalInput { get; set; }

    [Networked]
    public bool IsAttacking { get; private set; } = false;

    [Networked]
    public bool IsBlocking { get; private set; } = false;

    [Networked]
    public bool IsUltimate { get; private set; } = false;

    [Networked]
    private TickTimer AttackCooldown { get; set; }

    [Header("COMPONENTS")]
    public Animator anim;
    private NetworkRigidbody2D rb = null;
    public Ultimate UltimateAttack = null;

    [Header("STATE CONTROL")]
    public PlayerState currentState = PlayerState.Idle;

    [Header("ULTIMATE")]
    public Text ultiText;

    [Header("COMBO")]
    public int attackAmount = 0;
    public float comboTimer = 4F;
    [Networked]
    public bool comboDamage { get; private set; } = false;

    [Header("UI")]
    public Text comboText;

    [Header("Jump Control")]
    public float jumpingPower = 20f;
    public int maxJumps = 1; // Maximum number of jumps allowed
    private int jumpsRemaining;

    public Transform groundCheck;
    public LayerMask groundLayer;

    private Damage_Player _damagePlayer = null;

    [Networked] private NetworkButtons _buttonsPrevious { get; set; }
    [Networked] public Vector3 Velocity { get; set; }

    public override void Spawned()
    {
        // --- Host & Client
        // Set the local runtime references.
        rb = GetComponent<NetworkRigidbody2D>();
        _damagePlayer = GetComponent<Damage_Player>();
        //UltimateAttack = GetComponent<Ultimate>();
        //_playerDataNetworked = GetComponent<Mult_PlayerDataNetworked>();
        //_visualController = GetComponent<Mult_PlayerVisualController>();
        //_playerController = GetComponent<Mult_PlayerController>();

        jumpsRemaining = maxJumps; // Initially grant jumps for the maximum number of jumps allowed

        //Host
        //The GameSession Specific Session are Initialised
        if (Object.HasStateAuthority == false) return;
    }

    public override void FixedUpdateNetwork()
    {
        // GetInput() can only be called from NetworkBehaviours.
        // In SimulationBehaviours, either TryGetInputForPlayer<T>() or GetInputForPlayer<T>() has to be called.
        // This will only return true on the Client with InputAuthority for this Object and the Host.
        if (GetInput(out Mult_NetworkInputData input))
        {
            Move(input);
        }

        switch (currentState)
        {
            case PlayerState.Idle:  //Idle State
                if (!trampoline.isRecoiling)
                    HandleHorizontalMovement(horizontalInput);
                break;

            case PlayerState.Attack: //Attack State
                HandleHorizontalMovement(horizontalInput);
                PlayerAttack();
                break;

            case PlayerState.Block:  //Block State
                if (!IsBlocking)
                {
                    PlayerBlocking();
                    HandleHorizontalMovement(horizontalInput);
                }
                break;

            case PlayerState.Ultimate:  //Ulti State
                PlayerUltimateAttack();
                break;
            case PlayerState.Lock:  //Lock State
                break;

            case PlayerState.End:  //End State
                break;
        }
        ComboAttack();

        if (IsAttacking && AttackCooldown.ExpiredOrNotRunning(Runner))
        {
            IsAttacking = false;
            SetPlayerState(PlayerState.Idle);
        }

        if (IsBlocking && AttackCooldown.ExpiredOrNotRunning(Runner))
        {
            IsBlocking = false;
            SetPlayerState(PlayerState.Idle);
        }

        if (IsUltimate && AttackCooldown.ExpiredOrNotRunning(Runner))
        {
            IsUltimate = false;
            _damagePlayer.canUltimate = false;
            UltimateAttack.StopAbility();
            SetPlayerState(PlayerState.Idle);
        }

        Velocity = rb.Rigidbody.velocity;
        ResetJumps();
    }

    // Moves the spaceship RB using the input for the client with InputAuthority over the object
    private void Move(Mult_NetworkInputData input)
    {
        horizontalInput = input.HorizontalInput;
        if (horizontalInput != 0)
            isRunning = true;
        if (horizontalInput == 0)
            isRunning = false;

        if (input.Buttons.WasPressed(_buttonsPrevious, PVPButtons.Attack) && !IsAttacking) //Attack Input
        {
            SetPlayerState(PlayerState.Attack);
        }

        if (input.Buttons.WasPressed(_buttonsPrevious, PVPButtons.Block) && !IsBlocking) //Block Input
        {
            SetPlayerState(PlayerState.Block);
        }

        if (input.Buttons.WasPressed(_buttonsPrevious, PVPButtons.Ultimate) && _damagePlayer.canUltimate) //Ulti Input
        {
            SetPlayerState(PlayerState.Ultimate);
        }

        if (input.Buttons.WasPressed(_buttonsPrevious, PVPButtons.Jump) && (isGrounded() || jumpsRemaining > 0))
        {
            anim.SetBool("isJump", true);
            if (!isGrounded()) // If we are not on the ground, it means a double jump is being made.
            {
                jumpsRemaining--;
            }
            rb.Rigidbody.velocity = new Vector2(rb.Rigidbody.velocity.x, jumpingPower);
        }

        if (input.Buttons.WasReleased(_buttonsPrevious, PVPButtons.Jump) && rb.Rigidbody.velocity.y > 0f)
            rb.Rigidbody.velocity = new Vector2(rb.Rigidbody.velocity.x, rb.Rigidbody.velocity.y * 0.5f);

        _buttonsPrevious = input.Buttons;
    }

    void LateUpdate()
    {
        if (Velocity.x == - 12.0f)
        {
            if (!Flip)
            {
                Flip = true;
                IsFacingRight = false;
                Debug.Log("In PVP Player Movement" + IsFacingRight);
            }
        }
        else if (Velocity.x == 12.0f)
        {
            if (Flip)
            {
                Flip = false;
                IsFacingRight = true;
                Debug.Log("In PVP Player Movement" + IsFacingRight);
            }
        }
    }

    public override void Render()
    {
        anim.SetBool("isRun", isRunning);
        anim.SetBool("isAttack", IsAttacking);
        anim.SetBool("isBlock", IsBlocking);
        anim.SetBool("isUlti", IsUltimate);
    }

    private void PlayerAttack()
    {
        if (IsUltimate)
            return;

        if (IsBlocking)
            return;

        if (IsAttacking)
            return;

        if (AttackCooldown.ExpiredOrNotRunning(Runner) == false)
            return;

        IsAttacking = true;
        AttackCooldown = TickTimer.CreateFromSeconds(Runner, attackDuration);
    }

    private void PlayerBlocking()
    {
        if (IsUltimate)
            return;

        if (IsBlocking)
            return;

        if (IsAttacking)
            return;

        if (AttackCooldown.ExpiredOrNotRunning(Runner) == false)
            return;

        IsBlocking = true;
        AttackCooldown = TickTimer.CreateFromSeconds(Runner, BlockDuration);
    }

    private void PlayerUltimateAttack()
    {
        if (IsUltimate)
            return;

        if (IsBlocking)
            return;

        if (IsAttacking)
            return;

        if (AttackCooldown.ExpiredOrNotRunning(Runner) == false)
            return;

        IsUltimate = true;
        UltimateAttack.ExecuteSpecialAbility();
        AttackCooldown = TickTimer.CreateFromSeconds(Runner, UltimateDuration);
    }


    public void ComboAttack()
    {
        if (attackAmount != 0) //If attack count is not 0, start comboTimer countdown
        {
            //comboText.enabled = true;
            //comboText.text = attackAmount.ToString(attackAmount + "");
            comboTimer -= Time.deltaTime;
            comboDamage = false;

            if (comboTimer <= 0) //Reset combo if comboTimer is less than 0
            {
                ComboReset();
                //comboText.enabled = false;
            }

            if (attackAmount >= 2)
            {
                comboTimer -= Time.deltaTime;
                //comboText.text = attackAmount.ToString(attackAmount + "");
                comboDamage = true;
                //print("COMBO TIME");
            }
        }
    }
    public void ComboReset()
    {
        comboTimer = 5f;
        attackAmount = 0;
        comboDamage = false;
    }
    public void HandleHorizontalMovement(float Horizontal) //Movement
    {
        rb.Rigidbody.velocity = new Vector2(Horizontal * moveSpeed, rb.Rigidbody.velocity.y);
    }

    private void ResetJumps()
    {
        if (isGrounded())
        {
            jumpsRemaining = maxJumps;
            anim.SetBool("isJump", false);
        }
    }

    private bool isGrounded() //Ground contact control
    {
        //return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return Runner.GetPhysicsScene2D().OverlapCircle(groundCheck.position, 0.2f, groundLayer);

    }

    public void SetPlayerState(PlayerState newState) //Change state
    {
        currentState = newState;
    }

    public bool GetIsFacingRight()
    {
        return IsFacingRight;
    }

    public bool GetIsAttacking()
    {
        return IsAttacking;
    }

    public bool GetIsBlocking()
    {
        return IsBlocking;
    }

    private void FlipEventOccured()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }


    public bool Flip
    {
        get { return FlipEvent; }
        set
        {
            Debug.Log("Flip");
            if (FlipEvent != value)
            {
                FlipEvent = value;
                FlipEventOccured();
            }
        }
    }
}

public enum PlayerState //Enums
{
    Idle,
    Attack,
    Block,
    Ultimate,
    Lock,
    End
}
