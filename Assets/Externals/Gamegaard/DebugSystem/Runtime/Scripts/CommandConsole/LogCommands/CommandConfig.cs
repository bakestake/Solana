using UnityEngine;
using Gamegaard.Singleton;

namespace Gamegaard.RuntimeDebug
{
    [CreateAssetMenu(fileName = "CommandConfig", menuName = "Commands/CommandConfig")]
    public class CommandConfig : ScriptableObjectSingleton<CommandConfig>
    {
        [Header("Format")]
        [SerializeField] private string commandPrefix = "!";
        [SerializeField] private string commandSpace = "-";

        [SerializeField] private string result = "!Command-Teste.Basic";

        [Header("Sugestion Text Colors")]
        [SerializeField] private Color sugestionTextColor = new Color(1, 1, 1, 0.5f);
        [SerializeField] private Color paramTextColor = new Color(1, 1, 1, 0.15f);

        [Header("Chat Text Colors")]
        [SerializeField] private Color callbackTextColor = Color.gray;
        [SerializeField] private Color logTextColor = Color.white;
        [SerializeField] private Color warningTextColor = Color.yellow;
        [SerializeField] private Color errorTextColor = Color.red;

        public string CommandPrefix => commandPrefix;
        public string CommandSpace => commandSpace;
        public Color SugestionTextColor => sugestionTextColor;
        public Color ParamTextColor => paramTextColor;
        public Color CallbackTextColor => callbackTextColor;
        public Color LogTextColor => logTextColor;
        public Color WarningTextColor => warningTextColor;
        public Color ErrorTextColor => errorTextColor;

        public bool HasCommandPrefix => commandPrefix != "";
        public bool HasCommandSpace => commandSpace != "";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FindCommandscriAsset()
        {
            CommandConfig instance = Resources.Load("CommandConfig") as CommandConfig;
            _instance = instance;
        }
    }
}