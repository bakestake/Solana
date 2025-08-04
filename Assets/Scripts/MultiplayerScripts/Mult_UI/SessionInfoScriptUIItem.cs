using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System;

public class SessionInfoScriptUIItem : MonoBehaviour
{
    //Private Memebers
    [SerializeField]
    private TextMeshProUGUI _sessionName;
    [SerializeField]
    private TextMeshProUGUI _playerCount;
    [SerializeField]
    private Button _joinButton;

    SessionInfo _sessionInfo;

    //Event
    public event Action<SessionInfo> OnJoinSession;

    //Public Methods
    public void SetInformation(SessionInfo sessionInfo)
    {
        this._sessionInfo = sessionInfo;

        _sessionName.text = sessionInfo.Name;
        _playerCount.text = $"{ sessionInfo.PlayerCount.ToString()}/{sessionInfo.MaxPlayers.ToString()}";

        bool _isJoinButtonActive = true;

        if (sessionInfo.PlayerCount >= sessionInfo.MaxPlayers)
        {
            _isJoinButtonActive = false;
        }

        _joinButton.gameObject.SetActive(_isJoinButtonActive);
    }

    public void OnClick()
    {
        //Invoke the On Join Session
        OnJoinSession?.Invoke(_sessionInfo);
    }


}
