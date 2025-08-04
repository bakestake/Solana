using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WalletConnectSharp.Core.Interfaces;

public class PvC_PlayerMovement : MonoBehaviour
{
    [Header("MOVEMENT")]
    private bool FlipEvent = false;
    private float moveSpeed = 12f;
    private bool IsFacingRight = false;
    private float attackDuration = 1.0f;
    private float BlockDuration = 1.0f;
    private float UltimateDuration = 1.5f;
    private bool isCoolDownRunning = false;


    private bool isRunning { get; set; } = false;


    private float horizontalInput { get; set; }


    public bool IsAttacking { get; private set; } = false;


    public bool IsBlocking { get; private set; } = false;


    public bool IsUltimate { get; private set; } = false;


    //private TickTimer AttackCooldown { get; set; }

    public bool isPlayer;
    public bool isNPC;


    [Header("COMPONENTS")]
    public Animator anim;
    private Rigidbody2D rb = null;
    public PvC_Ultimate UltimateAttack = null;

    [Header("STATE CONTROL")]
    public PvC_PlayerState currentState = PvC_PlayerState.Idle;

    [Header("ULTIMATE")]
    public Text ultiText;

    [Header("COMBO")]
    public int attackAmount = 0;
    public float comboTimer = 4F;

    public bool comboDamage { get; private set; } = false;

    [Header("UI")]
    public Text comboText;

    [Header("Jump Control")]
    public float jumpingPower = 20f;
    public int maxJumps = 1; // Maximum number of jumps allowed
    private int jumpsRemaining;

    public Transform groundCheck;
    public LayerMask groundLayer;

    private PvC_Damage_Player _damagePlayer = null;

    //private NetworkButtons _buttonsPrevious { get; set; }
    public Vector3 Velocity { get; set; }

    private IPvCInputs inputProvider;

    //bool jumpReleased = false;
    //Inputs region End

    public void Start()
    {
        // --- Host & Client
        // Set the local runtime references.
        rb = GetComponent<Rigidbody2D>();
        _damagePlayer = GetComponent<PvC_Damage_Player>();
        //UltimateAttack = GetComponent<Ultimate>();
        //_playerDataNetworked = GetComponent<Mult_PlayerDataNetworked>();
        //_visualController = GetComponent<Mult_PlayerVisualController>();
        //_playerController = GetComponent<Mult_PlayerController>();

        jumpsRemaining = maxJumps; // Initially grant jumps for the maximum number of jumps allowed

        if (isPlayer) // Your condition to check if it's a player.
        {
            inputProvider = new Player_InputProvider();
        }
        else if (isNPC) // Your condition to check if it's an NPC.
        {
            NPCController npcController = GetComponent<NPCController>();
            inputProvider = new NPC_InputProvider(npcController);
        }
    }

    public void LateUpdate()
    {
        // GetInput() can only be called from NetworkBehaviours.
        // In SimulationBehaviours, either TryGetInputForPlayer<T>() or GetInputForPlayer<T>() has to be called.
        // This will only return true on the Client with InputAuthority for this Object and the Host.
        // if (inputProvider != null)
        // {
        //     Move(inputProvider);
        // }

        switch (currentState)
        {
            case PvC_PlayerState.Idle:  //Idle State
                if (!trampoline.isRecoiling)
                    HandleHorizontalMovement(horizontalInput);
                break;

            case PvC_PlayerState.Attack: //Attack State
                HandleHorizontalMovement(horizontalInput);
                PlayerAttack();
                break;

            case PvC_PlayerState.Block:  //Block State
                if (!IsBlocking)
                {
                    PlayerBlocking();
                    HandleHorizontalMovement(horizontalInput);
                }
                break;

            case PvC_PlayerState.Ultimate:  //Ulti State
                PlayerUltimateAttack();
                break;
            case PvC_PlayerState.Lock:  //Lock State
                break;

            case PvC_PlayerState.End:  //End State
                break;
        }

        //For Animations
        anim.SetBool("isAttack", IsAttacking);
        anim.SetBool("isBlock", IsBlocking);
        anim.SetBool("isUlti", IsUltimate);
        //Animations End

        ComboAttack();

        if (IsAttacking && !isCoolDownRunning)
        {
            IsAttacking = false;
            SetPlayerState(PvC_PlayerState.Idle);
        }

        if (IsBlocking && !isCoolDownRunning)
        {
            IsBlocking = false;
            SetPlayerState(PvC_PlayerState.Idle);
        }

        if (IsUltimate && !isCoolDownRunning)
        {
            IsUltimate = false;
            _damagePlayer.canUltimate = false;
            UltimateAttack.StopAbility();
            SetPlayerState(PvC_PlayerState.Idle);
        }

        Velocity = rb.velocity;
        ResetJumps();
    }

    private void Move(IPvCInputs pvCInputs)
    {
        // Retrieve the input from either player or NPC
        horizontalInput = pvCInputs.GetHorizontalInput();

        if (pvCInputs.IsAttacking() && !IsAttacking) //Attack Input
        {
            SetPlayerState(PvC_PlayerState.Attack);
        }

        if (pvCInputs.IsBlocking() && !IsBlocking) //Block Input
        {
            SetPlayerState(PvC_PlayerState.Block);
        }

        if (pvCInputs.UseUltimate() && _damagePlayer.canUltimate) //Ulti Input
        {
            SetPlayerState(PvC_PlayerState.Ultimate);
        }

        if (pvCInputs.IsJumping() && (isGrounded() || jumpsRemaining > 0))
        {
            anim.SetBool("isJump", true);
            if (!isGrounded()) // If we are not on the ground, it means a double jump is being made.
            {
                jumpsRemaining--;
            }
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (pvCInputs.IsJumping() && rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }

    void Update()
    {
        if (Velocity.x == -12.0f)
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

    public void FixedUpdate()
    {
        isRunning = horizontalInput != 0;
        anim.SetBool("isRun", isRunning);

        if (inputProvider != null)
        {
            Move(inputProvider);
        }
    }

    private void PlayerAttack()
    {
        if (IsUltimate)
            return;

        if (IsBlocking)
            return;

        if (IsAttacking)
            return;

        if (isCoolDownRunning)
            return;

        IsAttacking = true;
        StartCoroutine(AttackCooldown(attackDuration));
    }

    private void PlayerBlocking()
    {
        if (IsUltimate)
            return;

        if (IsBlocking)
            return;

        if (IsAttacking)
            return;

        if (isCoolDownRunning)
            return;

        IsBlocking = true;

        StartCoroutine(AttackCooldown(BlockDuration));
    }

    private void PlayerUltimateAttack()
    {
        if (IsUltimate)
            return;

        if (IsBlocking)
            return;

        if (IsAttacking)
            return;

        if (isCoolDownRunning)
            return;

        IsUltimate = true;
        UltimateAttack.ExecuteSpecialAbility();
        StartCoroutine(AttackCooldown(UltimateDuration));
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
        rb.velocity = new Vector2(Horizontal * moveSpeed, rb.velocity.y);
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
        // Visualize the overlap circle in the scene view (for debugging)
        //Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.2f, Color.black);
        // Perform the overlap circle check and store the result
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        return groundCollider != null;

    }

    public void SetPlayerState(PvC_PlayerState newState) //Change state
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

    private IEnumerator AttackCooldown(float duration)
    {
        isCoolDownRunning = true;
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        isCoolDownRunning = false;
    }
}

public enum PvC_PlayerState //Enums
{
    Idle,
    Attack,
    Block,
    Ultimate,
    Lock,
    End
}
