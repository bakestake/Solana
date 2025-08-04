using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.RadialMenu
{
    [System.Serializable]
    public class RadialMenuButtonInfo
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private string labelText;
        [SerializeField] public UnityEvent callback;

        public Sprite Icon => icon;
        public string LabelText => labelText;
    }
}
