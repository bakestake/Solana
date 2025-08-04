using Gamegaard.Singleton;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamegaard.RuntimeDebug
{
    public class CommandConsole : MonoBehaviourSingleton<CommandConsole>
    {
        [Header("Commands")]
        [SerializeField] private UnityEventLogCommand[] eventCommands;

        [Header("References")]
        [SerializeField] private CommandFieldSuggestor inputField;
        [SerializeField] private TextChat textChat;

        private CommandCenter commandCenter;
        private bool isDebugLogEnabled = true;

        protected override void Start()
        {
            base.Start();
            commandCenter = CommandCenter.NotNullInstance;
            InitializeDefaultCommands();
            AddEventCommands();
            inputField.SetAutoCompleteOptions(commandCenter.commandByID.Values);

            commandCenter.OnCommandRegistered += OnCommandRegistered;
            commandCenter.OnCommandRemoved += OnCommandRemoved;
            commandCenter.OnCommandsCleared += OnCommandCleared;
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (CommandCenter.IsDestroyed || CommandCenter.IsQuitting) return;
            commandCenter.OnCommandRegistered -= OnCommandRegistered;
            commandCenter.OnCommandRemoved -= OnCommandRemoved;
            commandCenter.OnCommandsCleared -= OnCommandCleared;
        }

        private void OnCommandRegistered(LogCommandBase command)
        {
            inputField.AddAutoCompleteOption(command);
        }

        private void OnCommandRemoved(LogCommandBase command)
        {
            inputField.RemoveAutoCompleteOption(command);
        }

        private void OnCommandCleared()
        {
            inputField.ClearAutoCompleteOptions();
        }

        // used by commands. Dont save message on history.
        public void SendTextMessage(string text)
        {
            textChat.CreateTextMessage(text.ToString(), false, Color.white);
        }

        public void SendInputMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            textChat.SendInputMessage(message, true, "> ");

            foreach (LogCommandBase command in commandCenter.commandByID.Values)
            {
                if (command.Validate(message))
                {
                    command.TryInvoke(message);
                    if (command.HasCallbackText)
                    {
                        textChat.CreateTextMessage(command.CommandCallbackText, false, CommandConfig.Instance.CallbackTextColor);
                    }
                    break;
                }
            }
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!isDebugLogEnabled) return;
            Color color = GetLogColor(type);

            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    logString = $"Error: {logString}";
                    break;
                case LogType.Warning:
                    logString = $"Warning: {logString}";
                    break;
                case LogType.Log:
                    logString = $"Debug: {logString}";
                    break;
            }

            textChat.CreateTextMessage(logString, false, color);
        }

        private Color GetLogColor(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    return CommandConfig.Instance.ErrorTextColor;
                case LogType.Warning:
                    return CommandConfig.Instance.WarningTextColor;
                case LogType.Log:
                    return CommandConfig.Instance.LogTextColor;
                default:
                    return Color.white;
            }
        }

        #region Commands
        private void AddEventCommands()
        {
            foreach (LogCommandBase command in eventCommands)
            {
                commandCenter.TryRegister(command);
            }
        }

        private void InitializeDefaultCommands()
        {
            commandCenter.TryRegister(new ActionLogCommand("Help", "!Help", DebugHelp, "Show all possible commands"));
            commandCenter.TryRegister(new ActionLogCommand("DebugCatch", "!DebugCatch", EnbaleDebugCatch, "Start catch game debugs"));
            commandCenter.TryRegister(new ActionLogCommand("Clear", "!Clear", textChat.Clear, "Clear chat text"));
            commandCenter.TryRegister(new ActionLogCommand("ActiveScenes", "!ActiveScenes", GetActiveScenes, "Show all active scenes"));
            commandCenter.TryRegister(new ActionLogCommand("ActiveScene", "!ActiveScene", GetActiveScene, "Show the current active scene"));
            commandCenter.TryRegister(new ActionLogCommand("CommandCount", "!CommandCount", GetCommandCount, "Show the current active scene"));
            commandCenter.TryRegister(new ActionLogCommand("GetScreenDPI", "!GetScreenDPI", GetScreenDPI, "..."));
            commandCenter.TryRegister(new ActionLogCommand("GetResolution", "!GetResolution", GetResolution, "..."));
            commandCenter.TryRegister(new ActionLogCommand("GetAspectRatio", "!GetAspectRatio", GetAspectRatio, "..."));
        }

        private void DebugHelp()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine("List of all commands:");
            foreach (LogCommandBase item in commandCenter.commandByID.Values)
            {
                text.Append("- ");
                text.AppendLine(item.FormatedHelpText);
            }

            SendTextMessage(text.ToString());
        }

        private void GetActiveScenes()
        {
            StringBuilder text = new StringBuilder("List of active scenes:\n");

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                text.AppendLine($"[{scene.buildIndex}] {scene.name}");
            }

            SendTextMessage(text.ToString());
        }

        private void GetActiveScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            string text = $"[{scene.buildIndex}] {scene.name}";

            SendTextMessage(text);
        }

        private void EnbaleDebugCatch()
        {
            isDebugLogEnabled = !isDebugLogEnabled;
        }

        private void GetCommandCount()
        {
            SendTextMessage(commandCenter.CommandCount.ToString());
        }

        private void GetScreenDPI()
        {
            float screenDPI = Screen.dpi;
            if (screenDPI == 0)
            {
                screenDPI = 96;
            }
            SendTextMessage($"Screen DPI: {screenDPI}");
        }

        private void GetResolution()
        {
            Resolution currentResolution = Screen.currentResolution;
            SendTextMessage($"Resolution: {currentResolution.width} x {currentResolution.height}");
        }

        private void GetAspectRatio()
        {
            int width = Screen.width;
            int height = Screen.height;
            int gcd = GreatestCommonDivisor(width, height);
            int simplifiedWidth = width / gcd;
            int simplifiedHeight = height / gcd;

            SendTextMessage($"Aspect Ratio: {simplifiedWidth}:{simplifiedHeight}");
            static int GreatestCommonDivisor(int a, int b)
            {
                while (b != 0)
                {
                    int t = b;
                    b = a % b;
                    a = t;
                }
                return a;
            }
        }
        #endregion
    }
}