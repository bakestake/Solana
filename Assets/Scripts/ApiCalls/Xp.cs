using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

namespace Bakeland
{
    public class Xp : MonoBehaviour
    {
  
        private static Xp instance;

        private void Awake()
        {
            // @dev - single instance across the scenes
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

     
        /// @dev - Call this to mint XP based on type and interval.
        /// @param type - ["farmer","narc"]
        /// @param interval - ["daily", "weeekly", "monthly"] 
        /// @param evmAddress - The player's wallet address
        public static void ClaimXp(string type, string interval, string evmAddress)
        {
            if (instance != null)
                instance.StartCoroutine(instance.SendClaimXpRequest(type, interval, evmAddress));
        }

        // Internal coroutine to send XP claim request with type and interval
        private IEnumerator SendClaimXpRequest(string type, string interval, string evmAddress)
        {
            string url = GetEnvVars.Get("XP_API_URL")+$"{type}/{interval}"; 
            string jsonBody = $"{{\"evmAddress\": \"{evmAddress}\"}}";

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("XP_API_URL"));

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"XP Claim Success: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"XP Claim Failed: {request.error}");
            }
        }

        /// @dev: call this after user performs a restake action.
        /// @param - evmAddress connnected wallet address of user
        public static void ClaimRestakeXp(string evmAddress)
        {
            if (instance != null)
                instance.StartCoroutine(instance.SendSimpleXpRequest("restake", evmAddress));
        }

        /// @dev: call this after user performs a yeet action.
        /// @param - evmAddress connnected wallet address of user
        public static void ClaimYeetXp(string evmAddress)
        {
            if (instance != null)
                instance.StartCoroutine(instance.SendSimpleXpRequest("yeet", evmAddress));
        }

        // Internal coroutine to handle restake/yeet
        private IEnumerator SendSimpleXpRequest(string action, string evmAddress)
        {
            string url = GetEnvVars.Get("XP_API_URL")+$"/xp/{action}";
            string jsonBody = $"{{\"evmAddress\": \"{evmAddress}\"}}";

            UnityWebRequest request = new(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("XP_API_KEY"));

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"{action} XP Success: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"{action} XP Failed: {request.error}");
            }
        }
    }
}
