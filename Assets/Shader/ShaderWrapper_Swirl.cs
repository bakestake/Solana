using System.Collections;
using UnityEngine;

namespace Bakeland
{
    public class ShaderWrapper : MonoBehaviour
    {
        [SerializeField] private Material material;
        [SerializeField] private string parameterName;
        [SerializeField] private float duration;

        private Coroutine effectCoroutine;

        private void Awake()
        {
            material.SetFloat(parameterName, 0f);
        }

        private void OnDestroy()
        {
            RemoveEffect();
        }

        public void RemoveEffect()
        {
            material.SetFloat(parameterName, 0f);
        }

        [ContextMenu("PlayEffect")]
        public void PlayEffect()
        {
            if (effectCoroutine != null) StopCoroutine(EffectCoroutine(true));
            effectCoroutine = StartCoroutine(EffectCoroutine(true));
        }

        [ContextMenu("HideEffect")]
        public void HideEffect()
        {
            if (effectCoroutine != null) StopCoroutine(EffectCoroutine(false));
            effectCoroutine = StartCoroutine(EffectCoroutine(false));
        }

        private IEnumerator EffectCoroutine(bool active)
        {
            if (duration == 0f)
            {
                Debug.LogError("DIVISION BY ZERO");
                yield break;
            }

            float materialWeight = material.GetFloat(parameterName);
            float targetWeight = active ? 1f : 0f;
            while (materialWeight != targetWeight)
            {
                materialWeight += (active ? Time.deltaTime : -Time.deltaTime) / duration;
                materialWeight = Mathf.Clamp(materialWeight, 0f, 1f);
                material.SetFloat(parameterName, materialWeight);
                yield return null;
            }
        }
    }
}