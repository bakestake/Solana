using System.Collections.Generic;
using UnityEngine;

public class Switch_Water : Switch_Basic
{
    [SerializeField] private PipeConnector pipeConnector;

    public void CheckConnector()
    {
        SetToggle(pipeConnector.HasWater);
    }

    public void EmitWater(bool emit)
    {
        if (pipeConnector == null) return;
        pipeConnector.SetWater(isToggled);

        List<Collider2D> connections = pipeConnector.CheckConnections(pipeConnector.Connections_array);

        foreach (Collider2D c in connections)
        {
            PipeConnector connector = c.gameObject.GetComponentInParent<PipeConnector>();

            if (connector != null)
            {
                connector.last_connections = pipeConnector.Connections_array;
                connector.SetWater(emit);
            }
        }
    }
}
