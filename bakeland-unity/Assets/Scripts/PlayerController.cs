using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float speed = 7f;
    [SerializeField] float fastSpeed = 10f;
    [SerializeField] float ridingSpeed = 17f;
    float currentSpeed;
    bool isWalking;
    bool ridingSoundInProgress;
    public static bool isRiding;
    public static bool isUsingWeapon;
    public static bool isWeaponActive;
    public static Vector2 direction;
    public static bool canMove;

    [Header("Speed Multipliers")]
    public float moveSpeedMultiplier = 1;
    public float rideSpeedMultiplier = 1;

    [Header("Stairs Settings")]
    private bool onStairs = false;
    private Vector2 stairDirection;
    public float stairsMoveSpeed;

    [Header("References")]
    Rigidbody2D rb;
    Collider2D col2D;
    Animator animator;
    public Collider2D wallsCollider;
    public ParticleSystem dustParticles;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        currentSpeed = speed * moveSpeedMultiplier;
        isWalking = false;
        isRiding = false;
        isUsingWeapon = false;
        canMove = true;
    }

    void FixedUpdate()
    {
        if (!isUsingWeapon) // Disable movement when attacking
            Movement();
    }

    void Update()
    {
        if (!isUsingWeapon) // Allow input only when not attacking
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
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

    void Movement()
    {
        if (!canMove) return;

        if (onStairs)
        {
            wallsCollider.enabled = false;

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
            wallsCollider.enabled = true;
            rb.velocity = moveInput.normalized * currentSpeed;
        }

        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && rb.velocity.sqrMagnitude > 0.01f)
        {
            direction.x = Input.GetAxisRaw("Horizontal");
            direction.y = Input.GetAxisRaw("Vertical");
            isWalking = true;
        }
        else
        {
            isWalking = false;
            rb.velocity = Vector2.zero;
        }
    }

    void HandleSpeed()
    {
        if (isRiding)
        {
            currentSpeed = ridingSpeed * rideSpeedMultiplier;
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = fastSpeed * moveSpeedMultiplier;
        }
        else
        {
            currentSpeed = speed * moveSpeedMultiplier;
        }
    }

    void HandleAnimations()
    {
        animator.SetFloat("SpeedX", direction.x);
        animator.SetFloat("SpeedY", direction.y);
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRiding", isRiding);
    }

    public void PlayFootstepSound()
    {
        SoundManager.instance.PlayRandomFromList(SoundManager.instance.footstepSounds);
        dustParticles.Play();
    }

    public void PlayDustParticles()
    {
        dustParticles.Play();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            onStairs = true;
            stairDirection = collision.GetComponent<Stairs>().stairMovementDirection.normalized;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stairs"))
        {
            onStairs = false;
        }
    }

    // Function to change facing direction based on the attack
    public void FaceTowards(Vector2 attackDirection)
    {
        if (attackDirection == Vector2.zero) return;

        direction = attackDirection.normalized;
        animator.SetFloat("SpeedX", direction.x);
        animator.SetFloat("SpeedY", direction.y);
    }

    // Call this function to trigger an attack
    public void StartAttack(Vector2 attackDirection)
    {
        if (isUsingWeapon) return;

        isUsingWeapon = true;
        canMove = false;

        FaceTowards(attackDirection);

        // Simulate attack duration
        StartCoroutine(EndAttack());
    }

    // Ends the attack after a delay
    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.5f); // Example duration
        isUsingWeapon = false;
        canMove = true;
    }
}
