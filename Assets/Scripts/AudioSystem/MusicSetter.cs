using UnityEngine;

public class MusicSetter : MonoBehaviour
{
    [SerializeField] private AudioClip musicToChange;
    [SerializeField] private bool stopMusic;
    [SerializeField] private AudioClip ambienceToChange;
    [SerializeField] private bool stopAmbience;

    public void Apply()
    {
        if (!SoundManager.HasInstance) return;

        if (stopMusic) SoundManager.Instance.StopMusic();
        else if (musicToChange != null) SoundManager.Instance.ChangeMusic(musicToChange);

        if (stopAmbience) SoundManager.Instance.StopAmbience();
        else if (ambienceToChange != null) SoundManager.Instance.ChangeAmbience(ambienceToChange);
    }
}
