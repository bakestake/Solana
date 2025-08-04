using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Fusion;
using NBitcoin.Protocol;

public class CreateRoomHuddle : NetworkBehaviour
{

    [System.Serializable]
    public class CreateRoomResponse
    {
        public string message;
        public Data data;

        [System.Serializable]
        public class Data
        {
            public string roomId;
        }
    }

    [System.Serializable]
    public class ErrorResponse
    {
        public bool success;
        public ErrorDetails error;

        [System.Serializable]
        public class ErrorDetails
        {
            public Issue[] issues;

            [System.Serializable]
            public class Issue
            {
                public string code; // The code of the error (e.g., "invalid_type")
                public bool expected; // Expected type (e.g., "boolean")
                public string received; // Received type (e.g., "string")
                public string[] path; // The path to the property that caused the error
                public string message; // The error message
            }
        }
    }


    public async Task<(string,string)> CreateRoom()
    {
        DebugUtils.Log("Huddle prefab created Successfully");
        string apiUrl = "https://api.huddle01.com/api/v1/create-room";
        string APIKey = "ak_yFRNXsVZVXa2WxHk";
        string hostaddress = "0x29f54719E88332e70550cf8737293436E9d7b10b";

        string roomId = "null";
        string message = "null";
        try
        {
            await CreateRoomTask(apiUrl, "Huddle01-Test", new string[] { hostaddress },APIKey, (string res) =>
            {
                CreateRoomResponse response = JsonUtility.FromJson<CreateRoomResponse>(res);
                DebugUtils.Log("The unparsed json value is:" + res);

                if (response != null)
                {
                    roomId = response.data.roomId;
                    message = response.message;
                }
                else
                {
                    DebugUtils.LogError("Failed to deserialize JSON response.");
                }
            });
        }
        catch (Exception e)
        {
            DebugUtils.LogError("Failed to get Response from Huddle create Room API with reason" + e);
        }
        return (roomId, message);
    }

    private async Task CreateRoomTask(string apiUrl, string titl, string[] hostWallet, string API_Key, Action<string> callback)
    {
        try
        {
            // Create the payload as a C# object
            var payload = new
            {
                title = titl,
                hostWallets = hostWallet
            };
            string jsonBody = JsonUtility.ToJson(payload);
            using (UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("x-api-key", API_Key);
                await webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    DebugUtils.Log(apiUrl);
                    callback(null);
                }
                else if(webRequest.result == UnityWebRequest.Result.Success)
                {
                    DebugUtils.Log("Room created Successfully");
                    string jsonResponse = webRequest.downloadHandler.text;
                    callback(jsonResponse);
                }
            }
        }
        catch (Exception e)
        {
            DebugUtils.LogError("Api error occurred: " + e.Message + "reason" + e.ToString());
        }
    }
}
