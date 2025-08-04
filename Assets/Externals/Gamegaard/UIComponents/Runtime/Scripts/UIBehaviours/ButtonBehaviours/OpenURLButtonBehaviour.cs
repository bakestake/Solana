using UnityEngine;
using System.Collections;

namespace Gamegaard.Commons
{
    public class OpenURLButtonBehaviour : ButtonBehaviour
    {
        [SerializeField] private string url;

        public override void OnClick()
        {
            StartCoroutine(OpenURLCoroutine());
        }

        private IEnumerator OpenURLCoroutine()
        {
            yield return new WaitForEndOfFrame();
            Application.OpenURL(url);
        }
    }
}