using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource sfxSource;
    public AudioSource sfxRandomPitchSource;
    public AudioSource musicSource;
    public AudioSource spatialSource1;
    public AudioSource spatialSource2;
    public AudioSource spatialSource3;

    [Header("Settings")]
    public float defaultMusicVolume;
    public float defaultSfxVolume;
    // public bool isMusicMuted;
    // public bool isSfxMuted;

    [Header("Single Sounds")]
    public AudioClip handbookIn;
    public AudioClip handbookOut;
    public AudioClip fastTravelIn;
    public AudioClip fastTravelOut;
    public AudioClip faucetIn;
    public AudioClip faucetOut;
    public AudioClip bulletinBoardIn;
    public AudioClip bulletinBoardOut;
    public AudioClip dialogueIn;
    public AudioClip dialogueOut;
    public AudioClip dialogueNext;
    public AudioClip transition;
    public AudioClip farmIn;
    public AudioClip farmOut;
    public AudioClip policeIn;
    public AudioClip policeOut;
    public AudioClip blackMarketIn;
    public AudioClip blackMarketOut;
    public AudioClip playgroundIn;
    public AudioClip playgroundOut;
    public AudioClip txIn;
    public AudioClip txOut;
    public AudioClip phoneIn;
    public AudioClip phoneOut;
    public AudioClip caveEnterSteps;
    public AudioClip inventoryIn;
    public AudioClip inventoryOut;
    public AudioClip bicycleLoop;
    public AudioClip weedUsed;
    public AudioClip weedEnded;
    public AudioClip itemPickup;
    public AudioClip itemDrop;
    public AudioClip potion;
    public AudioClip campfireLit;
    public AudioClip campfireLoop;
    public AudioClip saveGame;
    public AudioClip saveGameFail;
    public AudioClip chainsawStart;
    public AudioClip chainsawEnd;
    public AudioClip chainsawAttack;
    public AudioClip succubusDisappear;

    [Header("Music")]
    public AudioClip cityMusic;
    public AudioClip farmMusic;
    public AudioClip blackMarketMusic;

    [Header("Sound Lists")]
    public AudioClip[] footstepSounds;
    public AudioClip[] popSounds;
    public AudioClip[] clickSounds;
    public AudioClip[] bicycleBellSounds;
    public AudioClip[] coinSounds;
    public AudioClip[] playerHitSounds;
    public AudioClip[] zombieHitSounds;
    public AudioClip[] zombieDeathSounds;

    // [Header("References")]
    // public Image musicMute;
    // public Sprite musicMutedSprite;
    // public Sprite musicUnmutedSprite;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sfxSource.volume = defaultSfxVolume;
        sfxRandomPitchSource.volume = defaultSfxVolume;
        musicSource.volume = defaultMusicVolume;
        // SetMuteStatus();

        PlayMusic(farmMusic);
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySfx(AudioClip clip, AudioSource audioSource)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlaySfxRandomPitch(AudioClip clip)
    {
        sfxRandomPitchSource.pitch = Random.Range(0.6f, 1.1f);
        sfxRandomPitchSource.PlayOneShot(clip);
    }

    public void PlaySfxRandomPitch(AudioClip clip, AudioSource audioSource)
    {
        audioSource.pitch = Random.Range(0.6f, 1.1f);
        audioSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySpatial(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomFromList(AudioClip[] list)
    {
        if (list.Length > 0)
        {
            int rand = Random.Range(0, list.Length);
            PlaySfx(list[rand]);
        }
    }

    public void PlayRandomFromList(AudioClip[] list, AudioSource audioSource)
    {
        if (list.Length > 0)
        {
            int rand = Random.Range(0, list.Length);
            PlaySfx(list[rand], audioSource);
        }
    }

    public void PlayRandomFromListRandomPitch(AudioClip[] list)
    {
        if (list.Length > 0)
        {
            int rand = Random.Range(0, list.Length);
            PlaySfxRandomPitch(list[rand]);
        }
    }

    public void PlayRandomFromListRandomPitch(AudioClip[] list, AudioSource audioSource)
    {
        if (list.Length > 0)
        {
            int rand = Random.Range(0, list.Length);
            PlaySfxRandomPitch(list[rand], audioSource);
        }
    }

    public void ChangeMusic(AudioClip clip)
    {
        StartCoroutine(ChangeMusicRoutine(clip));
    }

    private IEnumerator ChangeMusicRoutine(AudioClip clip)
    {
        if (musicSource.isPlaying)
        {
            if (musicSource.clip == clip) yield break;
            FadeMusicOut(0.5f);
            yield return new WaitForSeconds(0.5f);
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
            FadeMusicIn(0.5f);
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void FadeMusicOut(float time)
    {
        if (!DOTween.IsTweening(musicSource))
        {
            musicSource.DOFade(0, time);
        }
        else
        {
            musicSource.DOComplete();
        }
    }

    public void FadeMusicIn(float time)
    {
        if (!DOTween.IsTweening(musicSource))
        {
            musicSource.DOFade(defaultMusicVolume, time);
        }
        else
        {
            musicSource.DOComplete();
        }
    }

    public void AdjustVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void EnableReverb()
    {
        musicSource.GetComponent<AudioReverbFilter>().enabled = true;
        sfxSource.GetComponent<AudioReverbFilter>().enabled = true;
    }

    public void DisableReverb()
    {
        musicSource.GetComponent<AudioReverbFilter>().enabled = false;
        sfxSource.GetComponent<AudioReverbFilter>().enabled = false;
    }

    // public void ToggleMusic()
    // {
    //     int currentMusicMute = PlayerPrefs.GetInt("musicmute");

    //     if (currentMusicMute == 0)
    //     {
    //         // MUTE BUTTON
    //         musicSource.mute = true;
    //         // musicMute.sprite = musicMutedSprite;
    //         PlayerPrefs.SetInt("musicmute", 1);
    //     }
    //     else if (currentMusicMute == 1)
    //     {
    //         // UNMUTE BUTTON
    //         musicSource.mute = false;
    //         // musicMute.sprite = musicUnmutedSprite;
    //         PlayerPrefs.SetInt("musicmute", 0);
    //     }
    // }

    // private void SetMuteStatus()
    // {
    //     int currentMusicMute = PlayerPrefs.GetInt("musicmute");

    //     if (currentMusicMute == 0)
    //     {
    //         // IS NOT MUTED
    //         musicSource.mute = false;
    //         musicMute.sprite = musicUnmutedSprite;
    //     }
    //     else if (currentMusicMute == 1)
    //     {
    //         // IS MUTED
    //         musicSource.mute = true;
    //         musicMute.sprite = musicMutedSprite;
    //     }
    // }

}
