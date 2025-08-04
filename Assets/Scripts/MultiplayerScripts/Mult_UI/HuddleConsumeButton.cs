using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HuddleConsumeButton : MonoBehaviour
{
    public TextMeshProUGUI PlayerNickname = null;

    private string remotePeerID = string.Empty;

    private Mult_PlayerDataNetworked dataNetworked;

    public void SetPlayerScript(Mult_PlayerDataNetworked PlayerScript)
    {
        if (PlayerScript)
        {
            dataNetworked = PlayerScript;
        }
    }

    public void SetRemotePeerID(string peerID)
    {
        remotePeerID = peerID;
    }

    public void ConsumePeer()
    {
        if (dataNetworked != null)
        {
            dataNetworked.StartConsumingPeer(remotePeerID);
        }
    }
}
