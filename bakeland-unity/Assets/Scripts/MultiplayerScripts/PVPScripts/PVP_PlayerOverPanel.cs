using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using TMPro;

public class PVP_PlayerOverPanel :MonoBehaviour
{
    [SerializeField] private GameObject _playerOverviewPrefab = null;

    private Dictionary<PlayerRef, GameObject>
        _playerListEntries = new Dictionary<PlayerRef, GameObject>();

    private Dictionary<PlayerRef, string> _playerNickNames = new Dictionary<PlayerRef, string>();
    private Dictionary<PlayerRef, float> _playerScores = new Dictionary<PlayerRef, float>();
    private Dictionary<PlayerRef, float> _playerLives = new Dictionary<PlayerRef, float>();

    // Creates a new Overview Entry
    public void AddEntry(PlayerRef playerRef, Health_Playerr playerDataNetworked)
    {
        if (_playerListEntries.ContainsKey(playerRef)) return;
        if (playerDataNetworked == null) return;

        var entry = Instantiate(_playerOverviewPrefab, this.transform);
        //entry.transform.localScale = Vector3.one;
        //entry.color = SpaceshipVisualController.GetColor(playerRef.PlayerId);

        string nickName = String.Empty;
        float lives = 100;
        float score = 0;

        _playerNickNames.Add(playerRef, nickName);
        _playerScores.Add(playerRef, score);
        _playerLives.Add(playerRef, lives);

        _playerListEntries.Add(playerRef, entry);

        UpdateEntry(playerRef, entry);
    }

    // Removes an existing Overview Entry
    public void RemoveEntry(PlayerRef playerRef)
    {
        if (_playerListEntries.TryGetValue(playerRef, out var entry) == false) return;

        if (entry != null)
        {
            Destroy(entry.gameObject);
        }

        _playerNickNames.Remove(playerRef);
        _playerScores.Remove(playerRef);
        _playerLives.Remove(playerRef);

        _playerListEntries.Remove(playerRef);
    }

    public void UpdateLives(PlayerRef player, float lives)
    {
        if (_playerListEntries.TryGetValue(player, out var entry) == false) return;

        _playerLives[player] = lives;
        UpdateEntry(player, entry);
    }

    public void UpdateScore(PlayerRef player, float score)
    {
        if (_playerListEntries.TryGetValue(player, out var entry) == false) return;

        _playerScores[player] = score;
        UpdateEntry(player, entry);
    }

    public void UpdateNickName(PlayerRef player, string nickName)
    {
        if (_playerListEntries.TryGetValue(player, out var entry) == false) return;

        _playerNickNames[player] = nickName;
        UpdateEntry(player, entry);
    }

    private void UpdateEntry(PlayerRef player, GameObject entry)
    {
        var nickName = _playerNickNames[player];
        var score = _playerScores[player];
        var lives = _playerLives[player];

        //entry.text = $"{nickName}\nScore: {score}\nLives: {lives}";
        entry.GetComponent<PVP_HealthBar>().UpdateEntry(lives,score,nickName);
    }
}
