using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Audio_Manager : MonoBehaviour
{
    public static Audio_Manager instance;
    [Header("Outputs")]
    public AudioSource masterOutput;//muzik
    public AudioSource sfxOutput;//ses efektleri
    public float maxOutputVolume = 1f;

    public AudioClip shotgun, bong_hit, smoke, laser, dodge, ultiDmg, comboDmg, click;
    [Header("Damage Sounds")]
    public AudioClip hurt;

    [Header("Frame Sounds")]
    AudioClip currentClip;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartVolumeEffect();
    }
    public void StartVolumeEffect()
    {
        //sahne acildiginda muzigin sesini smooth sekilde acar
        masterOutput.volume = 0;
        ChangeVolumeMaster(maxOutputVolume, 5f);
    }
    public void ChangeVolumeMaster(float targetValue, float duration)
    {
        //muzigin sesini artirip azalt
        masterOutput.DOFade(targetValue, duration);
    }
    public void ChangeVolumeSFX(float targetValue, float duration)
    {
        //ses efektlerinin sesini artirip azalt
        sfxOutput.DOFade(targetValue, duration);
    }
    public void PlaySoundEffect(AudioClip audioClip, float volume)
    {
        //ses efekti cal
        sfxOutput.PlayOneShot(audioClip, volume);
    }
    public void PlaySound(AudioClip audioClip)
    {
        //ses efekti cal
        sfxOutput.PlayOneShot(audioClip, 1f);
    }
}
