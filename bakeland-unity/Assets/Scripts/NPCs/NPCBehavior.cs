using System.Collections;
using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public bool masterBoolEnabled = true;
    public bool canAttack = false;                  // Determines if the NPC is hostile
    public bool noBounds = false;                   // New bool to allow free wandering
    public bool ignoreNPCs = false;                 // New bool to ignore NPC collisions
    public Transform player;                        // Reference to the player's transform
    public float detectRange = 5f;                  // Range within which the NPC detects the player
    public float attackRange = 1.5f;                // Range within which the NPC attacks
    public int attackDamage = 2;                    // NPC attack damage
    public float attackCooldown = 2f;              // Cooldown time between attacks
    public float speed = 2f;                        // Speed for walking
    public float chaseSpeed = 3f;                   // Speed for chasing (faster than normal speed)
    public Collider2D boundsCollider2D;             // Bounding area for NPC wandering
    public float obstacleDetectionDistance = 0.5f;  // Distance to detect obstacles
    public float outOfBoundsTimeLimit = 3f;         // Time allowed outside bounds before teleporting
    public float teleportCheckRadius = 0.5f;        // Radius to check for obstacles during teleport
    public int maxTeleportAttempts = 10;            // Maximum attempts to find a valid position

    [Header("Pushback Settings")]
    public float pushbackForce = 5f;      // Strength of the pushback effect
    public float pushbackDuration = 0.2f; // Duration of the pushback
    private bool isBeingPushedBack = false;

    [Header("Run Duration Settings")]
    public float minRunDuration = 2f;
    public float maxRunDuration = 5f;

    [Header("Stop Duration Settings")]
    public float minStopDuration = 1f;
    public float maxStopDuration = 3f;

    private Vector3 currentDirection;
    private bool isWalking = false;
    private Animator animator;
    private Rigidbody2D rb;
    private float moveTimer;
    private float outOfBoundsTimer = 0f;
    private float attackTimer = 0f;                // Timer for attack cooldown
    private bool isChasing = false;                // State to determine if NPC is chasing the player

    void Start()
    {
        if (!masterBoolEnabled) { return; }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        isWalking = true;
        ChangeDirection();
        SetRandomRunDuration();
        animator.fireEvents = false;

        player = GameObject.FindWithTag("Player").transform;

        // Validate bounds only if noBounds is false
        if (!noBounds && boundsCollider2D == null)
        {
            Debug.LogWarning($"No bounds collider set for {gameObject.name} and noBounds is false!");
        }

        // Subscribe to the Health component's damage event
        var healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.onDamageTaken += ApplyPushback;
        }
    }

    private void OnDestroy()
    {
        var healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.onDamageTaken -= ApplyPushback;
        }
    }

    void Update()
    {
        if (!masterBoolEnabled || isBeingPushedBack) { return; }

        attackTimer -= Time.deltaTime;

        if (canAttack && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectRange)
            {
                // Switch to chasing or attacking state
                isChasing = true;

                if (distanceToPlayer <= attackRange)
                {
                    AttackPlayer();
                }
                else
                {
                    ChasePlayer();
                }
            }
            else if (isChasing)
            {
                // Return to wandering when the player is out of range
                isChasing = false;
                ResetToWanderingState();
            }
        }

        if (!isChasing)
        {
            WanderingBehavior();
        }
    }

    private void ApplyPushback(Vector3 damageSourcePosition)
    {
        if (isBeingPushedBack) return; // Prevent overlapping pushback effects
        isBeingPushedBack = true;

        Vector3 pushDirection = (transform.position - damageSourcePosition).normalized;
        StartCoroutine(PerformPushback(pushDirection));
    }

    private IEnumerator PerformPushback(Vector3 pushDirection)
    {
        float timer = 0f;

        while (timer < pushbackDuration)
        {
            timer += Time.deltaTime;

            rb.MovePosition(rb.position + (Vector2)(pushDirection * pushbackForce * Time.deltaTime));
            yield return null;
        }

        isBeingPushedBack = false;
    }

    private void ChasePlayer()
    {
        if (isBeingPushedBack) return; // Skip chasing if being pushed back

        EnableOutline();

        isWalking = true; // Ensure walking animation is active
        UpdateAnimation();

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + directionToPlayer * chaseSpeed * Time.deltaTime);

        // Update direction for animations to face the player
        FaceTowards(player.position);
    }
    private void FaceTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement
            if (direction.x > 0)
                currentDirection = Vector2.right;  // Facing right
            else
                currentDirection = Vector2.left;   // Facing left
        }
        else
        {
            // Vertical movement
            if (direction.y > 0)
                currentDirection = Vector2.up;    // Facing up
            else
                currentDirection = Vector2.down;  // Facing down
        }

        UpdateAnimation(); // Ensure animations reflect the updated direction
    }
    private void AttackPlayer()
    {
        isWalking = false;
        UpdateAnimation();

        if (player.GetComponent<Health>().canGetHit)
        {
            if (attackTimer <= 0)
            {
                // Trigger attack animation and reset cooldown
                animator.SetTrigger("Attack");
                attackTimer = attackCooldown;

                // Apply damage to the player
                player.GetComponent<Health>().TakeDamage(attackDamage, transform.position);
                SoundManager.instance.PlayRandomFromList(SoundManager.instance.playerHitSounds);
            }
        }
    }

    private void WanderingBehavior()
    {
        if (isBeingPushedBack) return; // Skip wandering if being pushed back

        DisableOutline();

        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0)
        {
            // Toggle between running and stopping
            isWalking = !isWalking;

            if (isWalking)
            {
                SetRandomRunDuration();
                ChangeDirection();
            }
            else
            {
                SetRandomStopDuration();
            }

            UpdateAnimation();
        }

        if (isWalking)
        {
            Movement();
        }

        CheckOutOfBounds();
    }

    private void Movement()
    {
        Vector3 point = transform.position + currentDirection * speed * Time.deltaTime;

        // If noBounds is true, skip bounds checking
        if (noBounds)
        {
            if (!IsNearObstacle())
            {
                rb.MovePosition(point);
            }
            else
            {
                ChangeDirection();
            }
            return;
        }

        // Original bounds-checking logic
        if (boundsCollider2D.bounds.Contains(point))
        {
            outOfBoundsTimer = 0f;

            if (!IsNearObstacle())
            {
                rb.MovePosition(point);
            }
            else
            {
                ChangeDirection();
            }
        }
        else
        {
            ChangeDirection();
        }
    }

    private bool IsNearObstacle()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, currentDirection, obstacleDetectionDistance);

        foreach (var hit in hits)
        {
            // Skip if the hit object is null or is the bounds collider or is this gameObject
            if (hit.collider == null || hit.collider == boundsCollider2D || hit.collider.gameObject == gameObject)
                continue;

            // If ignoreNPCs is true, skip other NPCs
            if (ignoreNPCs && hit.collider.GetComponent<NPCBehavior>() != null)
                continue;

            return true;
        }
        return false;
    }

    private void ChangeDirection()
    {
        int randomInt = Random.Range(1, 5);
        switch (randomInt)
        {
            case 1:
                currentDirection = Vector2.down;
                break;
            case 2:
                currentDirection = Vector2.up;
                break;
            case 3:
                currentDirection = Vector2.left;
                break;
            case 4:
                currentDirection = Vector2.right;
                break;
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        animator.SetBool("IsWalking", isWalking);
        animator.SetFloat("SpeedX", currentDirection.x);
        animator.SetFloat("SpeedY", currentDirection.y);
    }

    public void Talking()
    {
        isWalking = false;
        UpdateAnimation();
    }

    public void EndTalking()
    {
        isWalking = true;
        SetRandomRunDuration();
        ChangeDirection();
        UpdateAnimation();
    }

    private void ResetToWanderingState()
    {
        isWalking = true;
        SetRandomRunDuration();
        ChangeDirection();
        UpdateAnimation();
    }

    private void CheckOutOfBounds()
    {
        // Skip bounds checking if noBounds is true
        if (noBounds) return;

        if (!boundsCollider2D.bounds.Contains(transform.position))
        {
            outOfBoundsTimer += Time.deltaTime;

            if (outOfBoundsTimer >= outOfBoundsTimeLimit)
            {
                TeleportToRandomPosition();
                outOfBoundsTimer = 0f;
            }
        }
    }

    private void TeleportToRandomPosition()
    {
        Vector3 newPosition = GetRandomPositionInsideBounds();
        int attempts = 0;

        while (Physics2D.OverlapCircle(newPosition, teleportCheckRadius) != null && attempts < maxTeleportAttempts)
        {
            newPosition = GetRandomPositionInsideBounds();
            attempts++;
        }

        if (attempts >= maxTeleportAttempts)
        {
            Debug.LogWarning("Teleporting NPC to bounds center due to failed attempts to find valid position.");
            newPosition = boundsCollider2D.bounds.center;
        }

        rb.MovePosition(newPosition);
    }

    private Vector3 GetRandomPositionInsideBounds()
    {
        Bounds bounds = boundsCollider2D.bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(randomX, randomY, transform.position.z);
    }

    private void SetRandomRunDuration()
    {
        moveTimer = Random.Range(minRunDuration, maxRunDuration);
    }

    private void SetRandomStopDuration()
    {
        moveTimer = Random.Range(minStopDuration, maxStopDuration);
    }

    private void EnableOutline()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineAlpha", 1f);
    }

    private void DisableOutline()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_OutlineAlpha", 0f);
    }
}
