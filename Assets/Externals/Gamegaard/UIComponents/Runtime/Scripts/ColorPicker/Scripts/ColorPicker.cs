using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [Header("Behaviours")]
    [SerializeField] private bool isRealTimeColorUpdate;

    [Header("References")]
    [SerializeField] private RawImage hueImage;
    [SerializeField] private RawImage SvImage;
    [SerializeField] private RawImage outputImage;
    [SerializeField] private Slider hueSlider;
    [SerializeField] private TMP_InputField hexInputField;

    private SVImageControl svImageControl;
    private Texture2D hueTexture;
    private Texture2D SvTexture;
    private Texture2D outputTexture;
    private IColorable colorable;
    private bool colorInitialized;

    public Color CurrentColor { get; protected set; }
    public float CurrentHue { get; protected set; }
    public float CurrentSaturation { get; protected set; }
    public float CurrentValue { get; protected set; }

    private void Awake()
    {
        svImageControl = SvImage.GetComponent<SVImageControl>();
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();
        UpdateOutputImage();
        colorInitialized = true;
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16)
        {
            wrapMode = TextureWrapMode.Clamp,
            name = "HueTexture"
        };

        for (int i = 0; i < hueTexture.height; i++)
        {
            Color color = Color.HSVToRGB((float)i / hueTexture.height, 1, 1);
            hueTexture.SetPixel(0, i, color);
        }
        hueTexture.Apply();
        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        SvTexture = new Texture2D(16, 16)
        {
            wrapMode = TextureWrapMode.Clamp,
            name = "SVTexture"
        };

        for (int y = 0; y < SvTexture.height; y++)
        {
            for (int x = 0; x < SvTexture.width; x++)
            {
                Color color = Color.HSVToRGB(CurrentHue, (float)x / SvTexture.width, (float)y / SvTexture.height);
                SvTexture.SetPixel(x, y, color);
            }
        }
        SvTexture.Apply();
        SvImage.texture = SvTexture;
    }

    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(1, 16)
        {
            wrapMode = TextureWrapMode.Clamp,
            name = "OutputTexture"
        };

        Color currentColor = Color.HSVToRGB(CurrentHue, CurrentSaturation, CurrentValue);

        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColor);
        }
        outputTexture.Apply();
        outputImage.texture = outputTexture;
    }

    protected virtual void UpdateOutputImage()
    {
        if (outputTexture == null) return;
        CurrentColor = Color.HSVToRGB(CurrentHue, CurrentSaturation, CurrentValue);
        for (int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, CurrentColor);
        }
        outputTexture.Apply();
        if (!hexInputField.isFocused)
        {
            hexInputField.SetTextWithoutNotify(ConvertToHTMLColor(CurrentColor));
        }
        UpdateColor();
    }

    public void SetSV(float saturation, float value)
    {
        CurrentSaturation = saturation;
        CurrentValue = value;
        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        if (SvTexture == null) return;
        CurrentHue = 1 - hueSlider.value;

        for (int y = 0; y < SvTexture.height; y++)
        {
            for (int x = 0; x < SvTexture.width; x++)
            {
                Color color = Color.HSVToRGB(CurrentHue, (float)x / SvTexture.width, (float)y / SvTexture.height);
                SvTexture.SetPixel(x, y, color);
            }
        }

        SvTexture.Apply();
        UpdateOutputImage();
    }

    public void ShowColorPicker(IColorable colorable)
    {
        gameObject.SetActive(true);
        this.colorable = colorable;
        SincronizeColor();
    }

    public void SincronizeColor()
    {
        if (colorable == null) return;
        SetCurrentColor(colorable.CurrentColor);
    }

    public void SetCurrentColor(Color color)
    {
        if (svImageControl == null) return;
        Color.RGBToHSV(color, out float h, out float s, out float v);
        CurrentHue = h;
        CurrentSaturation = s;
        CurrentValue = v;

        hueSlider.value = 1 - h;
        SetSV(s, v);

        svImageControl.RefreshPickerPosition();
    }

    private void UpdateColor()
    {
        if (!isRealTimeColorUpdate || !colorInitialized) return;
        colorable?.SetColor(CurrentColor);
    }

    public void ApplyColor()
    {
        colorable?.SetColor(CurrentColor);
    }

    public string ConvertToHTMLColor(Color color)
    {
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);

        r = Mathf.Clamp(r, 0, 255);
        g = Mathf.Clamp(g, 0, 255);
        b = Mathf.Clamp(b, 0, 255);

        return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
    }

    public Color ConvertToUnityColor(string htmlColor)
    {
        ColorUtility.TryParseHtmlString(htmlColor, out Color unityColor);
        return unityColor;
    }

    public void OnColorHexChanged(string htmlColor)
    {
        Color color = ConvertToUnityColor("#" + htmlColor);
        Color.RGBToHSV(color, out float hue, out float saturation, out float value);
        hueSlider.value = 1 - hue;
        SetSV(saturation, value);
    }
}