using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Mult_BusScript : NetworkBehaviour
{
    [SerializeField] FastTravelPanel fastTravelPanel;
    [SerializeField] string currentStation;
    private Collider2D _hitCollider;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private LayerMask _playerLayer;

    public void OpenBus()
    {
        fastTravelPanel.currentStation = currentStation;
        fastTravelPanel.GetComponent<Animator>().SetTrigger("in");
        fastTravelPanel.SetButtons();

        SoundManager.instance.PlaySfx(SoundManager.instance.fastTravelIn);
    }

    public void CloseBus()
    {
        fastTravelPanel.GetComponent<Animator>().SetTrigger("out");
    }

    public void StartFastTravel(Vector2 FastTravelPosition)
    {
        StartCoroutine(TravelRoutine(FastTravelPosition));
    }
    private IEnumerator TravelRoutine(Vector2 FastTravelPosition)
    {
        SoundManager.instance.PlaySfx(SoundManager.instance.transition);
        ServerGameManager.Instance.transitionAnimator.SetTrigger("in");

        _hitCollider = Runner.GetPhysicsScene2D().OverlapBox(_collider.transform.position, _collider.bounds.size * 2f, 0, LayerMask.GetMask("MultPlayer"));
        if (_hitCollider != default)
        {
            if (_hitCollider.tag.Equals("Player"))
            {
                var playerMovementBehaviour = _hitCollider.GetComponent<Mult_PlayerMovementController>();
                playerMovementBehaviour.PlayerFastTravel(FastTravelPosition);
            }
        }

        yield return new WaitForSeconds(1.0f);
        ServerGameManager.Instance.transitionAnimator.SetTrigger("out");
        CloseBus();
        SoundManager.instance.PlaySfx(SoundManager.instance.fastTravelOut);
    }
}
