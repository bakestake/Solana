using Gamegaard.CustomValues;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WanderingMovementHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool noBounds;
    [SerializeField] private bool ignoreNPCs;

    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private MinMaxFloat moveDuration = new MinMaxFloat(0.5f, 2f);
    [SerializeField] private MinMaxFloat idleDuration = new MinMaxFloat(0.5f, 1f);
    [SerializeField] private float outOfBoundsTimeLimit = 3f;
    [SerializeField] private Vector2 collisionCheckSize = new Vector2(0.8f, 0.8f);

    [Header("References")]
    [SerializeField] private Collider2D boundsCollider2D;
    [SerializeField] private Animator animator;

    private Vector2 currentDirection;
    private Bounds bounds;
    private Coroutine wanderCoroutine;
    private Rigidbody2D rb;
    private Collider2D[] selfColliders;

    public bool IsWalking { get; private set; }

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        selfColliders = GetComponents<Collider2D>();

        if (!noBounds && boundsCollider2D != null)
        {
            bounds = boundsCollider2D.bounds;
        }
    }

    private void Start()
    {
        StartWandering();
    }

    public void StartWandering()
    {
        StopWandering();
        wanderCoroutine = StartCoroutine(WanderRoutine());
    }

    public void StopWandering()
    {
        if (wanderCoroutine == null) return;

        SetWalking(false);
        rb.velocity = Vector2.zero;
        StopCoroutine(wanderCoroutine);
        wanderCoroutine = null;
    }

    public void SetBounds(Collider2D boundsCollider)
    {
        boundsCollider2D = boundsCollider;
    }

    private Vector2 GetValidDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        for (int i = directions.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (directions[i], directions[j]) = (directions[j], directions[i]);
        }

        foreach (Vector2 dir in directions)
        {
            Vector2 checkPos = (Vector2)transform.position + dir;
            if ((noBounds || bounds.Contains(checkPos)) && !IsDirectionBlocked(dir))
            {
                return dir;
            }
        }

        return Vector2.zero;
    }

    private bool IsDirectionBlocked(Vector2 direction)
    {
        Vector2 origin = rb.position;
        float distance = 1f;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, collisionCheckSize, 0f, direction, distance, obstacleLayer);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            bool isSelf = false;
            foreach (Collider2D col in selfColliders)
            {
                if (hit.collider == col)
                {
                    isSelf = true;
                    break;
                }
            }

            if (isSelf) continue;

            if (ignoreNPCs && hit.collider.GetComponent<NPCBehavior>() != null) continue;

            return true;
        }

        return false;
    }

    private Vector2 GetReturnDirection()
    {
        if (noBounds) return Vector2.zero;

        Vector2 center = bounds.center;
        Vector2 current = transform.position;
        Vector2 delta = center - current;

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 bestDir = Vector2.zero;
        float bestDot = -1f;

        foreach (Vector2 dir in directions)
        {
            Vector2 checkPos = current + dir;

            if (IsDirectionBlocked(dir)) continue;

            Vector2 toCenter = (center - checkPos).normalized;
            float dot = Vector2.Dot(dir, toCenter);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestDir = dir;
            }
        }

        return bestDir;
    }

    private void FaceTowards(Vector2 dir)
    {
        currentDirection = dir;
        animator.SetFloat("SpeedX", dir.x);
        animator.SetFloat("SpeedY", dir.y);
    }

    private void SetWalking(bool walking)
    {
        IsWalking = walking;
        animator.SetBool("IsWalking", walking);
        animator.SetFloat("SpeedX", currentDirection.x);
        animator.SetFloat("SpeedY", currentDirection.y);
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            currentDirection = GetValidDirection();
            bool hasDirection = currentDirection != Vector2.zero;

            if (hasDirection)
            {
                FaceTowards(currentDirection);
                SetWalking(true);

                float moveTime = moveDuration.GetRandom();
                float elapsed = 0f;

                while (elapsed < moveTime)
                {
                    elapsed += Time.deltaTime;

                    if (!noBounds && !bounds.Contains(transform.position))
                    {
                        Vector2 returnDir = GetReturnDirection();
                        if (returnDir != Vector2.zero)
                        {
                            currentDirection = returnDir;
                            FaceTowards(currentDirection);
                        }
                    }
                    else
                    {
                        if (IsDirectionBlocked(currentDirection))
                        {
                            currentDirection = GetValidDirection();
                            hasDirection = currentDirection != Vector2.zero;

                            if (hasDirection)
                            {
                                FaceTowards(currentDirection);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    rb.velocity = currentDirection;
                    yield return null;
                }
            }

            rb.velocity = Vector2.zero;
            SetWalking(false);

            float idleTime = idleDuration.GetRandom();
            float idleElapsed = 0f;
            while (idleElapsed < idleTime)
            {
                idleElapsed += Time.deltaTime;

                if (!noBounds && !bounds.Contains(transform.position))
                {
                    Vector2 returnDir = GetReturnDirection();
                    if (returnDir != Vector2.zero)
                    {
                        currentDirection = returnDir;
                        FaceTowards(currentDirection);
                        rb.velocity = currentDirection;
                        SetWalking(true);
                        break;
                    }
                }

                yield return null;
            }
        }
    }
}
