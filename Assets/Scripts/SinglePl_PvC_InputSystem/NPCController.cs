using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Transform target; // The target the NPC is interacting with (e.g., the player)
    [SerializeField] private float followRange = 16f; // How close the NPC gets before attacking
    [SerializeField] private float attackRange = 2f; // The range within which the NPC will attack

    public bool isJumping { get; private set; }
    public bool isBlocking { get; private set; }

    private void Start()
    {
        if (target == null)
        {
            target = LocalGameManager.Instance.PlayerController.transform;
        }
    }

    // Move toward the target if within follow range
    public float GetMovementDirection()
    {
        if (target == null)
        {
            return 0; // No movement if no target
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        // If the target is within the follow range but not in attack range, move toward it
        if (distanceToTarget < followRange && distanceToTarget > attackRange)
        {
            // Move toward the target
            return (target.position.x > transform.position.x) ? 1 : -1;
        }

        // Stop moving if the NPC is close enough
        return 0;
    }

    // Attack if within range
    public bool ShouldAttack()
    {
        if (target == null)
        {
            return false;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        return distanceToTarget <= attackRange; // Attack if the target is within range
    }

    // Block randomly or based on certain conditions (e.g., close to the player)
    public bool ShouldBlock()
    {
        // For now, block randomly
        return Random.Range(0, 10) > 8; // 20% chance to block
    }

    // NPC will jump occasionally for this example
    public bool ShouldJump()
    {
        if (!isJumping)
        {
            // For now, make the NPC jump randomly
            isJumping = Random.Range(0, 10) > 7; // 30% chance to jump
        }

        return isJumping;
    }

    // Ultimate ability (simple logic for now)
    public bool ShouldUseUltimate()
    {
        // Simple condition: NPC will use the ultimate if the player is within a very close range
        if (target == null)
        {
            return false;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        return distanceToTarget <= attackRange / 2; // Use ultimate if the target is really close
    }

    // Reset jumping after landing (just to simulate it ending)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false; // NPC lands on the ground
        }
    }
}
