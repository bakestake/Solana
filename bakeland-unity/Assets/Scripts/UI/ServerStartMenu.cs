using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Fusion.Sockets;
using Fusion.Addons.Hathora;
using Bakeland;
using Fusion.Photon.Realtime;
using Fusion.Sample.DedicatedServer.Utils;
using HathoraCloud.Models.Shared;

public class ServerStartMenu : MonoBehaviour
{
    public static ServerStartMenu Instance { get; private set; }

    [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
    [SerializeField] private Mult_playerData _playerDataPrefab = null;
    [SerializeField] private HathoraManager _manager;

    [SerializeField] private TMP_InputField _nickName = null;

    // The Placeholder Text is not accessible through the TMP_InputField component so need a direct reference
    [SerializeField] private TextMeshProUGUI _nickNamePlaceholder = null;
    [SerializeField] private TextMeshProUGUI RoomCode;

    [SerializeField] private string _gameSceneName = null;

    [Header("Session List")]
    public Button RefreshButton;
    public Transform SessionListContent;
    public GameObject SessionListEntryPrefab;

    private NetworkRunner _runnerInstance = null;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void SetPlayerData()
    {
        var playerData = FindObjectOfType<Mult_playerData>();
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

    public async void RefreshSessionListUI()
    {
        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        var result = await _runnerInstance.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
            Debug.Log("All good");
        }
        else
        {
            Debug.Log($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public void FillSessionList()
    {
        foreach (Transform child in SessionListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo session in PlayerInputBehaviour.Instance.GetSessionInfo())
        {
            if (session.IsVisible)
            {
                GameObject entry = GameObject.Instantiate(SessionListEntryPrefab, SessionListContent);
                SessionEntryPrefab script = entry.GetComponent<SessionEntryPrefab>();
                script.RoomName.text = session.Name;
                script.PlayerCount.text = session.PlayerCount + "/" + session.MaxPlayers;

                if (session.IsOpen == false || session.PlayerCount >= session.MaxPlayers)
                {
                    script.JoinButton.interactable = false;
                }
                else
                {
                    script.JoinButton.interactable = true;
                }
            }
        }
    }

    #region Multiplayer-Game_Scene

    public async void StartClient()
    {
        SetPlayerData();
        Debug.Log(RoomCode.text);
        HathoraClient hathoraClient = _manager.GetOrCreateClientInstance();
        await hathoraClient.Initialize(_manager.RunnerPrefab, "in", RoomCode.text);// Passing null will help select automatically
        if (hathoraClient.HasValidSession == true)
        {
            Debug.Log("Session is valid");
        }
        else
        {
            Debug.Log("The Connection failed");
        }
        ConnectToClient(GameMode.Client, RoomCode.text, "in");
    }

    public async void ConnectToClient(GameMode mode, string roomName, string region)
    {
        LoadingWheel.instance.EnableLoading();
        // Assuming the server created the game with a specific scene to load
        const int multiplayerSceneIndex = 2; // Index of the scene to load
        SceneRef sceneRef = SceneRef.FromIndex(multiplayerSceneIndex);

        _runnerInstance = FindObjectOfType<NetworkRunner>();
        //if (_runnerInstance == null)
        //{
        //    _runnerInstance = Instantiate(_networkRunnerPrefab);
        //}

        // Get a copy of the global Photon App settings
        FusionAppSettings photonAppSettings = PhotonAppSettings.Global.AppSettings.GetCopy();

        // Set the region to the one the server is running on
        photonAppSettings.FixedRegion = region;

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

        LoadingWheel.instance.DisableLoading();
    }

    // Attempts to start a new game session 
    public void StartHost()
    {
        SetPlayerData();
        CreateSession();
    }

    private async void CreateSession()
    {
        LoadingWheel.instance.EnableLoading();
        int randomInt = UnityEngine.Random.Range(1000, 9999);
        string RandomSessionName = "Room-" + randomInt.ToString();

        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        // Let the Fusion Runner know that we will be providing user input
        _runnerInstance.ProvideInput = true;

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = RandomSessionName,
            PlayerCount = 10,
            IsVisible = true,
        };
        var result = await _runnerInstance.StartGame(startGameArgs);

        if (result.Ok)
        {
            if (_runnerInstance.IsServer)
            {
                await _runnerInstance.LoadScene(_gameSceneName);
                LoadingWheel.instance.DisableLoading();
            }
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        LoadingWheel.instance.DisableLoading();
    }

    #endregion

    #region PvP_Scene


    public async void ConnectToPvPServer()
    {
        LoadingWheel.instance.EnableLoading();

        SetPlayerData();
        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        // Get a copy of the global Photon App settings
        FusionAppSettings photonAppSettings = PhotonAppSettings.Global.AppSettings.GetCopy();

        // Set the region to the one the server is running on
        photonAppSettings.FixedRegion = "asia";

        // Let the Fusion Runner know that we will be providing user input
        _runnerInstance.ProvideInput = true;

        var startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = RoomCode.text,
            PlayerCount = 2,
            CustomPhotonAppSettings = photonAppSettings,
            IsVisible = true,
           
        };
        var result = await _runnerInstance.StartGame(startGameArgs);

        if (result.Ok)
        {
            if (_runnerInstance.IsServer)
            {
                await _runnerInstance.LoadScene(_gameSceneName);
                LoadingWheel.instance.DisableLoading();
            }
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
        LoadingWheel.instance.DisableLoading();
    }

    public async void ConnectToPvPClient()
    {
        LoadingWheel.instance.EnableLoading();
        SetPlayerData();

        _runnerInstance = FindObjectOfType<NetworkRunner>();
        if (_runnerInstance == null)
        {
            _runnerInstance = Instantiate(_networkRunnerPrefab);
        }

        // Get a copy of the global Photon App settings
        FusionAppSettings photonAppSettings = PhotonAppSettings.Global.AppSettings.GetCopy();

        // Set the region to the one the server is running on
        photonAppSettings.FixedRegion = "asia";


        // Let the Fusion Runner know that we will be providing user input
        _runnerInstance.ProvideInput = true;

        StartGameResult startGameArgs = await _runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = RoomCode.text,
            CustomPhotonAppSettings = photonAppSettings
        });

        if (startGameArgs.Ok)
        {
            Debug.Log("Game started successfully! Scene is now loading.");
        }
        else
        {
            Debug.LogError("Failed to start the game or load the scene.");
        }
        LoadingWheel.instance.DisableLoading();
    }

    #endregion
}
