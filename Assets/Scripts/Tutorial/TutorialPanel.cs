using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class TutorialPanel : MonoBehaviour
    {
        [System.Serializable]
        public struct TutorialPanelParams { public Texture2D texture; [TextArea(3, 10)] public string text; }

        [SerializeField] private RawImage image;
        [SerializeField] private TMP_Text text;

        public void Initialize(TutorialPanelParams panelParams)
        {
            this.image.texture = panelParams.texture;
            this.text.text = panelParams.text;
        }
    }
}