using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerObject : NetworkBehaviour
{
    public static PlayerObject Local { get; private set; }

    [Networked]
    public PlayerRef Ref { get; set; }

    [Networked]
    public string overrideController { get; set; }

    //[Networked]
    //public Sprite _Sprite { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {

        }

        if (Object.HasInputAuthority)
        {
            Local = this;
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Local == this) Local = null;
    }

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void Rpc_SetColor(Animator animator)
    //{
    //    _Animator = animator;
    //}

    //[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    //public void Rpc_SetColor(Sprite sprite)
    //{
    //    _Sprite = sprite;
    //}
}
