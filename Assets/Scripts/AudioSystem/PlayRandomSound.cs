using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    public AudioClip[] Clips => clips;

    public void PlaySfx()
    {
        Debug.Log(clips);
        Debug.Log(clips.Length);
        if (clips == null) return;
        SoundManager.Instance.PlayRandomFromList(clips);
    }

    public void SetSFX(AudioClip[] clips)
    {
        this.clips = clips;
    }
}