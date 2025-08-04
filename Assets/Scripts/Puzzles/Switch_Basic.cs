using UnityEngine;
using UnityEngine.Events;

public class Switch_Basic : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected bool isToggled;
    [SerializeField] protected bool invokeOnStart;

    [Header("Behaviours")]
    [SerializeField] protected bool isRaySwitch;

    [Header("Events")]
    [SerializeField] private UnityEvent onToggleON;
    [SerializeField] private UnityEvent onToggleOFF;

    [Header("References")]
    [SerializeField] private Sprite on_sprite;
    [SerializeField] private Sprite off_sprite;

    public bool IsRaySwitch => isRaySwitch;
    private SpriteRenderer spriteRenderer;
    private Interact interact;

    private void OnValidate()
    {
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (!spriteRenderer)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        UpdateSprite();
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        interact = GetComponent<Interact>();
    }

    private void Start()
    {
        UpdateSprite();

        if (invokeOnStart) SetToggle(isToggled);
    }

    public void SwitchToggle()
    {
        SetToggle(!isToggled);
    }

    public void SetToggle(bool toggle)
    {
        isToggled = toggle;

        UpdateSprite();

        if (isToggled)
        {
            onToggleON.Invoke();
        }
        else
        {
            onToggleOFF.Invoke();
        }
    }

    public virtual void UpdateSprite()
    {
        if (spriteRenderer)
        {
            if (Application.isPlaying)
            {
                spriteRenderer.enabled = true;
                if (interact) interact.enabled = true;
            }

            spriteRenderer.sprite = isToggled ? on_sprite : off_sprite;
        }
    }
}