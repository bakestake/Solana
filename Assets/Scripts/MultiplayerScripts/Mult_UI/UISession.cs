using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISession : UIBehaviour
{
    // PRIVATE MEMBERS

    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private TextMeshProUGUI _playerCount;
    [SerializeField]
    private TextMeshProUGUI _map;
    [SerializeField]
    private Image _mapImage;
    [SerializeField]
    private TextMeshProUGUI _gameplayType;
    [SerializeField]
    private TextMeshProUGUI _state;
    [SerializeField]
    //private string _emptyField = "-";

    // PUBLIC METHODS

    public void SetData(SessionInfo sessionInfo)
    {
        if (sessionInfo == null)
            return;

        int playerCount = sessionInfo.PlayerCount;
        int maxPlayers = sessionInfo.MaxPlayers;

        //It will always be server so
        playerCount -= 1;
        maxPlayers -= 1;

        _name.text = //sessionInfo.GetDisplayName();
        _playerCount.text = $"{playerCount}/{maxPlayers}";

        // We do not have lobby state for now
        _state.text = sessionInfo.IsOpen == true ? "In Game" : "Finished";
    }
}
