using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class LeaveSession : NetworkBehaviour
{
    private NetworkRunner _runnerInstance = null;

    public async void PlayerLeaveSession()
    {
        _runnerInstance = FindObjectOfType<NetworkRunner>();

        await _runnerInstance.Shutdown();
    }
}
