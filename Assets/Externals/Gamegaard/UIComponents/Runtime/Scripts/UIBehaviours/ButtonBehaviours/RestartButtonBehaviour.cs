using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gamegaard.Commons
{
    [RequireComponent(typeof(Button))]
    public class RestartButtonBehaviour : ButtonBehaviour
    {
        public override void OnClick()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }
    }
}