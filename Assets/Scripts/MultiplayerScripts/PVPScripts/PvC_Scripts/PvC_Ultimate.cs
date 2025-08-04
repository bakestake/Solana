using System.Collections;
using UnityEngine;

public class PvC_Ultimate : MonoBehaviour
{
    public float ultimateTime;

    private PvC_Health_Playerr _healthPlayer = null;
    private PvC_PlayerMovement _playerMovement = null;

    public void Start()
    {
        // --- Host & Client
        // Set the local runtime references.
        _healthPlayer = GetComponent<PvC_Health_Playerr>();
        _playerMovement = GetComponent<PvC_PlayerMovement>();
    }

    public void bongAudio()
    {
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.bong_hit);
    }
    public virtual void ExecuteSpecialAbility()
    {
        if (_healthPlayer == null)
        {
            _healthPlayer = GetComponent<PvC_Health_Playerr>();
        }

        _healthPlayer.ResetDodge(0.0f);

        Debug.Log("Basic special ability working.");
    }
    public virtual void StopAbility()
    {
        Debug.Log("Basic special ability is over.");
    }

    public PvC_PlayerMovement GetPlayerMovement()
    {
        return _playerMovement;
    }
}
