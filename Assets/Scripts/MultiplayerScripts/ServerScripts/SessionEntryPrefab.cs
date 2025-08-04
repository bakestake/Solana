using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SessionEntryPrefab : MonoBehaviour
{
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI PlayerCount;
    public Button JoinButton;

    private void Start()
    {
        JoinButton.onClick.AddListener(JoinSession);
    }

    private void JoinSession()
    {
        //ServerStartMenu.Instance.StartClient(RoomName.text);
    }

}
