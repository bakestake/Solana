using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Threading.Tasks;
using Fusion.Sockets;
using BakelandWalletInteraction;

public class ServerGameStateController : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static ServerGameStateController Instance;

    public UaserWalletInteractions walletInteractions;

    private Dictionary<PlayerRef, PlayerData> _playerData = new Dictionary<PlayerRef, PlayerData>();

    public enum GameState
    {
        WalletConnected,
        Starting,
        SpawnPlayer,
        Running,
        Ending
    }

    //[SerializeField] private float _startDelay = 4.0f;
    //[SerializeField] private float _endDelay = 4.0f;
    //[SerializeField] private float _gameSessionLength = 180.0f;

    //[SerializeField] private TextMeshProUGUI _startEndDisplay = null;
    //[SerializeField] private TextMeshProUGUI _ingameTimerDisplay = null;

    [Networked] private TickTimer _timer { get; set; }
    public GameState _gameState { get; private set; }

    private List<NetworkBehaviourId> _playerDataNetworkedIds = new List<NetworkBehaviourId>();

    private void OnEnable()
    {
        //OnPlayerLeftEvent.RegisterResponse(PlayerDisconnected);
        //OnRunnerShutDownEvent.RegisterResponse(DisconnectedFromSession);
    }

    private void OnDisable()
    {
        //OnPlayerLeftEvent.RegisterResponse(PlayerDisconnected);
        //OnRunnerShutDownEvent.RemoveResponse(DisconnectedFromSession);
    }

    public void SetGameState(GameState state)
    {
        _gameState = state;
    }

    public override void Spawned()
    {
        //Host and Client:
        LoadingWheel.instance.DisableLoading();
        // --- This section is for all information which has to be locally initialized based on the networked game state
        // --- when a CLIENT joins a game

        //_startEndDisplay.gameObject.SetActive(true);
        //_ingameTimerDisplay.gameObject.SetActive(false);

        // Set is Simulated so that FixedUpdateNetwork runs on every client instead of just the Host
        Runner.SetIsSimulated(Object, true);

        // --- This section is for all networked information that has to be initialized by the HOST
        if (Object.HasStateAuthority == false) return;

        // Initialize the game state on the host
        _gameState = GameState.WalletConnected;
    }


    public override void FixedUpdateNetwork()
    {
        // Update the game display with the information relevant to the current game state
        switch (_gameState)
        {
            case GameState.WalletConnected:
                CheckForWalletConnection();
                break;
            case GameState.Starting:
                TrackAllPlayers();
                break;
            case GameState.SpawnPlayer:
                UpdateStartingDisplay();
                break;
            case GameState.Running:
                //UpdateRunningDisplay();
                // Ends the game if the game session length has been exceeded
                //if (_timer.ExpiredOrNotRunning(Runner))
                //{
                    //GameHasEnded();
                //}

                break;
            case GameState.Ending:
                UpdateEndingDisplay();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckForWalletConnection()
    {
        // --- Host
        if (Object.HasStateAuthority == false) return;

        if (string.IsNullOrEmpty(walletInteractions.GetConnectedAddress())) return;

        _gameState = GameState.Starting;

    }

    private void TrackAllPlayers()
    {
        // If the game has already started, find all currently active players' PlayerDataNetworked component Ids
        if (_gameState != GameState.SpawnPlayer)
        {
            foreach (var player in Runner.ActivePlayers)
            {
                if (Runner.TryGetPlayerObject(player, out var playerObject) == false) continue;
                TrackNewPlayer(playerObject.GetComponent<Mult_PlayerDataNetworked>().Id);
            }
        }

        // --- This section is for all networked information that has to be initialized by the HOST
        if (Object.HasStateAuthority == false) return;

        // Initialize the game state on the host
        _gameState = GameState.SpawnPlayer;
        //_timer = TickTimer.CreateFromSeconds(Runner, _startDelay);
    }

    private void UpdateStartingDisplay()
    {
        // --- Host & Client
        // Display the remaining time until the game starts in seconds (rounded down to the closest full second)

        //_startEndDisplay.text = $"Game Starts In {Mathf.RoundToInt(_timer.RemainingTime(Runner) ?? 0)}";

        // --- Host
        if (Object.HasStateAuthority == false) return;
        //if (_timer.ExpiredOrNotRunning(Runner) == false) return;

        // Switches to the Running GameState and sets the time to the length of a game session
        _gameState = GameState.Running;

        // Starts the Spaceship and Asteroids spawners once the game start delay has expired
        FindObjectOfType<ServerPlayerSpawner>().StartplayerSpawner(this);
        //FindObjectOfType<AsteroidSpawner>().StartAsteroidSpawner();

        //_timer = TickTimer.CreateFromSeconds(Runner, _gameSessionLength);
    }

    /*private void UpdateRunningDisplay()
    {
        // --- Host & Client
        // Display the remaining time until the game ends in seconds (rounded down to the closest full second)
        _startEndDisplay.gameObject.SetActive(false);
        _ingameTimerDisplay.gameObject.SetActive(true);
        _ingameTimerDisplay.text =
            $"{Mathf.RoundToInt(_timer.RemainingTime(Runner) ?? 0).ToString("000")} seconds left";
    }*/

    private void UpdateEndingDisplay()
    {
        /* --- Host & Client
        // Display the results and
        // the remaining time until the current game session is shutdown

        if (Runner.TryFindBehaviour(_winner, out Mult_PlayerDataNetworked playerData) == false) return;

        _startEndDisplay.gameObject.SetActive(true);
        _ingameTimerDisplay.gameObject.SetActive(false);
        _startEndDisplay.text =
            $"{playerData.NickName} won with {playerData.Score} points. Disconnecting in {Mathf.RoundToInt(_timer.RemainingTime(Runner) ?? 0)}";
        _startEndDisplay.color = SpaceshipVisualController.GetColor(playerData.Object.InputAuthority.PlayerId);*/

        // --- Host
        // Shutdowns the current game session.
        // The disconnection behaviour is found in the OnServerDisconnect.cs script
        //if (_timer.ExpiredOrNotRunning(Runner) == false) return;

        Runner.Shutdown();
    }

    // Called from the ShipController when it hits an asteroid
    /*public void CheckIfGameHasEnded()
    {
        if (Object.HasStateAuthority == false) return;

        int playersAlive = 0;

        for (int i = 0; i < _playerDataNetworkedIds.Count; i++)
        {
            if (Runner.TryFindBehaviour(_playerDataNetworkedIds[i],
                    out Mult_PlayerDataNetworked playerDataNetworkedComponent) == false)
            {
                _playerDataNetworkedIds.RemoveAt(i);
                i--;
                continue;
            }

            if (playerDataNetworkedComponent.Lives > 0) playersAlive++;
        }

        // If more than 1 player is left alive, the game continues.
        // If only 1 player is left, the game ends immediately.
        if (playersAlive > 1 || (Runner.ActivePlayers.Count() == 1 && playersAlive == 1)) return;

        foreach (var playerDataNetworkedId in _playerDataNetworkedIds)
        {
            if (Runner.TryFindBehaviour(playerDataNetworkedId,
                    out Mult_PlayerDataNetworked playerDataNetworkedComponent) ==
                false) continue;

            if (playerDataNetworkedComponent.Lives > 0 == false) continue;

            _winner = playerDataNetworkedId;
        }

        if (_winner == default) // when playing alone in host mode this marks the own player as winner
        {
            _winner = _playerDataNetworkedIds[0];
        }

        GameHasEnded();
    }*/

    public void GameHasEnded()
    {
        //_timer = TickTimer.CreateFromSeconds(Runner, _endDelay);
        _gameState = GameState.Ending;
    }

    public void TrackNewPlayer(NetworkBehaviourId playerDataNetworkedId)
    {
        _playerDataNetworkedIds.Add(playerDataNetworkedId);
    }

    private async Task ShutdownRunner()
    {
        await OnServerDisconnected.LocalRunner?.Shutdown();
        //SetGameState(GameState.Lobby);
        _playerData.Clear();
    }

    public void FightPvP()
    {
        _ = ShutdownRunner();
    }

    #region INetworkRunnerCallbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }
    #endregion
}
