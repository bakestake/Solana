using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public class SinMovement : MonoBehaviour
    {
        [SerializeField] private float offset = -50;
        [SerializeField] private float amplitude, frequency;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            float sinMovement = Mathf.Sin(Time.time * frequency * Mathf.PI * 2) * amplitude;
            this.rectTransform.anchoredPosition = new Vector3(sinMovement + offset, 0, 0);
        }
    }
}
