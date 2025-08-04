using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class Ultimate : NetworkBehaviour
{
    public float ultimateTime;

    private Health_Playerr _healthPlayer = null;

    public override void Spawned()
    {
        // --- Host & Client
        // Set the local runtime references.
        _healthPlayer = GetComponent<Health_Playerr>();
    }

    public void bongAudio()
    {
        Audio_Manager.instance.PlaySound(Audio_Manager.instance.bong_hit);
    }
    public virtual void ExecuteSpecialAbility()
    {
        if (_healthPlayer == null)
        {
            _healthPlayer = GetComponent<Health_Playerr>();
        }
        if (Object.HasStateAuthority == false) return;

        _healthPlayer.ResetDodge(0.0f);

        Debug.Log("Basic special ability working.");
    }
    public virtual void StopAbility()
    {
        Debug.Log("Basic special ability is over.");
    }
}
