using System.Collections;
using UnityEngine;

[SelectionBase]
public class Enemy : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private bool canAttack;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private GameObject enemyHealthBar;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 3f;

    [Header("Pushback")]
    [SerializeField] private float pushbackForce = 5f;
    [SerializeField] private float pushbackDuration = 0.2f;

    private bool isBeingPushedBack;
    private bool isChasing;
    private float attackTimer;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Coroutine attackCoroutine;
    private WanderingMovementHandler movementHandler;

    public bool CanAttack
    {
        get => canAttack;
        set => canAttack = value;
    }

    private void Awake()
    {
        player = LocalGameManager.Instance.PlayerController.transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        movementHandler = GetComponent<WanderingMovementHandler>();

        var health = GetComponent<Health>();
        if (health != null)
            health.OnDamageTaken += ApplyPushback;
    }

    private void OnDestroy()
    {
        var health = GetComponent<Health>();
        if (health != null)
            health.OnDamageTaken -= ApplyPushback;
    }

    private void OnEnable()
    {
        player = LocalGameManager.Instance.PlayerController.transform;
        attackCoroutine ??= StartCoroutine(EnemyRoutine());
    }

    private void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private void TryAttackPlayer()
    {
        rb.velocity = Vector2.zero;

        if (attackTimer <= 0 && player.GetComponent<Health>().CanGetHit)
        {
            animator.SetTrigger("OnAttack");
            attackTimer = attackCooldown;
        }
    }

    public void ApplyDamageToPlayer()
    {
        player.GetComponent<Health>().TakeDamage(attackDamage, transform.position);
    }

    private void ApplyPushback(Vector3 source)
    {
        if (isBeingPushedBack) return;
        isBeingPushedBack = true;

        Vector3 direction = (transform.position - source).normalized;
        StartCoroutine(PushbackRoutine(direction));
    }

    private void FaceTowards(Vector2 dir)
    {
        animator.SetFloat("SpeedX", dir.x);
        animator.SetFloat("SpeedY", dir.y);
    }

    private void EnableOutline()
    {
        if (spriteRenderer != null)
            spriteRenderer.material.SetFloat("_OutlineAlpha", 1f);
    }

    private void DisableOutline()
    {
        if (spriteRenderer != null)
            spriteRenderer.material.SetFloat("_OutlineAlpha", 0f);
    }

    public void Die()
    {
        animator.SetTrigger("OnDie");
        enabled = false;
    }

    private IEnumerator EnemyRoutine()
    {
        while (true)
        {
            if (isBeingPushedBack)
            {
                yield return null;
                continue;
            }

            if (canAttack && player != null)
            {
                float dist = Vector2.Distance(transform.position, player.position);
                attackTimer -= Time.deltaTime;

                if (dist <= detectRange)
                {
                    if (!isChasing)
                    {
                        isChasing = true;
                        if (movementHandler != null)
                        {
                            movementHandler.StopWandering();
                        }
                    }

                    if (dist <= attackRange)
                    {
                        TryAttackPlayer();
                        yield return null;
                        continue;
                    }

                    Vector2 direction = (player.position - transform.position).normalized;
                    FaceTowards(direction);
                    rb.velocity = direction * chaseSpeed;

                    if (enemyHealthBar != null)
                        enemyHealthBar.SetActive(true);

                    EnableOutline();
                    yield return null;
                    continue;
                }
                else if (isChasing)
                {
                    isChasing = false;

                    if (movementHandler != null)
                    {
                        movementHandler.StartWandering();
                    }

                    if (enemyHealthBar != null)
                        enemyHealthBar.SetActive(false);

                    DisableOutline();
                }
            }

            yield return null;
        }
    }

    private IEnumerator PushbackRoutine(Vector3 direction)
    {
        float timer = 0f;
        movementHandler?.StopWandering();

        while (timer < pushbackDuration)
        {
            rb.MovePosition(rb.position + (Vector2)(direction * pushbackForce * Time.deltaTime));
            timer += Time.deltaTime;
            yield return null;
        }

        isBeingPushedBack = false;

        if (!isChasing)
            movementHandler?.StartWandering();
    }
}