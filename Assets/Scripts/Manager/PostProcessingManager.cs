using System.Collections;
using UnityEngine;

namespace Bakeland
{
    public class PostProcessingManager : MonoBehaviour
    {
        public static PostProcessingManager Instance { get; private set; }

        [SerializeField] private Camera postProcessingCamera;
        [SerializeField] private GameObject postProcessingRenderTexture;

        [SerializeField] private Material intoxicatedMaterial;
        private Coroutine intoxicatedCoroutine;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);

            postProcessingCamera.gameObject.SetActive(false);
            postProcessingRenderTexture.SetActive(false);
            intoxicatedMaterial.SetFloat("_EffectWeight", 0f);
        }

        [ContextMenu("ShowIntoxicated")]
        public void ShowIntoxicated()
        {
            postProcessingCamera.gameObject.SetActive(true);
            postProcessingRenderTexture.SetActive(true);

            if (intoxicatedCoroutine != null) StopCoroutine(IntoxicatedCoroutine(true));
            intoxicatedCoroutine = StartCoroutine(IntoxicatedCoroutine(true));
        }

        [ContextMenu("HideIntoxicated")]
        public void HideIntoxicated()
        {
            if (intoxicatedCoroutine != null) StopCoroutine(IntoxicatedCoroutine(false));
            intoxicatedCoroutine = StartCoroutine(IntoxicatedCoroutine(false));
        }

        private IEnumerator IntoxicatedCoroutine(bool active, float duration = 5f)
        {
            if (duration == 0f)
            {
                Debug.LogError("DIVISION BY ZERO");
                yield break;
            }

            float materialWeight = intoxicatedMaterial.GetFloat("_EffectWeight");
            float targetWeight = active ? 1f : 0f;
            while (materialWeight != targetWeight)
            {
                materialWeight += (active ? Time.deltaTime : -Time.deltaTime) / duration;
                materialWeight = Mathf.Clamp(materialWeight, 0f, 1f);
                intoxicatedMaterial.SetFloat("_EffectWeight", materialWeight);
                yield return null;
            }

            if (active == false)
            {
                postProcessingCamera.gameObject.SetActive(false);
                postProcessingRenderTexture.SetActive(false);
            }
        }
    }
}
