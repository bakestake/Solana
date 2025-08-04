using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public class PlayerSceneStart : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private SceneLoader loader;
        [SerializeField] private CanvasGroup fadeCanvas;

        private IEnumerator Start()
        {
            yield return null;
            fadeCanvas.alpha = 1.0f;
            var lastWaitBeforeFade = loader.waitBeforeFade;
            loader.waitBeforeFade = 0f;
            loader.LoadSceneInterior(sceneName);

            yield return new WaitForSeconds(1f);
            loader.waitBeforeFade = lastWaitBeforeFade;
            while (fadeCanvas.alpha > 0f)
            {
                fadeCanvas.alpha -= Time.deltaTime / .75f;
                yield return null;
            }

            Destroy(fadeCanvas.gameObject);
            this.enabled = false;
        }
    }
}