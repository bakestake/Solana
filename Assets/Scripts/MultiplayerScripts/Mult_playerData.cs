using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class Mult_playerData : MonoBehaviour
{
    private string _nickName = null;
    private string ipAddress = null;
    private int selectedCharacterIndex;

    private void Start()
    {
        var count = FindObjectsOfType<Mult_playerData>().Length;
        if (count > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetCharacterSelectorIndex(int index)
    {
        //selectedCharacterIndex = Random.Range(0, 9);
        selectedCharacterIndex = index;
        DebugUtils.Log("The value index selected is: " + selectedCharacterIndex);
    }

    public int GetCharacter()
    {
        return selectedCharacterIndex;
    }

    public void SetNickName(string nickName)
    {
        _nickName = nickName;
    }

    public string GetNickName()
    {
        if (string.IsNullOrWhiteSpace(_nickName))
        {
            _nickName = GetRandomNickName();
        }

        return _nickName;
    }

    public static string GetRandomNickName()
    {
        var rngPlayerNumber = Random.Range(0, 9999);
        return $"Player {rngPlayerNumber.ToString("0000")}";
    }

    public void SetIpAddress(string ip)
    {
        ipAddress = ip;
    }

    public string GetIpAddress()
    {
        return ipAddress;
    }
}
