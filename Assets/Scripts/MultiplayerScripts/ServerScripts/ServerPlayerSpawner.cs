using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class ServerPlayerSpawner : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    // References to the NetworkObject prefab to be used for the players' players.
    [SerializeField] private NetworkPrefabRef _playerNetworkPrefab = NetworkPrefabRef.Empty;

    private bool _gameIsReady = false;
    private ServerGameStateController _gameStateController = null;

    public List<Transform> _spawnPoints;

    public override void Spawned()
    {
        if (Object.HasStateAuthority == false) return;
        // Collect all spawn points in the scene.
    }

    // The spawner is started when the GameStateController switches to GameState.Running.
    public void StartplayerSpawner(ServerGameStateController gameStateController)
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
        // Modulo is used in case there are more players than spawn points.
        //int index = player.PlayerId % _spawnPoints.Length;
        var spawnPosition = _spawnPoints[Random.Range(0, (_spawnPoints.Count - 1))].transform.position;//_spawnPoints[index].transform.position;

        var playerObject = Runner.Spawn(_playerNetworkPrefab, spawnPosition, Quaternion.identity, player);

        // Set Player Object to facilitate access across systems.
        Runner.SetPlayerObject(player, playerObject);

        // Add the new player to the players to be tracked for the game end check.
        _gameStateController.TrackNewPlayer(playerObject.GetComponent<Mult_PlayerDataNetworked>().Id);
        //Disable loading
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
}
