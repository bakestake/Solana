using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Mult_UIController : MonoBehaviour
{
    public Button muteButton;
    public GameObject overlayMuteButton;

    public Button joingroupTalk;

    public TextMeshProUGUI nickNameText;

    public void NickNameToggle(bool nickNameEnabled, string nickName)
    {
        nickNameText.enabled  = nickNameEnabled;
        nickNameText.text = nickName;
    }

    public void JoinGroupTalk()
    {
        // Change Channel of the Player
        
    }
}
