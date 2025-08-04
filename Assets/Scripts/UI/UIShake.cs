using UnityEngine;
using System.Collections;

public class UIShake : MonoBehaviour
{
    public float duration, intensity;

    [SerializeField] private RectTransform targetUI;

    private Vector2 originalPos;
    private Coroutine coroutine;

    private void Awake()
    {
        originalPos = targetUI.anchoredPosition;
    }

    public void Shake()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    public void Shake(float duration, float intensity)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float time = 0f;

        float perlinX = Random.Range(0f, 100f);
        float perlinY = Random.Range(0f, 100f);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            float noiseX = (Mathf.PerlinNoise(perlinX, Time.time * 20f) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(perlinY, Time.time * 20f) - 0.5f) * 2f;

            Vector2 offset = (1f - t) * intensity * new Vector2(noiseX, noiseY);
            targetUI.anchoredPosition = originalPos + offset;

            yield return null;
        }

        targetUI.anchoredPosition = originalPos;
    }
}
