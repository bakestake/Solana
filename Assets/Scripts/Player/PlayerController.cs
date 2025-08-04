using System.Collections;
using TMPro;
using UnityEngine;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Settings")]
    [SerializeField] private float speed = 7f;
    [SerializeField] private float fastSpeed = 10f;
    [SerializeField] private float ridingSpeed = 17f;
    [SerializeField] private float moveSpeedMultiplier = 1f;
    [SerializeField] private float rideSpeedMultiplier = 1f;
    [SerializeField] private float stairsMoveSpeed;

    [Header("References")]
    [SerializeField] private Collider2D wallsCollider;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private TMP_Text usernameText;

    [Header("Inputs")]
    [SerializeField] private KeyCode interactKey = KeyCode.Space;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private Vector2 inputDirection;
    private Vector2 stairDirection;

    private float currentSpeed;
    private bool isWalking;
    private bool IsRunInputPressed;
    private bool onStairs;

    public string username = "";

    public static Vector2 direction;
    public static bool isRiding;
    public static bool isUsingWeapon;
    public static bool isWeaponActive;
    public static bool canMove = true;
    public static bool canInteract = true;

    public PlayerInteractionAnimator InteractionAnimator { get; private set; }
    public Animator Animator { get; private set; }
    public float MoveSpeedMultiplier
    {
        get => moveSpeedMultiplier;
        set => moveSpeedMultiplier = value;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        InteractionAnimator = GetComponent<PlayerInteractionAnimator>();

        currentSpeed = speed * moveSpeedMultiplier;
        isWalking = false;
        isRiding = false;
        isUsingWeapon = false;
        canMove = true;
    }

    private void Update()
    {
        if (!isUsingWeapon)
        {
            HandleInput();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (rb.velocity == Vector2.zero) isWalking = false;
        if (!canMove) rb.velocity = Vector2.zero;

        HandleSpeed();
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        if (!isUsingWeapon)
        {
            HandleMovement();
        }
    }

    [ContextMenu(nameof(RemoveCoinstraints))]
    public void RemoveCoinstraints()
    {
        canMove = true;
        canInteract = true;
    }

    private void HandleInput()
    {
#if  UNITY_EDITOR || (!UNITY_IOS && !UNITY_ANDROID)
        SetMoveInput(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        HandleRunInput(Input.GetKey(KeyCode.LeftShift));
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryTouchAttack(Input.GetTouch(0).position);
        }
#endif

        if (moveInput != Vector2.zero)
        {
            inputDirection = moveInput;
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    public void TryTouchAttack(Vector2 screenPosition)
    {
        if (!isUsingWeapon && isWeaponActive)
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 attackDirection = (worldPosition - (Vector2)transform.position).normalized;
            StartAttack(attackDirection);
        }
    }

    public void TryInteract()
    {
        if (Interact.currentInteraction != null)
        {
            Interact.currentInteraction.TryInteract();
        }
        DialogueManager.Instance.TryInteract();
    }

    public void HandleRunInput(bool isPressed)
    {
        IsRunInputPressed = isPressed;
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        if (onStairs)
        {
            if (wallsCollider != null) wallsCollider.enabled = false;

            if (stairDirection.x != 0)
            {
                if (moveInput.x != 0)
                {
                    rb.velocity = stairDirection * moveInput.x * stairsMoveSpeed;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                }
            }
            else
            {
                rb.velocity = moveInput.normalized * stairsMoveSpeed;
            }
        }
        else
        {
            if (wallsCollider != null) wallsCollider.enabled = true;
            rb.velocity = moveInput.normalized * currentSpeed;
        }

        if (moveInput != Vector2.zero && rb.velocity.sqrMagnitude > 0.01f)
        {
            direction = moveInput;
            isWalking = true;
        }
        else
        {
            isWalking = false;
            rb.velocity = Vector2.zero;
        }
    }

    private void HandleSpeed()
    {
        if (isRiding)
        {
            currentSpeed = ridingSpeed * rideSpeedMultiplier;
            return;
        }

        if (IsRunInputPressed)
        {
            currentSpeed = fastSpeed * moveSpeedMultiplier;
        }
        else
        {
            currentSpeed = speed * moveSpeedMultiplier;
        }
    }

    private void HandleAnimations()
    {
        Animator.SetFloat("SpeedX", direction.x);
        Animator.SetFloat("SpeedY", direction.y);
        Animator.SetBool("IsWalking", isWalking);
        Animator.SetBool("IsRiding", isRiding);
    }

    public void SetMoveInput(Vector2 newMoveInput)
    {
        moveInput = newMoveInput;
    }

    public void PlayFootstepSound()
    {
        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.footstepSounds);
        dustParticles.Play();
    }

    public void PlayDustParticles()
    {
        dustParticles.Play();
    }

    public void FaceTowards(Vector2 attackDirection)
    {
        if (attackDirection == Vector2.zero) return;

        direction = attackDirection.normalized;
        Animator.SetFloat("SpeedX", direction.x);
        Animator.SetFloat("SpeedY", direction.y);
    }

    public void StartAttack(Vector2 attackDirection)
    {
        if (isUsingWeapon) return;

        isUsingWeapon = true;
        canMove = false;

        FaceTowards(attackDirection);

        StartCoroutine(EndAttack());
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            onStairs = true;
            stairDirection = collision.GetComponent<Stairs>().stairMovementDirection.normalized;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            onStairs = false;
        }
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.5f);
        isUsingWeapon = false;
        canMove = true;
    }

    public void SetUsername(string username)
    {
        usernameText.text = username;
    }
}