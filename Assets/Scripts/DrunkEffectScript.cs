using UnityEngine;

[ExecuteInEditMode]
public class DrunkEffectScript : MonoBehaviour
{
    [SerializeField] private Material drunkMaterial;
    [Range(0, 1)]
    [SerializeField] private float distortionStrength = 0.1f;
    [Range(1, 10)]
    [SerializeField] private float waveFrequency = 3.0f;
    [Range(0, 1)]
    [SerializeField] private float aberrationStrength = 0.1f;
    [Range(0, 10)]
    [SerializeField] private float rotationSpeed = 1.0f;
    [Range(0, 1)]
    [SerializeField] public float effectWeight = 0.0f;

    private bool fadingIn;
    private bool fadingOut;
    private float fadeDuration = 1f;

    private void Update()
    {
        if (fadingIn)
        {
            FadeInEffect(fadeDuration);
        }

        if (fadingOut)
        {
            FadeOutEffect(fadeDuration);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (drunkMaterial != null)
        {
            Debug.Log("drunk material");

            drunkMaterial.SetFloat("_DistortionStrength", distortionStrength);
            drunkMaterial.SetFloat("_WaveFrequency", waveFrequency);
            drunkMaterial.SetFloat("_AberrationStrength", aberrationStrength);
            drunkMaterial.SetFloat("_RotationSpeed", rotationSpeed);
            drunkMaterial.SetFloat("_EffectWeight", effectWeight);

            Graphics.Blit(src, dest, drunkMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public void StartFadeIn(float duration)
    {
        fadeDuration = duration;
        fadingIn = true;
        fadingOut = false;
    }

    public void StartFadeOut(float duration)
    {
        fadeDuration = duration;
        fadingOut = true;
        fadingIn = false;
    }

    public void FadeInEffect(float duration)
    {
        if (duration > 0)
        {
            effectWeight += Time.deltaTime / duration;
            effectWeight = Mathf.Clamp01(effectWeight);

            if (effectWeight >= 1f)
            {
                fadingIn = false;
            }
        }
    }

    public void FadeOutEffect(float duration)
    {
        if (duration > 0)
        {
            effectWeight -= Time.deltaTime / duration;
            effectWeight = Mathf.Clamp01(effectWeight);

            if (effectWeight <= 0f)
            {
                fadingOut = false;
            }
        }
    }
}
