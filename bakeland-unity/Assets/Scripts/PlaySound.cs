using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip sfx;
    public void PlaySfx()
    {
        if (sfx != null)
        {
            SoundManager.instance.PlaySfx(sfx);
        }
    }
}
