using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Mult_PlayerVisualController : SimulationBehaviour, ISpawned
{
    [SerializeField] private ParticleSystem dustParticle = null;

    // Colors the ship in the color assigned to the PlayerRef's index
    public void Spawned()
    {
        var playerRef = Object.InputAuthority;
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = GetColor(playerRef.PlayerId);
        }
    }

    public void TriggerDustParticle()
    {
        dustParticle.Play();
    }

    // Defines a set of colors to distinguish between players
    public static Color GetColor(int player)
    {
        switch (player % 8)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.grey;
            case 6: return Color.magenta;
            case 7: return Color.white;
        }
        return Color.black;
    }
}

