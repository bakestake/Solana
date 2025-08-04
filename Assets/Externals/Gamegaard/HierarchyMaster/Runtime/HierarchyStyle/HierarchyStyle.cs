using Gamegaard.HierarchyMaster.Attributes;
using UnityEngine;

namespace Gamegaard.HierarchyMaster
{
    [HierarchyIcon]
    [DisallowMultipleComponent]
    public sealed class HierarchyStyle : EditorOnlyBehaviour
    {
        [SerializeField] private BackgroundStyle backgroundStyle = BackgroundStyle.Default;
        [SerializeField] private Color32 backgroundColor = new Color32(255, 255, 255, 255);
        [SerializeField] private Gradient backgroundGradient = new Gradient();
        [SerializeField] private float gradientAngle;

        [SerializeField] private FontColorStyle fontColorStyle = FontColorStyle.Default;
        [SerializeField] private Color32 textColor = new Color32(255, 255, 255, 255);
        [SerializeField] private Font font;
        [SerializeField] private int fontSize = 12;
        [SerializeField] private FontStyle fontStyle = FontStyle.Normal;
        [SerializeField] private TextAnchor alignment = TextAnchor.UpperLeft;
        [SerializeField] private bool textDropShadow;
        [SerializeField] private bool enableTextOutline;
        [SerializeField] private Color32 textOutlineColor = new Color32(0, 0, 0, 255);

        [SerializeField] private ContainerLines containerLines = ContainerLines.None;
        [SerializeField] private Color32 containerLineColor = new Color32(0, 0, 0, 255);
        [SerializeField] private bool useCustomIcon;
        [SerializeField] private Texture2D customIcon;

        [SerializeField] private bool enableRichText;

        public Gradient BackgroundGradient => backgroundGradient;
        public Texture2D CustomIcon => customIcon;
        public Font Font => font;
        public Color32 BackgroundColor => backgroundColor;
        public Color32 TextColor => textColor;
        public Color32 TextOutlineColor => textOutlineColor;
        public Color32 ContainerLineColor => containerLineColor;
        public BackgroundStyle BackgroundStyle => backgroundStyle;
        public FontColorStyle FontColorStyle => fontColorStyle;
        public ContainerLines ContainerLines => containerLines;
        public FontStyle FontStyle => fontStyle;
        public TextAnchor Alignment => alignment;
        public float GradientAngle => gradientAngle;
        public int FontSize => fontSize;
        public bool UseCustomIcon => useCustomIcon;
        public bool EnableRichText => enableRichText;
        public bool TextDropShadow => textDropShadow;
        public bool EnableTextOutline => enableTextOutline;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
        }

        private void Start()
        {

        }
#endif
    }
}