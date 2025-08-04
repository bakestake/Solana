using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.Burst.CompilerServices;

public class WeaponDamage : NetworkBehaviour
{
    [SerializeField] private PVP_PlayerMovement _playerMovement;
    private Collider2D _hitCollider;
    [SerializeField] private Collider2D _collider;

    private Vector2 AttackDirection;

    public float normalDamage,comboDamage;


    public override void FixedUpdateNetwork()
    {
        HasInteractedPlayer();
    }

    private void HasInteractedPlayer()
    {
        _hitCollider = Runner.GetPhysicsScene2D().OverlapBox(_collider.transform.position, _collider.bounds.size * 0.5f, 0, LayerMask.GetMask("MultPlayer"));
        if (_hitCollider != default)
        {
            if (_hitCollider.tag.Equals("Player") && _hitCollider.transform.root != transform.root && _playerMovement.GetIsAttacking())
            {
                if (transform.root.position.x < _hitCollider.transform.position.x)
                {
                    AttackDirection = Vector2.left;
                }
                else
                {
                    AttackDirection = Vector2.right;
                }
                var playerBehaviour = _hitCollider.GetComponent<Damage_Player>();
                //Increase attack of the player
                _playerMovement.attackAmount += 1;
                if (_playerMovement.comboDamage)
                {
                    playerBehaviour.WeaponCollisions(comboDamage,AttackDirection, true);
                }
                else
                {
                    playerBehaviour.WeaponCollisions(normalDamage,AttackDirection);
                }
            }
        }
        
    }

}
