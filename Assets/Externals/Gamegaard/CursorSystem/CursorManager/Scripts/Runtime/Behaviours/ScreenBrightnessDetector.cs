using UnityEngine;

public abstract class ScreenBrightnessDetector : MonoBehaviour
{
    [Header("Check Settings")]
    [SerializeField] protected float brightnessCheckInterval = 0.05f;
    [SerializeField] protected Vector2 brightnessSampleArea = new Vector2(10, 10);

    [Header("References")]
    [SerializeField] private Camera mainCamera;

    private RenderTexture renderTexture;
    private Texture2D reusableTexture;
    private Vector2Int currentResolution;

    public float ScreenBrightness { get; private set; }

    protected virtual void Reset()
    {
        mainCamera = Camera.main;
        EnsureRenderTextureIsValid();
    }

    private void EnsureRenderTextureIsValid()
    {
        Vector2Int screenResolution = new Vector2Int(Screen.width, Screen.height);

        if (renderTexture == null || reusableTexture == null || screenResolution != currentResolution)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }

            renderTexture = new RenderTexture(screenResolution.x, screenResolution.y, 24)
            {
                format = RenderTextureFormat.ARGB32,
                useMipMap = false
            };
            renderTexture.Create();

            if (reusableTexture != null)
            {
                Destroy(reusableTexture);
            }
            reusableTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            currentResolution = screenResolution;
        }
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        if (reusableTexture != null)
        {
            Destroy(reusableTexture);
        }
    }

    private void Update()
    {
        EnsureRenderTextureIsValid();
        CalculateScreenBrightness();
    }

    private void CalculateScreenBrightness()
    {
        mainCamera.targetTexture = renderTexture;
        mainCamera.Render();
        mainCamera.targetTexture = null;

        RenderTexture previousActiveRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        reusableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = previousActiveRT;

        Vector2 mousePosition = transform.position;

        Vector2 renderPos = new Vector2(
            mousePosition.x / Screen.width * renderTexture.width,
            mousePosition.y / Screen.height * renderTexture.height
        );

        int startX = Mathf.Clamp((int)(renderPos.x - brightnessSampleArea.x / 2), 0, renderTexture.width);
        int startY = Mathf.Clamp((int)(renderPos.y - brightnessSampleArea.y / 2), 0, renderTexture.height);
        int width = Mathf.Clamp((int)brightnessSampleArea.x, 0, renderTexture.width - startX);
        int height = Mathf.Clamp((int)brightnessSampleArea.y, 0, renderTexture.height - startY);

        Color[] pixels = reusableTexture.GetPixels(startX, startY, width, height);

        float totalBrightness = 0f;
        for (int i = 0; i < pixels.Length; i++)
        {
            totalBrightness += pixels[i].grayscale;
        }

        ScreenBrightness = totalBrightness / pixels.Length;

        if (float.IsNaN(ScreenBrightness))
        {
            ScreenBrightness = 0;
        }

        OnBrightnessChanged(ScreenBrightness);
    }

    protected abstract void OnBrightnessChanged(float value);
}
