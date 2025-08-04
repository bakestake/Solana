using Fusion;
using Fusion.Sockets;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Fusion.Menu;
using Fusion.Photon.Realtime;
using Bakeland;
using Fusion.Addons.Hathora;

public class Multiplayer_StartMenu : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
    [SerializeField] private Mult_playerData _playerDataPrefab = null;
    [SerializeField] private HathoraManager _manager;

    [SerializeField] private TMP_InputField _nickName = null;

    //// The Placeholder Text is not accessible through the TMP_InputField component so need a direct reference
    [SerializeField] private TextMeshProUGUI _nickNamePlaceholder = null;

    [SerializeField] private TMP_InputField _roomName = null;

    [SerializeField] private CharacterSelector selector;

    private Mult_playerData playerData;
    private IPhotonMenuConnectArgs connectionArgs { get; set; }
    private bool _connected;
    private string _sessionRegion;

    private NetworkRunner _runnerInstance = null;

    private void Start()
    {
        _nickName.onValueChanged.AddListener(value => CheckForSpecialcharacters(value, _nickName));
    }

    public void SelectCharacter()
    {
        playerData = FindObjectOfType<Mult_playerData>();
        if (playerData == null)
        {
            playerData = Instantiate(_playerDataPrefab);
        }
        int selectedIndex = selector.GetCurrentIndex();
        playerData.SetCharacterSelectorIndex(selectedIndex);
    }

    public void StartHost()
    {
        SetPlayerData();
        StartGame(GameMode.AutoHostOrClient, _roomName.text,"in");
    }

    public void StartClient()
    {
        SetPlayerData();
        //StartGame(GameMode.Client, _roomName.text, _gameSceneName);
        //ToDo: Put an if statement checking dedicated server or host Server
        //if Host Server call function StartGame()
        //else StartingClient();
        StartingClient();
    }

    private async void StartingClient()
    {
        HathoraClient hathoraClient = _manager.GetOrCreateClientInstance();
        await hathoraClient.Initialize(_manager.RunnerPrefab, "in", _roomName.text);// Passing null will help select automatically
        if (hathoraClient.HasValidSession == true)
        {
            Debug.Log("Session is valid");
            StartGame(GameMode.Client, _roomName.text,"in");
        }
        else
        {
            Debug.Log("The Connection failed");
        }
    }

    private void SetPlayerData()
    {
        playerData = FindObjectOfType<Mult_playerData>();
        if (playerData == null)
        {
            playerData = Instantiate(_playerDataPrefab);
        }

        if (string.IsNullOrWhiteSpace(_nickName.text))
        {
            playerData.SetNickName(_nickNamePlaceholder.text);
        }
        else
        {
            playerData.SetNickName(_nickName.text);
        }
    }

    private async void StartGame(GameMode mode, string roomName, string Region = null, string sceneName = null, NetAddress? serverAddress = null)
    {
        LoadingWheel.instance.EnableLoading();

        // Assuming the server created the game with a specific scene to load
        const int multiplayerSceneIndex = 2; // Index of the scene to load
        SceneRef sceneRef = SceneRef.FromIndex(multiplayerSceneIndex);

        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        // Get a copy of the global Photon App settings
        FusionAppSettings photonAppSettings = PhotonAppSettings.Global.AppSettings.GetCopy();

        // Set the region to the one the server is running on
        photonAppSettings.FixedRegion = Region;

        StartGameResult startGameResult = await _runnerInstance.StartGame(new StartGameArgs
        {
            SessionName = roomName,
            Scene = sceneRef, // Ensure this is the correct scene that matches the server
            GameMode = mode,
            CustomPhotonAppSettings = photonAppSettings
        });

        if (startGameResult.Ok)
        {
            Debug.Log("Game started successfully! Scene is now loading.");
        }
        else
        {
            Debug.LogError("Failed to start the game or load the scene.");
        }
    }
    public async void RefreshSessionListUI()
    {
        FusionAppSettings appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
        appSettings.FixedRegion = "in";

        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        var result = await _runnerInstance.JoinSessionLobby(SessionLobby.ClientServer, customAppSettings: appSettings);

        if (result.Ok)
        {
            Debug.Log("All good");
        }
        else
        {
            Debug.Log($"Failed to Start: {result.ShutdownReason}");
        }
        FillSessionList();
    }

    public void FillSessionList()
    {
        foreach (SessionInfo session in PlayerInputBehaviour.Instance.GetSessionInfo())
        {
            if (session.IsVisible)
            {
                Debug.Log("The Session's name is:" + session.Name);
                Debug.Log("The numeber of players in that session:" + session.PlayerCount);
            }
        }
    }

    private void CheckForSpecialcharacters(string value, TMP_InputField textfield)
    {
        string newValue = Regex.Replace(value, @"[^0-9a-zA-Z]", string.Empty);
        if (value != newValue)
        {
            Debug.Log("Please do not use special characters in room name or player name.");
            textfield.text = newValue;
        }
    }

}
