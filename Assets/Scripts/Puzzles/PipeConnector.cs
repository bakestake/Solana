using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PipeConnector : MonoBehaviour
{
    [SerializeField] protected bool hasWater;
    [SerializeField] protected LayerMask overlapLayerMask;

    [Header("Conections")]
    [SerializeField] private BoxCollider2D connection_top;
    [SerializeField] private BoxCollider2D connection_right;
    [SerializeField] private BoxCollider2D connection_bottom;
    [SerializeField] private BoxCollider2D connection_left;
    [SerializeField] private BoxCollider2D[] connections_array;

    [Header("Events")]
    [SerializeField] protected UnityEvent onWaterON;
    [SerializeField] protected UnityEvent onWaterOFF;
    [HideInInspector] public BoxCollider2D[] last_connections;

    public bool HasWater => hasWater;
    public BoxCollider2D[] Connections_array => connections_array;
    public BoxCollider2D Connection_top { get => connection_top; set => connection_top = value; }
    public BoxCollider2D Connection_right { get => connection_right; set => connection_right = value; }
    public BoxCollider2D Connection_bottom { get => connection_bottom; set => connection_bottom = value; }
    public BoxCollider2D Connection_left { get => connection_left; set => connection_left = value; }

    public List<Collider2D> CheckConnections(IReadOnlyCollection<BoxCollider2D> toIgnore)
    {
        List<Collider2D> finalColliders = new List<Collider2D>();
        List<BoxCollider2D> connections = new List<BoxCollider2D>();

        TryAddConecttion(connection_top, ref connections);
        TryAddConecttion(connection_right, ref connections);
        TryAddConecttion(connection_bottom, ref connections);
        TryAddConecttion(connection_left, ref connections);

        foreach (BoxCollider2D boxCollider in connections)
        {
            Vector2 center = boxCollider.bounds.center;
            Vector2 size = boxCollider.bounds.size;

            Collider2D[] overlappingColliders = Physics2D.OverlapBoxAll(center, size, 0f, overlapLayerMask);

            foreach (Collider2D collider in overlappingColliders)
            {
                if (collider == boxCollider || toIgnore.Contains(collider)) continue;
                if (collider.isTrigger)
                {
                    finalColliders.Add(collider);
                }
            }
        }

        return finalColliders;
    }

    private void TryAddConecttion(BoxCollider2D collider, ref List<BoxCollider2D> connections)
    {
        if (collider && collider.enabled)
        {
            connections.Add(collider);
        }
    }

    public void SetWater(bool toggle)
    {
        if (hasWater == toggle) return;

        hasWater = toggle;

        if (hasWater)
        {
            onWaterON.Invoke();
        }
        else
        {
            onWaterOFF.Invoke();
        }
    }
}
