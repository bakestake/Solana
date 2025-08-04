using Gamegaard.Singleton;
using UnityEngine;

namespace COT
{
    public class AudioManager : MonoBehaviourSingleton<AudioManager>
    {
        [SerializeField] private AudioSource sfxSource;

        public void PlaySound(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}