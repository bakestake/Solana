using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class ZoomSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private new CinemachineVirtualCamera camera;

        private readonly Dictionary<int, float> sliderValueToOrthoSize = new()
        {
            { 0, 11f },
            { 1, 7f },
            { 2, 4f }
        };

        private void Start()
        {
            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnSliderChanged(float value)
        {
            camera.m_Lens.OrthographicSize = sliderValueToOrthoSize.GetValueOrDefault((int)value, 11f);
        }
    }
}