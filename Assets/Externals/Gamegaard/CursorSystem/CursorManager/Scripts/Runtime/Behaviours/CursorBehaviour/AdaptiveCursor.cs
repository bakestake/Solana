using Gamegaard.CursorSystem;
using UnityEngine;

public class AdaptiveCursor : ScreenBrightnessDetector
{
    [SerializeField] private VirtualCursor cursor;

    [Header("Visual Settings")]
    [SerializeField]
    protected Gradient cursorColorGradient = new Gradient()
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.white, 0),
            new GradientColorKey(Color.white, 1)
        },
        alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1)
        }
    };

    protected override void Reset()
    {
        base.Reset();
        cursor = GetComponent<VirtualCursor>();
    }

    protected override void OnBrightnessChanged(float value)
    {
        cursor.SetColor(cursorColorGradient.Evaluate(ScreenBrightness));
    }
}