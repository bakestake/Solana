using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform weaponVisual;
    [SerializeField] private LayerMask enemyLayer;

    private float nextAttackTime = 0f;
    [SerializeField] private float distanceFromPlayer = 1.5f;
    [SerializeField] private float attackSwingDistance = 0.5f;

    [Header("Settings")] 
    [SerializeField] private bool shouldShowOutline = false;

    private Transform player;
    private Collider2D hitbox;
    private HitboxVisual hitboxVisual;
    private SpriteRenderer weaponRenderer;

    private HashSet<GameObject> hitEnemies; // Track enemies hit during the current swing
    private bool isAttacking = false;

    private Material weaponMaterial;
    private static readonly int OutlineAlpha = Shader.PropertyToID("_OutlineAlpha");

    void Start()
    {
        player = transform.parent;

        // Initialize DOTween if not already initialized
        if (!DOTween.instance)
        {
            DOTween.Init();
        }

        // Initialize hitbox
        hitbox = GetComponent<Collider2D>();
        if (hitbox == null)
        {
            Debug.LogError("A Collider2D is required on the Weapon object to enable hitbox functionality.");
            return;
        }

        hitbox.isTrigger = true; // Ensure it's a trigger collider
        hitEnemies = new HashSet<GameObject>();
        hitbox.enabled = false;

        // Setup hitbox visual
        /*
        GameObject visualObj = new GameObject("HitboxVisual");
        visualObj.transform.SetParent(transform, false);
        hitboxVisual = visualObj.AddComponent<HitboxVisual>();
        hitboxVisual.GenerateFromCollider(hitbox);
        */

        // Get the weapon's material
        if (weaponVisual != null)
        {
            weaponRenderer = weaponVisual.GetComponent<SpriteRenderer>();
            if (weaponRenderer != null)
            {
                weaponMaterial = weaponRenderer.material;
            }
        }
    }

    void Update()
    {
        if (player != null && !isAttacking) // Only update position when not attacking
        {
            UpdateWeaponPositionAndRotation();
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && PlayerController.canMove)
        {
            StartAttack();
        }
    }

    private void UpdateWeaponPositionAndRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 directionToMouse = (mousePosition - player.position).normalized;

        transform.position = player.position + directionToMouse * distanceFromPlayer;
        weaponRenderer.flipY = directionToMouse.x < 0f;

        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void StartAttack()
    {
        PlayerController.isUsingWeapon = true;
        nextAttackTime = Time.time + weaponData.attackCooldown;
        hitEnemies.Clear();

        // Get attack direction
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 directionToMouse = ((Vector2)(mousePosition - player.position)).normalized;

        // Update player facing direction
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.FaceTowards(directionToMouse);
        }

        SoundManager.Instance.PlaySfx(SoundManager.Instance.chainsawAttack);

        ThrustWeapon(() =>
        {
            PlayerController.isUsingWeapon = false;
        });
    }

    private void ThrustWeapon(TweenCallback onComplete)
    {
        isAttacking = true;
        hitbox.enabled = true;

        // Get direction towards mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 attackDirection = (mousePosition - player.position).normalized;

        if (hitboxVisual != null) hitboxVisual.StartWaveEffect(attackDirection);

        // Show outline
        if (weaponMaterial != null && shouldShowOutline)
        {
            DOTween.To(() => weaponMaterial.GetFloat(OutlineAlpha),
                x => weaponMaterial.SetFloat(OutlineAlpha, x),
                1f, 0.1f)  // Quick fade in
                .OnComplete(() =>
                {
                    // Fade out after a short delay
                    DOTween.To(() => weaponMaterial.GetFloat(OutlineAlpha),
                        x => weaponMaterial.SetFloat(OutlineAlpha, x),
                        0f, 0.3f)
                        .SetDelay(0.2f);  // Wait a bit before fading out
                });
        }

        // Store the original position and rotation
        Vector3 originalPos = transform.position;
        Quaternion originalRot = transform.rotation;

        // Calculate thrust positions
        Vector3 recoilPos = originalPos - (attackDirection * 0.3f);
        Vector3 thrustPos = originalPos + (attackDirection * attackSwingDistance);

        // Create thrust sequence
        Sequence thrustSequence = DOTween.Sequence();

        // Quick recoil back
        thrustSequence.Append(transform.DOMove(recoilPos, 0.1f)
            .SetEase(Ease.OutQuad));

        // Powerful thrust forward
        thrustSequence.Append(transform.DOMove(thrustPos, 0.15f)
            .SetEase(Ease.OutCubic));

        // Chainsaw vibration effect
        thrustSequence.AppendCallback(() =>
        {
            transform.DOShakePosition(0.2f, 0.15f, 30, 90, false, true);
        });

        // Return to original position
        thrustSequence.Append(transform.DOMove(originalPos, 0.15f)
            .SetEase(Ease.OutQuad));

        // Complete the attack
        thrustSequence.OnComplete(() =>
        {
            transform.position = originalPos;
            transform.rotation = originalRot;
            hitbox.enabled = false;
            isAttacking = false;
            onComplete?.Invoke();
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.gameObject.TryGetComponent(out Health health))
        {
            if (!hitEnemies.Contains(collision.gameObject))
            {
                hitEnemies.Add(collision.gameObject);
                health.TakeDamage(weaponData.damage, player.position);
                SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.zombieHitSounds);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw the hitbox in the editor
        Gizmos.color = Color.green;
        if (hitbox is BoxCollider2D boxCollider)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.offset, boxCollider.size);
        }
        else if (hitbox is CircleCollider2D circleCollider)
        {
            Gizmos.DrawWireSphere((Vector2)transform.position + circleCollider.offset, circleCollider.radius);
        }
    }
}
