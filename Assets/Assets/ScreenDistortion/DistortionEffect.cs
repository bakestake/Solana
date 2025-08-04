
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistortionEffect : MonoBehaviour
{
    public RawImage distortionImage;
    public Camera distortionCamera;
    public float duration = 2f;
    public float maxRotation = 720f;
    public float distortionScale = 0.2f;

    private RectTransform imageTransform;

    void Awake()
    {
        if (distortionImage != null)
            imageTransform = distortionImage.rectTransform;

        distortionImage.gameObject.SetActive(false);
        distortionCamera.enabled = false;
    }

    public void PlayEffect()
    {
        StartCoroutine(DistortionRoutine());
    }

    private IEnumerator DistortionRoutine()
    {
        distortionImage.gameObject.SetActive(true);
        distortionCamera.enabled = true;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Roda a imagem
            float angle = Mathf.Lerp(0, maxRotation, t);
            imageTransform.rotation = Quaternion.Euler(0, 0, angle);

            // Oscila a escala
            float scale = 1 + Mathf.Sin(t * Mathf.PI * 4f) * distortionScale;
            imageTransform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        // Limpa
        imageTransform.rotation = Quaternion.identity;
        imageTransform.localScale = Vector3.one;

        distortionImage.gameObject.SetActive(false);
        distortionCamera.enabled = false;
    }
}
