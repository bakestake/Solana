using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard.Commons
{
    [RequireComponent(typeof(Button))]
    public class ExitButtonBehaviour : ButtonBehaviour
    {
        public override void OnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}