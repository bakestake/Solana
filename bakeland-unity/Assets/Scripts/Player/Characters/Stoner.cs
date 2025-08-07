using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Stoner : Ultimate
{
    [SerializeField] private NetworkPrefabRef smokePrefab = NetworkPrefabRef.Empty;

    [Networked]
    private TickTimer Fire { get; set; }

    private bool CanUltimate = false;

    public int smokeSpeed;
    public Transform smokeFirePoint;

    private PVP_PlayerMovement _playerMovement = null;


    public override void Spawned()
    {
        // --- Host & Client
        // Set the local runtime references.
        _playerMovement = GetComponent<PVP_PlayerMovement>();
    }

    public override void FixedUpdateNetwork()
    {
        if (CanUltimate && Fire.ExpiredOrNotRunning(Runner))
        {
            ThrowSmoke();
            CanUltimate = false;
        }
    }

    public override void ExecuteSpecialAbility()
    {
        Debug.Log("Stoner special ability working.");
        CanUltimate = true;
        Fire = TickTimer.CreateFromSeconds(Runner, 1.1f);
    }
    public override void StopAbility()
    {
        Debug.Log("Stoner special ability is over.");
    }
    void ThrowSmoke()
    {
        // smoke spawn
        //smoke = Instantiate(smokePrefab, smokeFirePoint.position, smokeFirePoint.rotation);
        var smoke = Runner.Spawn(smokePrefab, smokeFirePoint.position, smokeFirePoint.rotation, Object.InputAuthority);

        //// Throw the bullet forward
        Rigidbody2D smokerb = smoke.GetComponent<Rigidbody2D>();

        if (_playerMovement.GetIsFacingRight())
            smokerb.velocity = transform.right * smokeSpeed;
        else
            smokerb.velocity = -transform.right * smokeSpeed;


        //Audio_Manager.instance.PlaySoundEffect(Audio_Manager.instance.smoke, 3f);
    }
    //IEnumerator wait() //Ulti attack anim duration
    //{
    //    anim.SetBool("isUlti", true);
    //    yield return new WaitForSeconds(1.1f);
    //    ThrowSmoke();
    //    yield return new WaitForSeconds(0.2f);
    //    anim.SetBool("isUlti", false);

    //}
}
