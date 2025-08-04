using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class WaterPipe : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PipeType pipe;
    [SerializeField] private bool isValve;
    [SerializeField] private bool valveToggled;
    [SerializeField] private Sprite[] sprites_off = new Sprite[7];
    [SerializeField] private Sprite[] sprites_on = new Sprite[7];

    [Header("Valve")]
    [SerializeField] private SpriteRenderer valve_spriteRenederer;
    [SerializeField] private Sprite valve_off;
    [SerializeField] private Sprite valve_on;

    [Header("Events")]
    [SerializeField] private UnityEvent onToggleON;
    [SerializeField] private UnityEvent onToggleOFF;

    [Header("References")]
    [SerializeField] private Collider2D interactCollider;
    [SerializeField] private PipeConnector pipeConnector;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void CheckConnector()
    {
        UpdateSprite();

        if (!pipeConnector) return;

        List<Collider2D> connections = pipeConnector.CheckConnections(pipeConnector.last_connections);

        foreach (Collider2D c in connections)
        {
            PipeConnector connector = c.gameObject.GetComponentInParent<PipeConnector>();

            if (connector != null)
            {
                connector.last_connections = pipeConnector.Connections_array;
                if (isValve)
                {
                    if (valveToggled) connector.SetWater(pipeConnector.HasWater);
                    else connector.SetWater(false);
                }
                else connector.SetWater(pipeConnector.HasWater);
            }
        }
    }

    public void UpdateSprite()
    {
        interactCollider.enabled = isValve;

        if (spriteRenderer)
        {
            spriteRenderer.sprite = pipeConnector.HasWater ? sprites_on[(int)pipe] : sprites_off[(int)pipe];
        }

        if (valve_spriteRenederer)
        {
            valve_spriteRenederer.enabled = isValve;
            valve_spriteRenederer.sprite = valveToggled ? valve_on : valve_off;
        }
    }

    public void SwitchToggle()
    {
        SetToggle(!valveToggled);
    }

    public void SetToggle(bool toggle)
    {
        UpdateSprite();

        if (valveToggled == toggle) return;
        valveToggled = toggle;

        if (valveToggled)
        {
            onToggleON.Invoke();
        }
        else
        {
            onToggleOFF.Invoke();
        }
    }
}

public partial class WaterPipe : MonoBehaviour
{
    private void OnValidate()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateSprite();
        UpdateConnectors();
    }

    private void UpdateConnectors()
    {
        name = $"Blue_WaterPipe [{pipe}]";

        switch (pipe)
        {
            case PipeType.Vertical:
                pipeConnector.Connection_top.enabled = true;
                pipeConnector.Connection_right.enabled = false;
                pipeConnector.Connection_bottom.enabled = true;
                pipeConnector.Connection_left.enabled = false;
                break;
            case PipeType.Horizontal:
                pipeConnector.Connection_top.enabled = false;
                pipeConnector.Connection_right.enabled = true;
                pipeConnector.Connection_bottom.enabled = false;
                pipeConnector.Connection_left.enabled = true;
                break;
            case PipeType.Corner_TopRight:
                pipeConnector.Connection_top.enabled = true;
                pipeConnector.Connection_right.enabled = true;
                pipeConnector.Connection_bottom.enabled = false;
                pipeConnector.Connection_left.enabled = false;
                break;
            case PipeType.Cornet_TopLeft:
                pipeConnector.Connection_top.enabled = true;
                pipeConnector.Connection_right.enabled = false;
                pipeConnector.Connection_bottom.enabled = false;
                pipeConnector.Connection_left.enabled = true;
                break;
            case PipeType.Corner_BottomRight:
                pipeConnector.Connection_top.enabled = false;
                pipeConnector.Connection_right.enabled = true;
                pipeConnector.Connection_bottom.enabled = true;
                pipeConnector.Connection_left.enabled = false;
                break;
            case PipeType.Corner_BottomLeft:
                pipeConnector.Connection_top.enabled = false;
                pipeConnector.Connection_right.enabled = false;
                pipeConnector.Connection_bottom.enabled = true;
                pipeConnector.Connection_left.enabled = true;
                break;
            case PipeType.AllSides:
                pipeConnector.Connection_top.enabled = true;
                pipeConnector.Connection_right.enabled = true;
                pipeConnector.Connection_bottom.enabled = true;
                pipeConnector.Connection_left.enabled = true;
                break;
        }
    }
}
