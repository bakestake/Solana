using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PVCStoner : PvC_Ultimate
{
    [SerializeField] private GameObject smokePrefab;

    private bool IsFire;

    private bool CanUltimate = false;

    public int smokeSpeed;
    public Transform smokeFirePoint;


    public void FixedUpdate()
    {
        if (CanUltimate && !IsFire)
        {
            ThrowSmoke();
            CanUltimate = false;
        }
    }

    public override void ExecuteSpecialAbility()
    {
        Debug.Log("Stoner special ability working.");
        CanUltimate = true;
        StartCoroutine(FireCooldown(1.1f));
    }
    public override void StopAbility()
    {
        Debug.Log("Stoner special ability is over.");
    }
    void ThrowSmoke()
    {
        // smoke spawn
        //smoke = Instantiate(smokePrefab, smokeFirePoint.position, smokeFirePoint.rotation);
        var smoke = Instantiate(smokePrefab, smokeFirePoint.position, smokeFirePoint.rotation);

        //// Throw the bullet forward
        Rigidbody2D smokerb = smoke.GetComponent<Rigidbody2D>();

        if (GetPlayerMovement().GetIsFacingRight())
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
    private IEnumerator FireCooldown(float duration)
    {
        IsFire = true;
        yield return new WaitForSeconds(duration);
        IsFire = false;
    }
}
