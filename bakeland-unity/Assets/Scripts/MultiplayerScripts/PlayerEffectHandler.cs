using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEffectHandler : MonoBehaviour
{
    public UnityEvent onPlayEffects;

    public void PlayFootstepSound()
    {
        onPlayEffects?.Invoke();
    }
}
