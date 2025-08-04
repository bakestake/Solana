using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HConsumePanel : MonoBehaviour
{
    [SerializeField] private HuddleConsumeButton PlayerToConsume = null;

    private Dictionary<PlayerRef, HuddleConsumeButton>
    _HuddleConsumeEntries = new Dictionary<PlayerRef, HuddleConsumeButton>();

    private Dictionary<PlayerRef, string> _playerNickNames = new Dictionary<PlayerRef, string>();
    private Dictionary<PlayerRef, string> _playerRemotePeerID = new Dictionary<PlayerRef, string>();
    private Dictionary<PlayerRef, Mult_PlayerDataNetworked> _playerData = new Dictionary<PlayerRef, Mult_PlayerDataNetworked>();

    // Creates a new Overview Entry
    public void AddEntry(PlayerRef playerRef)
    {
        if (_HuddleConsumeEntries.ContainsKey(playerRef)) return;

        var entry = Instantiate(PlayerToConsume, this.transform);

        string nickName = String.Empty;

        _playerNickNames.Add(playerRef, nickName);

        _HuddleConsumeEntries.Add(playerRef, entry);

        _playerRemotePeerID.Add(playerRef, String.Empty);

        _playerData.Add(playerRef, null);

        UpdateEntry(playerRef, entry);
    }

    public void UpdateNickName(PlayerRef player, string nickName, string remotePeerID, Mult_PlayerDataNetworked mult_PlayerData)
    {
        if (_HuddleConsumeEntries.TryGetValue(player, out var entry) == false) return;

        _playerNickNames[player] = nickName;
        _playerRemotePeerID[player] = remotePeerID;
        _playerData[player] = mult_PlayerData;
        UpdateEntry(player, entry);
    }

    private void UpdateEntry(PlayerRef player, HuddleConsumeButton entry)
    {
        var nickName = _playerNickNames[player];

        entry.PlayerNickname.text = $"{nickName}";
        entry.SetRemotePeerID(_playerRemotePeerID[player]);
        entry.SetPlayerScript(_playerData[player]);
    }

    // Removes an existing Overview Entry
    public void RemoveEntry(PlayerRef playerRef)
    {
        if (_HuddleConsumeEntries.TryGetValue(playerRef, out var entry) == false) return;

        if (entry != null)
        {
            Destroy(entry.gameObject);
        }

        //Fail Safe on Stop Consuming peer
        _playerData[playerRef].StopConsumingPeer(_playerRemotePeerID[playerRef]);

        _playerNickNames.Remove(playerRef);
        _playerRemotePeerID.Remove(playerRef);
        _playerData.Remove(playerRef);
        _HuddleConsumeEntries.Remove(playerRef);
    }
}
