using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PvCdestroy_Object : MonoBehaviour
{
   // The settings
    [SerializeField] private float _maxLifetime = 3.0f;
    [SerializeField] private float _speed = 50.0f;
    [SerializeField] private LayerMask _playerLayer;

    // The countdown for a bullet lifetime.
    //[Networked] private TickTimer _currentLifetime { get; set; }

    private Collider2D _hitCollider;
    [SerializeField] private Collider2D _collider;

    private bool isLifetime;

    //private Rigidbody2D _rigidbody = null;

    public void Start()
    {

        // The network parameters get initializes by the host. These will be propagated to the clients since the
        // variables are [Networked]
        //_currentLifetime = TickTimer.CreateFromSeconds(Runner, _maxLifetime);
        StartCoroutine(CurrentLifetime(_maxLifetime));
    }

    public void FixedUpdate()
    {
        if(HasHitPlayer())return;

        CheckLifetime();
    }

    // If the bullet has exceeded its lifetime, it gets destroyed
    private void CheckLifetime()
    {
        Debug.Log(isLifetime);
        if (!isLifetime)
        {
            Destroy(gameObject);
        }
    }

    // Check if the bullet will hit an asteroid in the next tick.
    private bool HasHitPlayer()
    {

        _hitCollider = Physics2D.OverlapBox(_collider.transform.position, _collider.bounds.size * 0.5f, 0, LayerMask.GetMask("MultPlayer"));
        if (_hitCollider != default)
        {
            if (_hitCollider.tag.Equals("Player"))
            {
                var playerBehaviour = _hitCollider.GetComponent<Damage_Player>();
                playerBehaviour.Damage(this.gameObject.tag);
                Destroy(gameObject);
                return true;

            }
        }
        return false;
    }

    private IEnumerator CurrentLifetime(float duration)
    {
        isLifetime = true;
        yield return new WaitForSeconds(duration);
        isLifetime = false;
    }
}
