using System.Collections.Generic;
using UnityEngine;

public class PlayNamedSound : MonoBehaviour
{
    [SerializeField] private List<NamedSound> sounds;

    private Dictionary<string, AudioClip> soundsByName = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        foreach (NamedSound sound in sounds)
        {
            soundsByName.Add(sound.key, sound.audioClip);
        }
    }

    public void PlaySfx(string name)
    {
        if (soundsByName.TryGetValue(name, out AudioClip clip))
        {
            SoundManager.Instance.PlaySfx(clip);
        }
    }
}