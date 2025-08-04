using System.Collections;
using UnityEngine;

public class FireEmitter : MonoBehaviour
{
    [SerializeField] private Sprite on_sprite;
    [SerializeField] private Sprite off_sprite;
    [SerializeField] private bool toggled;

    [SerializeField] private BoxCollider2D fireCollider;
    [SerializeField] private BoxCollider2D heatArea;

    private SpriteRenderer spriteRenderer;
    private bool canDamage = true;

    public bool Toggled => toggled;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (toggled)
        {
            MeltIce(true);
        }
    }

    private void OnValidate()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateFire();
    }

    public void SetToggle(bool toggle)
    {
        toggled = toggle;

        if (!toggled)
        {
            MeltIce(false);
        }

        UpdateFire();
    }

    private void MeltIce(bool doMelt)
    {
        Collider2D[] objectsInRange = Physics2D.OverlapBoxAll(heatArea.bounds.center, heatArea.bounds.size, 0f);

        foreach (Collider2D col in objectsInRange)
        {
            IceBlock ice = col.GetComponent<IceBlock>();

            if (ice)
            {
                if (doMelt && !ice.IsMelting && !ice.Melted)
                {
                    ice.ToggleMelting(true);
                }
                if (!doMelt && ice.IsMelting)
                {
                    ice.ToggleMelting(false);
                }
            }
        }
    }

    private void UpdateFire()
    {
        if (toggled)
        {
            spriteRenderer.sprite = on_sprite;
            fireCollider.enabled = true;
            heatArea.enabled = true;
        }
        else
        {
            spriteRenderer.sprite = off_sprite;
            fireCollider.enabled = false;
            heatArea.enabled = false;
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(1);
        canDamage = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!toggled || !canDamage) return;
        Health health = collision.gameObject.GetComponent<Health>();
        if (health && toggled && canDamage)
        {
            health.TakeDamage(10, transform.position);
            StartCoroutine(DamageCooldown());
        }
    }
}
