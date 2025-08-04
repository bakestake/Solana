using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;


public class Mult_LocalcameraHandler : NetworkBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtual;
    private Transform PlayerToFollow = null;
    // Start is called before the first frame update

    public override void Spawned()
    {
        //Get CinemachineBrain
        //var cameraBrain = GameObject.FindGameObjectWithTag("MainCamera");
        //Get Virtual Camera
        cinemachineVirtual = GetComponentInChildren<CinemachineVirtualCamera>();
        if (cinemachineVirtual == null)
        {
            DebugUtils.LogError("The camera is not initialised");
            return;
        }

        if (Object.HasInputAuthority)
        {
            cinemachineVirtual.Priority = 10;
        }
        else
        {
            cinemachineVirtual.Priority = 0;
        }
    }
    public void SetCameraManager(Transform playerObject)
    {
        if (cinemachineVirtual != null && playerObject != null)
        {
            cinemachineVirtual.Follow = playerObject;
            PlayerToFollow = playerObject;
        }
    }
}
