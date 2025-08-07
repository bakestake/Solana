using System.Collections;
using UnityEngine;

public class PVCFarmer : PvC_Ultimate
{
    [SerializeField] private GameObject _bullet;

    private bool IsFire = false;

    private bool CanUltimate = false;

    public int bulletSpeed;

    public Transform bulletFirePoint;

    //private PvC_PlayerMovement _playerMovement = null;

    //public void Start()
    //{
    //    // --- Host & Client
    //    // Set the local runtime references.
    //    _playerMovement = GetComponent<PvC_PlayerMovement>();
    //}

    public void FixedUpdate()
    {
        if (CanUltimate && !IsFire)
        {
            FireBullet();
            CanUltimate = false;
        }
    }

    public override void ExecuteSpecialAbility()
    {
        Debug.Log("Farmer special ability working.");
        base.ExecuteSpecialAbility();
        //StartCoroutine(wait());
        CanUltimate = true;
        StartCoroutine(FireCooldown(0.9f));
    }
    public override void StopAbility()
    {
        Debug.Log("Farmer  special ability is over.");
        base.StopAbility();
    }

    public void FireBullet()
    {
        Debug.Log("Bullet is spawned");
        //if (!Runner.CanSpawn) return;
        var bullet = Instantiate(_bullet, bulletFirePoint.position, bulletFirePoint.rotation);
        // Bullet spawn

        //Audio_Manager.instance.PlaySound(Audio_Manager.instance.shotgun);

        //// Throw the bullet forward
        Rigidbody2D bulletrb = bullet.GetComponent<Rigidbody2D>();


        if (GetPlayerMovement().GetIsFacingRight())
        {
            bulletrb.velocity = transform.right * bulletSpeed;
            //bulletrb.Rigidbody.AddForce(Vector2.right * _speed * Runner.DeltaTime, ForceMode2D.Force);
        }
        else
            bulletrb.velocity = -transform.right * bulletSpeed;
    }

    private IEnumerator FireCooldown(float duration)
    {
        IsFire = true;
        yield return new WaitForSeconds(duration);
        IsFire = false;
    }
}
