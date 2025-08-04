using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Bakeland
{
    public static class Read
    {
        // === CORE GET METHOD ===
        private static async Task<string> SendGetAsync(string path)
        {
            string url = GetEnvVars.Get("READ_API_URL") + path;
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("READ_API_KEY"));

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"GET API Failed: {request.error}");
                return null;
            }
        }

        // === API METHODS ===

        public static Task<string> GetAllPoolsGlobalAsync() =>
            SendGetAsync("/global/pools");

        public static Task<string> GetMostStakedPoolGlobalAsync() =>
            SendGetAsync("/global/most-staked-pool");

        public static Task<string> GetMostRektPoolGlobalAsync() =>
            SendGetAsync("/global/most-rekt-pool");

        public static Task<string> GetGlobalStakedBudsAsync() =>
            SendGetAsync("/global/staked-buds");

        public static Task<string> GetAllPoolsOfUserAsync(string userAddress) =>
            SendGetAsync($"/user/{userAddress}/pools");

        public static Task<string> GetStakingRewardsForUserAsync(string userAddress) =>
            SendGetAsync($"/user/rewards/staking/{userAddress}");

        public static Task<string> GetHuddleSessionKeyAsync(string userAddress, string room) =>
            SendGetAsync($"/user/huddle/{userAddress}/{room}");

        public static Task<string> GetDeployedChainsAsync() =>
            SendGetAsync("/chain/deployed");

        public static Task<string> GetTvlByChainAsync(string chain) =>
            SendGetAsync($"/chain/{chain}/tvl");

        public static Task<string> GetPoolsByChainAsync(string chain) =>
            SendGetAsync($"/chain/{chain}/pools");

        public static Task<string> GetMostStakedPoolAsync(string chain) =>
            SendGetAsync($"/chain/{chain}/most-staked-pool");

        public static Task<string> GetMostRektPoolAsync(string chain) =>
            SendGetAsync($"/chain/{chain}/most-rekt-pool");

        public static Task<string> GetPoolInfoAsync(string chain, string poolId) =>
            SendGetAsync($"/chain/{chain}/pool/{poolId}");

        public static Task<string> GetNumberOfPoolsAsync(string chain) =>
            SendGetAsync($"/chain/{chain}/number-of-pools");

        public static Task<string> GetCctxFeeAsync(string src, string dest, int type) =>
            SendGetAsync($"/fee/cctx/{src}/{dest}/{type}");

        public static Task<string> GetRaidFeeAsync(string chain, string poolId, string risk) =>
            SendGetAsync($"/fee/raid/{chain}/{poolId}/{risk}");

        public static Task<string> GetRaidRewardsAsync(string chain, string poolId) =>
            SendGetAsync($"/rewards/raid/{chain}/{poolId}");
    }
}
