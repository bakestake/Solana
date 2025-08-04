using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PvC_Policeman : PvC_Ultimate
{
    [SerializeField] private GameObject laserPrefab;

    //private TickTimer Fire { get; set; }

    private bool CanUltimate = false;

    public float maxDistance = 50f;

    public Transform startPoint;

    private bool FireCooldown;

    public void FixedUpdate()
    {
        if (CanUltimate && !FireCooldown)
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
        StartCoroutine(FireCoroutine(0.1f));
    }
    public override void StopAbility()
    {
        Debug.Log("Policeman special ability is over.");
    }
    void FireLaser()
    {
        Instantiate(laserPrefab, startPoint.position, Quaternion.identity);
        //Lazer spawn
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.laser);
    }

    private IEnumerator FireCoroutine(float duration)
    {
        FireCooldown = true;
        yield return new WaitForSeconds(duration);
        FireCooldown = false;
    }
}
