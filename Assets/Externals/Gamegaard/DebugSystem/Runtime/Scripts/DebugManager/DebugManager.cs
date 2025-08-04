using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamegaard.RuntimeDebug
{
    public class DebugManager : MonoBehaviour
    {
        [SerializeField] private DebugKeys restartCommand;
        [SerializeField] private EventDebugKeys[] debugMenuCommands;

        private void Update()
        {
            if (restartCommand.CheckInputs())
            {
                int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(sceneBuildIndex);
            }
            foreach (EventDebugKeys key in debugMenuCommands)
            {
                key.CheckInputs();
            }
        }
    }
}