using Gamegaard.Singleton;
using UnityEngine;

namespace Game
{
    public class Main : MonoBehaviourSingleton<Main>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InstantiateMain()
        {
            Object main = Resources.Load("Main");
            if (main == null)
            {
                Debug.LogWarning("Main is Missing!");
                return;
            }
            Instantiate(main);
        }
    }
}