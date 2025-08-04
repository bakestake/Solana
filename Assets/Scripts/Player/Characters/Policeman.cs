using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Policeman : Ultimate
{
    [SerializeField] private NetworkPrefabRef laserPrefab = NetworkPrefabRef.Empty;

    [Networked]
    private TickTimer Fire { get; set; }

    private bool CanUltimate = false;

    public float maxDistance = 50f;

    public Transform startPoint;

    public override void FixedUpdateNetwork()
    {
        if (CanUltimate && Fire.ExpiredOrNotRunning(Runner))
        {
            FireLaser();
            CanUltimate = false;
        }
    }

    public override void ExecuteSpecialAbility()
    {
        Debug.Log("Policeman special ability working.");
        //StartCoroutine(wait());
        CanUltimate = true;
        Fire = TickTimer.CreateFromSeconds(Runner, 0.1f);
    }
    public override void StopAbility()
    {
        Debug.Log("Policeman special ability is over.");
    }
    void FireLaser()
    {
        Runner.Spawn(laserPrefab, startPoint.position, Quaternion.identity, Object.InputAuthority);
        //Lazer spawn
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.laser);
    }
}
