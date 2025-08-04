using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip sfx;

    public AudioClip Sfx => sfx;

    public void PlaySfx()
    {
        if (sfx == null) return;
        SoundManager.Instance.PlaySfx(sfx);
    }

    public void SetSFX(AudioClip sfx)
    {
        this.sfx = sfx;
    }
}
