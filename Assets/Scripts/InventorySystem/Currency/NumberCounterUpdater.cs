using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCounterUpdater : MonoBehaviour
{
    public NumberCounter numberCounter;

    public void SetValue(int value)
    {
        numberCounter.Value = value;
    }

    public void PlayPopSound()
    {
        // SoundManager.instance.PlayRandomFromListRandomPitch(SoundManager.instance.popSounds);
    }
}
