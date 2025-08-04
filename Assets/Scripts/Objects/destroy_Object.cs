using UnityEngine;
using Fusion;

public class destroy_Object : NetworkBehaviour
{
   // The settings
    [SerializeField] private float _maxLifetime = 3.0f;
    //[SerializeField] private float _speed = 50.0f;
    [SerializeField] private LayerMask _playerLayer;

    // The countdown for a bullet lifetime.
    [Networked] private TickTimer _currentLifetime { get; set; }

    private Collider2D _hitCollider;
    [SerializeField] private Collider2D _collider;

    //private Rigidbody2D _rigidbody = null;

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;

        // The network parameters get initializes by the host. These will be propagated to the clients since the
        // variables are [Networked]
        _currentLifetime = TickTimer.CreateFromSeconds(Runner, _maxLifetime);
    }

    public override void FixedUpdateNetwork()
    {
        if(HasHitPlayer())return;

        CheckLifetime();
    }

    // If the bullet has exceeded its lifetime, it gets destroyed
    private void CheckLifetime()
    {
        if (!Object.IsValid) return;
        if (_currentLifetime.Expired(Runner) == false) return;
        Runner.Despawn(Object);
    }

    // Check if the bullet will hit an asteroid in the next tick.
    private bool HasHitPlayer()
    {
        //var hitplayer = Runner.GetPhysicsScene2D().Raycast(transform.position, transform.right, _speed * Runner.DeltaTime, _playerLayer);

        //if (hitplayer == false) return false;

        //var player = hitplayer.rigidbody.gameObject.GetComponent<Damage_Player>();

        ////Check if isAlive

        //player.Damage(this.gameObject.tag);

        //return true;

        _hitCollider = Runner.GetPhysicsScene2D().OverlapBox(_collider.transform.position, _collider.bounds.size * 0.5f, 0, LayerMask.GetMask("MultPlayer"));
        if (_hitCollider != default)
        {
            if (_hitCollider.tag.Equals("Player"))
            {
                var playerBehaviour = _hitCollider.GetComponent<Damage_Player>();
                playerBehaviour.Damage(this.gameObject.tag);
                Runner.Despawn(Object);
                return true;

            }
        }
        return false;
    }



}
