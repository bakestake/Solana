using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gamegaard.Commons
{
    [RequireComponent(typeof(Button))]
    public class LoadSceneButtonBehaviour : ButtonBehaviour
    {
        [SerializeField] private string sceneName;

        public override void OnClick()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}