using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
    [SerializeField] private AudioClip ambienceToChange;

    private void Start()
    {
        if (!SoundManager.HasInstance) return;

        if (ambienceToChange != null) SoundManager.Instance.ChangeAmbience(ambienceToChange);
        else SoundManager.Instance.StopAmbience();
    }
}