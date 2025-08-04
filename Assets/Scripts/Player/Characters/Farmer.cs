using System.Collections;
using Fusion;
using UnityEngine;
using NetworkRigidbody2D = Fusion.Addons.Physics.NetworkRigidbody2D;

public class Farmer : Ultimate
{
    [SerializeField] private NetworkPrefabRef _bullet = NetworkPrefabRef.Empty;

    [Networked]
    private TickTimer Fire { get; set; }

    private bool CanUltimate = false;

    public int bulletSpeed;

    public Transform bulletFirePoint;

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
            if (Object.HasInputAuthority)
            {
                RPC_FireBullet();
            }
            CanUltimate = false;
        }
    }

    public override void ExecuteSpecialAbility()
    {
        Debug.Log("Farmer special ability working.");
        base.ExecuteSpecialAbility();
        //StartCoroutine(wait());
        CanUltimate = true;
        if (Object.HasStateAuthority == false) return;
        Fire = TickTimer.CreateFromSeconds(Runner, 0.9f);
    }
    public override void StopAbility()
    {
        Debug.Log("Farmer  special ability is over.");
        base.StopAbility();
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_FireBullet()
    {
        Debug.Log("Bullet is spawned");
        //if (!Runner.CanSpawn) return;
        var bullet = Runner.Spawn(_bullet, bulletFirePoint.position, bulletFirePoint.rotation, Object.InputAuthority);
        // Bullet spawn

        //Audio_Manager.instance.PlaySound(Audio_Manager.instance.shotgun);

        //// Throw the bullet forward
        NetworkRigidbody2D bulletrb = bullet.GetComponent<NetworkRigidbody2D>();

        Debug.Log("Inside Farmer" + _playerMovement.GetIsFacingRight());

        if (_playerMovement.GetIsFacingRight())
        {
            bulletrb.Rigidbody.velocity = transform.right * bulletSpeed;
            //bulletrb.Rigidbody.AddForce(Vector2.right * _speed * Runner.DeltaTime, ForceMode2D.Force);
        }
        else
            bulletrb.Rigidbody.velocity = -transform.right * bulletSpeed;
    }
    private void FireBullet()
    {
        Debug.Log("Bullet is spawned");
        if (!Runner.CanSpawn) return;
        var bullet = Runner.Spawn(_bullet, bulletFirePoint.position, bulletFirePoint.rotation, Object.InputAuthority);
        // Bullet spawn

        //Audio_Manager.instance.PlaySound(Audio_Manager.instance.shotgun);

        //// Throw the bullet forward
        NetworkRigidbody2D bulletrb = bullet.GetComponent<NetworkRigidbody2D>();

        Debug.Log("Inside Farmer" + _playerMovement.GetIsFacingRight());

        if (_playerMovement.GetIsFacingRight())
        {
            bulletrb.Rigidbody.velocity = transform.right * bulletSpeed;
            //bulletrb.Rigidbody.AddForce(Vector2.right * _speed * Runner.DeltaTime, ForceMode2D.Force);
        }
        else
            bulletrb.Rigidbody.velocity = -transform.right * bulletSpeed;
    }
}
