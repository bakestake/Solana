using UnityEngine;

namespace Gamegaard
{
    public class PauseWhileEnabled : MonoBehaviour
    {
        private IPauseStrategy pauseStrategy;

        private void Awake()
        {
            pauseStrategy ??= new PauseSystemStrategy(false);
        }

        private void OnEnable()
        {
            pauseStrategy.Pause(gameObject);
        }

        private void OnDisable()
        {
            pauseStrategy.Unpause(gameObject);
        }

        public void SetStrategy(IPauseStrategy pauseStrategy)
        {
            this.pauseStrategy = pauseStrategy;
        }
    }
}
