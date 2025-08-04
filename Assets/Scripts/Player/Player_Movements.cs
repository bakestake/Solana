using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Player_Movements : MonoBehaviour
{
    /*[Header("MOVEMENT")]
    public float moveSpeed = 12f;

    [Header("BOOLS")]
    public bool isFacingRight = false;

    [Header("COMPONENTS")]
    public Animator anim;
    private Rigidbody2D rb;
    private float horizontalInput;

    [Header("STATE CONTROL")]
    public PlayerState currentState = PlayerState.Idle;

    [Header("ATTACK")]
    public GameObject weaponHandler;
    public GameObject weapon;
    public float rotSpeedWeapon = 1200f;
    public  bool isAttacking = false;

    [Header("ATTACK")]
    public static bool isBlocking = false;
    public float attackDuration;

    [Header("ULTIMATE")]
    public static bool isUltimate = false;
    public float ultimateTime;
    public Text ultiText;

    [Header("COMBO")]
    public int attackAmount = 0;
    public float comboTimer = 4F;
    public bool comboDamage = false;

    [Header("UI")]
    public Text comboText;

    [Header("Jump Control")]
    public  float jumpingPower = 20f;
    public  int maxJumps = 1; // Maximum number of jumps allowed
    private int jumpsRemaining;

    public Transform groundCheck;
    public LayerMask groundLayer;



    private GameObject player2Reference;
    public void SetPlayerReference(GameObject player) // To take the selected character as a player reference.
    {
        player2Reference = player;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck");

        jumpingPower = 20;
        isFacingRight = true;
        isAttacking = false;
        isBlocking = false;
        isUltimate = false;
        comboDamage = false;
        ultiText.enabled = false;

        jumpsRemaining = maxJumps; // Initially grant jumps for the maximum number of jumps allowed
    }
    private void Update()
    {
        horizontalInput = 0; // Reset horizontal input

        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1;
        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1;

        if (Input.GetKeyDown(KeyCode.F) && !isAttacking && !Damage_Player.canUltimate) //Attack Input
            SetPlayerState(PlayerState.Attack);

        if (Input.GetKeyDown(KeyCode.R) && !isBlocking) //Block Input
            SetPlayerState(PlayerState.Block);

        if (Input.GetKeyDown(KeyCode.F) && Damage_Player.canUltimate) //Ulti Input
        {
            isUltimate = true;
            SetPlayerState(PlayerState.Ultimate);
        }
        if (PvPGameManager.managerInstance.isEnd) //End animation
        {
            SetPlayerState(PlayerState.End);
            anim.SetTrigger("isVic");
        }

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded() || jumpsRemaining > 0))
        {
            anim.SetBool("isJump", true);
            if (!isGrounded()) // If we are not on the ground, it means a double jump is being made.
            {
                jumpsRemaining--;
            }
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

        ComboAttack();

    }

    private void FixedUpdate()
    {
        //rb.velocity = new Vector2(horizontal * 5, rb.velocity.y);
        //horizontalInput = Input.GetAxis("Horizontal");
        switch (currentState)
        {
            case PlayerState.Idle:  //Idle State
                HandleIdleState();
                if(!trampoline.isRecoiling)
                    HandleHorizontalMovement(horizontalInput);
                break;
        
            case PlayerState.Attack: //Attack State
                HandleIdleState();
                HandleHorizontalMovement(horizontalInput);
                if (!isAttacking)
                    StartCoroutine(AttackCoroutine());
                break;
        
            case PlayerState.Block:  //Block State
                if (!isBlocking)
                {
                    StartCoroutine(BlockCoroutine());
                    HandleHorizontalMovement(horizontalInput);
                }
                break;
        
            case PlayerState.Ultimate:  //Ulti State
                StartCoroutine(UltimateAttack());
                break;
            case PlayerState.Lock:  //Lock State
                break;

            case PlayerState.End:  //End State
                break;
        }
        
        if (Damage_Player.canUltimate) // Activate ulti text if using ultimate
        ultiText.enabled = true;
    }

    public void ComboAttack()
    {
        if (attackAmount != 0) //If attack count is not 0, start comboTimer countdown
        {
            comboText.enabled = true;
            comboText.text = attackAmount.ToString(attackAmount + "");
            comboTimer -= Time.deltaTime;
            comboDamage = false;

            if (comboTimer <= 0) //Reset combo if comboTimer is less than 0
            {
                ComboReset();
                comboText.enabled = false;
            }

            if (attackAmount >= 2)
            {
                comboTimer -= Time.deltaTime;
                comboText.text = attackAmount.ToString(attackAmount + "");
                comboDamage = true;
                print("COMBO TIME");
            }
        }
    }
    public void ComboReset()
    {
        comboTimer = 5f;
        attackAmount = 0;
        comboDamage = false;
    }

    private void HandleIdleState()//Flip and run anim Control
    {
        if ((isFacingRight && horizontalInput < 0) || (!isFacingRight && horizontalInput > 0))
        {
            anim.SetBool("isRun", true);
            flip();
        }
        if (horizontalInput != 0)
            anim.SetBool("isRun", true);
        if (horizontalInput == 0)
            anim.SetBool("isRun", false);
    }
    private void flip()//Flip
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    public void HandleHorizontalMovement(float Horizontal) //Movement
    {
        rb.velocity = new Vector2(Horizontal * moveSpeed, rb.velocity.y);
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGrounded())
        {
            jumpsRemaining = maxJumps;
            anim.SetBool("isJump", false);
        }
    }
    private bool isGrounded() //Ground contact control
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    private IEnumerator AttackCoroutine() //Attack time and weapon move
    {
        isAttacking = true;
        anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;
        anim.SetBool("isAttack", false);

        SetPlayerState(PlayerState.Idle);

    }
    IEnumerator BlockCoroutine() //Block Time
    {
        anim.SetBool("isBlock", true);

        isBlocking = true;
        yield return new WaitForSeconds(0.3f);
        isBlocking = false;

        anim.SetBool("isBlock", false);

        SetPlayerState(PlayerState.Idle);
    }
    IEnumerator UltimateAttack() //UltimateAttack Time
    {

        isUltimate = false;
        yield return new WaitForSeconds(ultimateTime);
        ultiText.enabled = false;

        Damage_Player.canUltimate = false;
        //Ultimate.usedUltiP1 = false;
        SetPlayerState(PlayerState.Idle);

    }
    public void SetPlayerState(PlayerState newState) //Change state
    {
        currentState = newState;
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
    //public void Move(InputAction.CallbackContext context)
    //{
    //    horizontal = context.ReadValue<Vector2>().x * 4;
    //}
    */
}
