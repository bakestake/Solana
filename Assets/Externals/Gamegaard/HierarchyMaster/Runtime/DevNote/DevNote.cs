using Gamegaard.HierarchyMaster.Attributes;
using UnityEngine;

namespace Gamegaard.HierarchyMaster
{
    [HierarchyIcon]
    public class DevNote : EditorOnlyBehaviour
    {
        [SerializeField] private bool useTitle;
        [SerializeField] private bool useBoxColor;
        [SerializeField] private bool useFontColor;
        [SerializeField] private string title;
        [TextArea(4, 25)]
        [SerializeField] private string message;
        [SerializeField] private float fontSize = 12f;
        [SerializeField] private Color boxColor = Color.yellow;
        [SerializeField] private Color fontColor = Color.black;

        public bool UseTitle => useTitle;
        public bool UseBoxColor => useBoxColor;
        public bool UseFontColor => useFontColor;
        public string Title => title;
        public string Message => message;
        public float FontSize => fontSize;
        public Color BoxColor => boxColor;
        public Color FontColor => fontColor;
    }
}