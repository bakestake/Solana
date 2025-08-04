using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
public class PVP_PlayerSpawner : NetworkBehaviour, IPlayerLeft, IPlayerJoined
{
    // References to the NetworkObject prefab to be used for the players' players.
    [SerializeField] private NetworkPrefabRef _policemanNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private NetworkPrefabRef _stonerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private NetworkPrefabRef _farmerNetworkPrefab = NetworkPrefabRef.Empty;
    private NetworkPrefabRef _prefabCharacter { get; set; } = NetworkPrefabRef.Empty;
    private Mult_playerData data;

    private bool _gameIsReady = false;
    private PVP_GameStateController _gameStateController = null;

    public List<Transform> _spawnPoints;
    private Dictionary<Transform, bool> spawningPoints = new Dictionary<Transform, bool>();

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;
        // Collect all spawn points in the scene.
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            spawningPoints.Add(_spawnPoints[i], false);
        }

        _prefabCharacter = _farmerNetworkPrefab;
    }

    // The spawner is started when the GameStateController switches to GameState.Running.
    public void StartplayerSpawner(PVP_GameStateController gameStateController)
    {
        _gameIsReady = true;
        _gameStateController = gameStateController;
        foreach (var player in Runner.ActivePlayers)
        {
            Spawnplayer(player);
        }
    }

    // Spawns a new player if a client joined after the game already started
    public void PlayerJoined(PlayerRef player)
    {
        if (_gameIsReady == false) return;
        Spawnplayer(player);
    }

    // Spawns a player for a player.
    // The spawn point is chosen in the _spawnPoints array using the implicit playerRef to int conversion 
    private void Spawnplayer(PlayerRef player)
    {
        var spawnPosition = Vector3.zero;
        foreach (var spawnpoint in spawningPoints)
        {
            if (spawnpoint.Value == false)
            {
                spawnPosition = spawnpoint.Key.position;
                spawningPoints[spawnpoint.Key] = true;
                break;
            }
        }

        var playerObject = Runner.Spawn(_prefabCharacter, spawnPosition, Quaternion.identity, player);
        // Set Player Object to facilitate access across systems.
        Runner.SetPlayerObject(player, playerObject);

        // Add the new player to the players to be tracked for the game end check.
        _gameStateController.TrackNewPlayer(playerObject.GetComponent<Health_Playerr>().Id);
    }

    // Despawns the player associated with a player when their client leaves the game session.
    public void PlayerLeft(PlayerRef player)
    {
        Despawnplayer(player);
    }

    private void Despawnplayer(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject))
        {
            Runner.Despawn(playerNetworkObject);
        }

        // Reset Player Object
        Runner.SetPlayerObject(player, null);
    }

    // RPC used to send player information to the Host
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RpcSetPrefab(int CharacterNumber)
    {
        if (CharacterNumber == 0)
        {
            _prefabCharacter = _policemanNetworkPrefab;
        }
        else if (CharacterNumber == 1)
        {
            _prefabCharacter = _stonerNetworkPrefab;
        }
        else
        {
            _prefabCharacter = _farmerNetworkPrefab;
        }

    }
}
