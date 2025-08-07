using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Fusion;

public class Bus : MonoBehaviour//NetworkBehaviour
{
    bool isPlayerNear;
    [SerializeField] FastTravelPanel fastTravelPanel;
    [SerializeField] string currentStation;
    private Collider2D _hitCollider;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private LayerMask _playerLayer;

    void Start()
    {
        isPlayerNear = false;
    }

    public void OpenBus()
    {
        fastTravelPanel.currentStation = currentStation;
        fastTravelPanel.GetComponent<Animator>().SetTrigger("in");
        fastTravelPanel.SetButtons();
        PlayerController.canMove = false;

        SoundManager.instance.PlaySfx(SoundManager.instance.fastTravelIn);
    }

    public void CloseBus()
    {
        fastTravelPanel.GetComponent<Animator>().SetTrigger("out");
        PlayerController.canMove = true;
        Debug.Log("Can Move true");
    }

    public void StartFastTravel(Vector2 FastTravelPosition)
    {
        StartCoroutine(TravelRoutine(FastTravelPosition));
    }
    private IEnumerator TravelRoutine(Vector2 FastTravelPosition)
    {
        //SoundManager.instance.PlaySfx(SoundManager.instance.transition);
        ////ServerGameManager.Instance.transitionAnimator.SetTrigger("in");
        //fastTravelPanel.GetComponent<Animator>().SetTrigger("out");
        //yield return new WaitForSeconds(.5f);

        ////player.position = fastTravelPoint.position;
        ////PlayerController.direction = new Vector2(0, -1);
        ////PlayerController.canMove = true;

        //_hitCollider = Runner.GetPhysicsScene2D().OverlapBox(_collider.transform.position, _collider.bounds.size * 0.5f, 0, LayerMask.GetMask("MultPlayer"));
        //Debug.Log("The _hit collider" + _hitCollider);
        //if (_hitCollider != default)
        //{
        //    Debug.Log("Not default");
        //    if (_hitCollider.tag.Equals("Player"))
        //    {
        //        var playerMovementBehaviour = _hitCollider.GetComponent<Mult_PlayerMovementController>();
        //        playerMovementBehaviour.PlayerFastTravel(FastTravelPosition);

        //    }
        //}


        //ServerGameManager.Instance.transitionAnimator.SetTrigger("out");
        //SoundManager.instance.PlaySfx(SoundManager.instance.fastTravelOut);
        yield return null;
    }


}
