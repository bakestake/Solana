//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using System.Threading.Tasks;
//using Unity.Services.Core;
//using Unity.Services.Multiplay;
//using UnityEngine.SceneManagement;
//using Unity.Services.Core;
//using Unity.Services.Multiplay;
//using Fusion;
//using Fusion.Sockets;

//public class ServerScript : MonoBehaviour
//{
//    private string ticketId;
//    private NetworkRunner _runnerInstance = null;

//    [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
//    [SerializeField] private string _gameSceneName = null;
//    // Start is called before the first frame update
//    void Start()
//    {
//        DontDestroyOnLoad(gameObject);
//        StartServer();

//    }

//    async void StartServer()
//    {
//        try
//        {
//            await UnityServices.InitializeAsync();
//        }
//        catch (Exception e)
//        {
//            Debug.Log(e);
//        }

//        var server = MultiplayService.Instance.ServerConfig;
//        CreateSessionServer(GameMode.Server, 10, true, server.Port);
//        Debug.Log("Network Transport" + NetAddress.CreateFromIpPort("0.0.0.0", server.Port));

//        var callbacks = new MultiplayEventCallbacks();
//        callbacks.Allocate += OnAllocate;
//        callbacks.Deallocate += OnDeallocate;
//        callbacks.Error += OnError;
//        callbacks.SubscriptionStateChanged += OnSubscriptionStateChanged;

//        if (MultiplayService.Instance == null)
//        {
//            Debug.Log("The MultiplayService is Null");
//        }

//        // We must then subscribe.
//        var events = await MultiplayService.Instance.SubscribeToServerEventsAsync(callbacks);
//    }

//    public async void CreateSessionServer(GameMode gameMode, int playerCount, bool Visibility, ushort port)
//    {
//        LoadingWheel.instance.EnableLoading();
//        int randomInt = UnityEngine.Random.Range(1000, 9999);
//        string RandomSessionName = "Room-" + randomInt.ToString();

//        _runnerInstance = FindObjectOfType<NetworkRunner>();
//        if (_runnerInstance == null)
//        {
//            _runnerInstance = Instantiate(_networkRunnerPrefab);
//        }

//        // Let the Fusion Runner know that we will be providing user input
//        _runnerInstance.ProvideInput = true;

//        var startGameArgs = new StartGameArgs()
//        {
//            GameMode = gameMode,
//            SessionName = RandomSessionName,
//            PlayerCount = playerCount,
//            IsVisible = Visibility,
//            Address = NetAddress.CreateFromIpPort("0.0.0.0", port)
//        };
//        var result = await _runnerInstance.StartGame(startGameArgs);

//        if (result.Ok)
//        {
//            if (_runnerInstance.IsServer)
//            {
//                await _runnerInstance.LoadScene(_gameSceneName);
//            }
//        }
//        else
//        {
//            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
//        }
//    }

//    void OnSubscriptionStateChanged(MultiplayServerSubscriptionState obj)
//    {
//        Debug.Log($"Subscription state changed: {obj}");
//    }

//    void OnError(MultiplayError obj)
//    {
//        Debug.Log($"Error received: {obj}");
//    }

//    async void OnDeallocate(MultiplayDeallocation obj)
//    {
//        Debug.Log($"Deallocation received: {obj}");
//        await MultiplayService.Instance.UnreadyServerAsync();
//    }

//    async void OnAllocate(MultiplayAllocation allocation)
//    {
//        Debug.Log($"Allocation received: {allocation}");
//        await MultiplayService.Instance.ReadyServerForPlayersAsync();
//    }
//}
