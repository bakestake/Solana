using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private AudioType audioType;

        private void Awake()
        {
            slider.onValueChanged.AddListener((value) => SoundManager.Instance.SetVolume(value, audioType));
        }
    }
}