using System.Collections;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    [SerializeField] private float max_hp = 4;
    [SerializeField] private float hp = 4;
    [SerializeField] private Gradient hpGradient = new Gradient();
    [SerializeField] private Sprite iceSprite;
    [SerializeField] private Sprite meltedSprite;

    private int meltTimeInSeconds = 1;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Coroutine meltCoroutine;
    public bool IsMelting { get; private set; }
    public bool Melted { get; private set; }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnValidate()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (!boxCollider)
            boxCollider = GetComponent<BoxCollider2D>();

        UpdateIce();
        UpdateColor();
    }

    public void ToggleMelting(bool toggle)
    {
        if (toggle)
        {
            if (!Melted)
            {
                IsMelting = true;
                meltCoroutine = StartCoroutine(MeltTick());
            }
            UpdateIce();
        }
        else
        {
            IsMelting = false;
            StopCoroutine(meltCoroutine);
            UpdateIce();
        }
    }

    private void UpdateIce()
    {
        if (Melted)
        {
            spriteRenderer.sprite = meltedSprite;
            boxCollider.enabled = false;
        }
        else
        {
            spriteRenderer.sprite = iceSprite;
            boxCollider.enabled = true;
        }
    }

    private void UpdateColor()
    {
        if (!Melted)
        {
            var t = (hp - 0) / (max_hp - 0);
            spriteRenderer.color = hpGradient.Evaluate(t);
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    private IEnumerator MeltTick()
    {
        yield return new WaitForSeconds(meltTimeInSeconds);

        if (hp >= 0)
        {
            hp--;
            UpdateColor();
        }

        if (hp < 0)
        {
            Melted = true;
            ToggleMelting(false);
        }
        else
        {
            meltCoroutine = StartCoroutine(MeltTick());
        }
    }
}
