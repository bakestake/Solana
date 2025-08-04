using CandyCoded.env;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Solana.Unity.Programs;
using Solana.Unity.Rpc;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Wallet;
using Solana.Unity.SDK;



#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

namespace Bakeland
{

    // Serializable classes for Unity JsonUtility
    [Serializable]
    public class AddToBasketPayload
    {
        public string playerAddress;
        public string itemId;
        public int amount;
    }

    [Serializable]
    public class MintBasketPayload
    {
        public string playerAddress;
    }

    [Serializable]
    public class WritePayload
    {
        public string functionName;
        public object[] args;
    }

    [Serializable]
    class ApiResponse
    {
        public string result;
    }

    [Serializable]
    public class InventoryItem
    {
        public InventoryItem(string name, string id, string balance)
        {
            this.name = name;
            this.id = id;
            this.balance = balance;
        }

        public string name { get; set; }
        public string id { get; set; }
        public string balance { get; set; }
    }

    [Serializable]
    public class InventoryResponse
    {
        public List<InventoryItem> inventory { get; set; } = new();
    }

    [Serializable]
    public class GoldBalanceResponse
    {
        public string goldBalance { get; set; }
    }

    [Serializable]
    public class IssueItemPayload
    {
        public string destinationUserReferenceId;
        public int amount;
    }


    [Serializable]
    public class RegisterToGameShiftPayload
    {
        public string referenceId;
        public string email;
        public string externalWalletAddress;
    }

    public class UserItemSimple
    {
        public string itemId;
        public string name;
        public string quantity;
    }

    public class UserItems
    {
        public List<UserItemSimple> items = new List<UserItemSimple>();
    }

    public class BurnItemPayload
    {
        public string destinationUserReferenceId;
        public string destinationWallet;
        public string quantity;
   
    }

    [Serializable]
    public class GameShiftItem
    {
        public string id;
        public string name;
        public string symbol;
        public string description;
        public string imageUrl;
        public string mintAddress;
        public string status;
        public bool forSale;
        public int created;
        public string environment;
        public bool imported;
        public GameShiftCollection collection;
        public GameShiftOwner owner;
        public object price;
        public int decimals;
        public GameShiftAttribute[] attributes;
    }

    [Serializable]
    public class GameShiftCollection
    {
        public string id;
        public string name;
        public string description;
        public string environment;
        public string imageUrl;
        public bool imported;
        public string mintAddress;
        public int created;
        public string contains;
        public GameShiftStats stats;
    }

    [Serializable]
    public class GameShiftStats
    {
        public int numMinted;
        public int floorPrice;
        public int numListed;
        public int numOwners;
    }

    [Serializable]
    public class GameShiftOwner
    {
        public string address;
        public string referenceId;
    }

    [Serializable]
    public class GameShiftAttribute
    {
        public string traitType;
        public string value;
    }

    [Serializable]
    public class GameShiftItemEntry
    {
        public string type;
        public GameShiftItem item;
        public string quantity;
    }

    [Serializable]
    public class GameShiftMeta
    {
        public int page;
        public int perPage;
        public int totalPages;
        public int totalResults;
    }

    [Serializable]
    public class GameShiftUserItemsResponse
    {
        public GameShiftItemEntry[] data;
        public GameShiftMeta meta;
    }

    [Serializable]
    public class GameShiftSingleItemResponse
    {
        public string type;
        public GameShiftItem item;
        public string quantity;
    }

    [Serializable]
    public class UidToGameShiftMapping
    {
        public string UID;
        public string gameshiftId;
    }

    [Serializable]
    public class UidToGameShiftMappings
    {
        public List<UidToGameShiftMapping> mappings = new List<UidToGameShiftMapping>();
    }

    public class InventoryApi : MonoBehaviour
    {
        private static InventoryApi instance;
        private static Dictionary<string, string> uidToGameShiftCache;
        private static Dictionary<string, string> gameShiftToUidCache;

        public static InventoryResponse playerInventory { get; private set; }

        public static string goldBalance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                LoadUidToGameShiftMappings();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private static void LoadUidToGameShiftMappings()
        {
            try
            {
                TextAsset jsonFile = Resources.Load<TextAsset>("uidToGameshiftId");
                if (jsonFile != null)
                {
                    var mappings = JsonConvert.DeserializeObject<List<UidToGameShiftMapping>>(jsonFile.text);
                    uidToGameShiftCache = new Dictionary<string, string>();
                    gameShiftToUidCache = new Dictionary<string, string>();
                    
                    foreach (var mapping in mappings)
                    {
                        uidToGameShiftCache[mapping.UID] = mapping.gameshiftId;
                        gameShiftToUidCache[mapping.gameshiftId] = mapping.UID;
                    }
                    
                    Debug.Log($"Loaded {uidToGameShiftCache.Count} UID to GameShift ID mappings");
                }
                else
                {
                    Debug.LogError("Could not load uidToGameshiftId.json file from Resources folder");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load UID to GameShift mappings: {e.Message}");
            }
        }

        /// <summary>
        /// Converts a UID to its corresponding GameShift ID
        /// </summary>
        /// <param name="uid">The UID to convert</param>
        /// <returns>The corresponding GameShift ID, or null if not found</returns>
        public static string GetGameShiftIdFromUid(string uid)
        {
            if (uidToGameShiftCache == null)
            {
                LoadUidToGameShiftMappings();
            }

            if (uidToGameShiftCache != null && uidToGameShiftCache.ContainsKey(uid))
            {
                return uidToGameShiftCache[uid];
            }

            Debug.LogWarning($"No GameShift ID found for UID: {uid}");
            return null;
        }

        /// <summary>
        /// Converts a UID to its corresponding GameShift ID (integer version)
        /// </summary>
        /// <param name="uid">The UID to convert</param>
        /// <returns>The corresponding GameShift ID, or null if not found</returns>
        public static string GetGameShiftIdFromUid(int uid)
        {
            return GetGameShiftIdFromUid(uid.ToString());
        }

        public static string GetUidFromGameShiftId(string gameshiftId){
            if (gameShiftToUidCache == null)
            {
                LoadUidToGameShiftMappings();
            }

            if (gameShiftToUidCache != null && gameShiftToUidCache.ContainsKey(gameshiftId))
            {
                return gameShiftToUidCache[gameshiftId];
            }

            Debug.LogWarning($"No UID found for GameShift ID: {gameshiftId}");
            return null;
        }

        public static async Task IssueItem(string playerAddress, string itemId, int amount)
        {
            Debug.Log("ISSUE ASSET IS CALLED");
            Debug.Log("Params received: " + playerAddress + ", " + itemId + ", " + amount);

            try{
                string baseUrl = GetEnvVars.Get("GAMESHIFT_API_URL");
                string path = $"/stackable-assets/{GetGameShiftIdFromUid(itemId)}";

                string url = baseUrl + path;

                var payload = new IssueItemPayload
                {
                    destinationUserReferenceId = playerAddress,
                    amount = amount
                };

                string json = JsonUtility.ToJson(payload);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

                UnityWebRequest request = new UnityWebRequest(url, "POST"){
                    uploadHandler = new UploadHandlerRaw(bodyRaw),
                    downloadHandler = new DownloadHandlerBuffer()
                };

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("accept", "application/json");
                request.SetRequestHeader("x-api-key", GetEnvVars.Get("GAMESHIFT_API_KEY"));

                await request.SendWebRequest();
            }catch(Exception e){
                Debug.LogError("Failed to issue item: " + e.Message);
            }
            
            
        }

        public static async Task RegisterToGameShift(string playerAddress){
            if(await IsUserRegistered(playerAddress)){
                Debug.Log("User is already registered");
                return;
            }

            try{
                string baseUrl = GetEnvVars.Get("GAMESHIFT_API_URL");
                string path = $"/users";

                var payload = new RegisterToGameShiftPayload
                {
                    referenceId = playerAddress,
                    email = $"{playerAddress}@bakeland.com",
                    externalWalletAddress = playerAddress
                };

                string json = JsonUtility.ToJson(payload);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

                UnityWebRequest request = new UnityWebRequest(baseUrl + path, "POST"){
                    uploadHandler = new UploadHandlerRaw(bodyRaw),
                    downloadHandler = new DownloadHandlerBuffer()
                };

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("accept", "application/json");
                request.SetRequestHeader("x-api-key", GetEnvVars.Get("GAMESHIFT_API_KEY"));

                await request.SendWebRequest();
            }catch(Exception e){
                Debug.LogError("Failed to register to GameShift: " + e.Message);
            }
        }

        public static async Task<bool> IsUserRegistered(string playerAddress){
            try{
                string baseUrl = GetEnvVars.Get("GAMESHIFT_API_URL");
                string path = $"/users/?referenceId={playerAddress}";

                UnityWebRequest request = new UnityWebRequest(baseUrl + path, "GET"){
                    downloadHandler = new DownloadHandlerBuffer()
                };

                request.SetRequestHeader("accept", "application/json");
                request.SetRequestHeader("x-api-key", GetEnvVars.Get("GAMESHIFT_API_KEY"));

                await request.SendWebRequest();

                if(request.result == UnityWebRequest.Result.Success){
                    return true;
                }else{
                    return false;
                }
                
            }catch(Exception e){
                Debug.LogError("Failed to check if user is registered: " + e.Message);
                return false;
            }
        }

        public static async Task<InventoryResponse> GetUserInventory(string playerAddress)
        {
            try
            {
                string baseUrl = GetEnvVars.Get("GAMESHIFT_API_URL");
                string path = $"/users/{playerAddress}/items?types=StackableAsset";
                string url = baseUrl + path;

                UnityWebRequest request = new UnityWebRequest(url, "GET")
                {
                    downloadHandler = new DownloadHandlerBuffer()
                };
                request.SetRequestHeader("accept", "application/json");
                request.SetRequestHeader("x-api-key", GetEnvVars.Get("GAMESHIFT_API_KEY"));

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    // Use strongly-typed deserialization
                    var response = JsonConvert.DeserializeObject<GameShiftUserItemsResponse>(json);
                    playerInventory = new InventoryResponse();
                    
                    if (response?.data != null)
                    {
                        foreach (var entry in response.data)
                        {
                            if (entry.item != null)
                            {
                                string itemId = GetUidFromGameShiftId(entry.item.id);
                                string name = entry.item.name;
                                string quantity = !string.IsNullOrEmpty(entry.quantity) ? entry.quantity : "1";

                                playerInventory.inventory.Add(new InventoryItem(name, itemId, quantity));
                            }
                        }
                    }
                    return playerInventory;
                }
                else
                {
                    Debug.LogError($"Failed to fetch user items: {request.error}, {request.downloadHandler.text}");
                    return new InventoryResponse();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to fetch user items: " + e.Message);
                throw new Exception("Failed to fetch user items: " + e.Message);
            }
        }

        public static async Task<UserItemSimple> FetchUserItemBalance(string playerAddress, string itemId)
        {
            try
            {
                string baseUrl = GetEnvVars.Get("GAMESHIFT_API_URL");
                string path = $"/users/{playerAddress}/items/{GetGameShiftIdFromUid(itemId)}";
                string url = baseUrl + path;

                UnityWebRequest request = new UnityWebRequest(url, "GET")
                {
                    downloadHandler = new DownloadHandlerBuffer()
                };
                request.SetRequestHeader("accept", "application/json");
                request.SetRequestHeader("x-api-key", GetEnvVars.Get("GAMESHIFT_API_KEY"));

                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    // Use strongly-typed deserialization
                    var response = JsonConvert.DeserializeObject<GameShiftSingleItemResponse>(json);
                    
                    if (response?.item != null)
                    {
                        string fetchedItemId = response.item.id;
                        string name = response.item.name;
                        string quantity = !string.IsNullOrEmpty(response.quantity) ? response.quantity : "1";
                        
                        return new UserItemSimple
                        {
                            itemId = fetchedItemId,
                            name = name,
                            quantity = quantity
                        };
                    }
                    return new UserItemSimple();
                }
                else
                {
                    Debug.LogError($"Failed to fetch user item balance: {request.error}, {request.downloadHandler.text}");
                    return new UserItemSimple();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to fetch user item balance: " + e.Message);
                return new UserItemSimple();
            }
        }

        public static async Task BurnItemsGameshift(string playerAddress, string itemId, int amount){
            try{
                string baseUrl = GetEnvVars.Get("GAMESHIFT_API_URL");
                string path = $"users/{playerAddress}/items/{GetGameShiftIdFromUid(itemId)}/transfer";

                string burnAddress = "1nc1nerator11111111111111111111111111111111";

                var payload = new BurnItemPayload
                {
                    destinationUserReferenceId = burnAddress,
                    destinationWallet = burnAddress,
                    quantity = amount.ToString()
                };

                string json = JsonUtility.ToJson(payload);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

                UnityWebRequest request = new UnityWebRequest(baseUrl + path, "POST"){
                    uploadHandler = new UploadHandlerRaw(bodyRaw),
                    downloadHandler = new DownloadHandlerBuffer()
                };

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("accept", "application/json");
                request.SetRequestHeader("x-api-key", GetEnvVars.Get("GAMESHIFT_API_KEY"));

                await request.SendWebRequest();

                if(request.result == UnityWebRequest.Result.Success){
                    Debug.Log("Items burned successfully");
                    Debug.Log("API Response content: " + request.downloadHandler.text);
                }else{
                    Debug.LogError("Failed to burn items: " + request.error);
                }
            }catch(Exception e){
                Debug.LogError("Failed to burn items: " + e.Message);
            }
        }

        [Obsolete]
        public static async Task BurnBonk(string playerAddress, int amount){
            try{
                IRpcClient rpcClient = ClientFactory.GetClient(Cluster.MainNet);

                RequestResult<ResponseValue<BlockHash>> blockHash = await rpcClient.GetRecentBlockHashAsync();
                WalletBase connectedWallet = Solana.Unity.SDK.Web3.Wallet;

                Account fromAccount = connectedWallet.Account;

                PublicKey tokenMintAddress = (PublicKey)"DezXAZ8z7PnrnRJjz3wXBoRgixCa6xjnB7YaB1pPB263";

                TokenAccount[] tokenAccounts = await connectedWallet.GetTokenAccounts();

                TokenAccount bonkAssociatedAccount = new();

                foreach(TokenAccount tokenAccount in tokenAccounts){
                    if(tokenAccount.Account.Data.Parsed.Info.Mint == tokenMintAddress){
                        bonkAssociatedAccount = tokenAccount;
                    }
                }

                byte[] tx = new TransactionBuilder()
                    .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                    .SetFeePayer(fromAccount)
                    .AddInstruction(TokenProgram.Burn((PublicKey)bonkAssociatedAccount.PublicKey, tokenMintAddress, 42000 * 10^4, fromAccount.PublicKey))
                    .AddInstruction(MemoProgram.NewMemo(fromAccount.PublicKey, "Burning bonk at bakeland"))
                    .Build(fromAccount);


                Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
                RequestResult<ResponseValue<SimulationLogs>> txSim = await rpcClient.SimulateTransactionAsync(tx);
                string logs = string.Join("\n", txSim.Result.Value.Logs);
                Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
                RequestResult<string> firstSig = await rpcClient.SendTransactionAsync(tx);
                Console.WriteLine($"First Tx Signature: {firstSig.Result}");

            }catch(Exception e){
                Debug.LogError("Failed to burn bonk: " + e.Message);
            }
        }


        public static async Task<string> GetGoldBalance(string playerAddress)
        {
            try
            {
                var res = await FetchUserItemBalance(playerAddress, GetGameShiftIdFromUid("696969"));
                return res.quantity;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to get gold Balance" + e.Message);
                throw new Exception("Failed to get gold Balance" + e.Message);
            }
        }

        public static async Task MintGold(string playerAddress, int amount)
        {
            try
            {
                await IssueItem(playerAddress, GetGameShiftIdFromUid("696969"), amount);

            }
            catch (Exception e)
            {
                Debug.LogError("Failed to burn: " + e.Message);
            }
        }

        public static async Task BurnGold(string playerAddress, int amount)
        {
            try
            {
                await BurnItemsGameshift(playerAddress, GetGameShiftIdFromUid("696969"), amount);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to burn: " + e.Message);
            }
        }

        public static async Task Mintitems(string playerAddress, int[] itemIds, int[] itemAmounts)
        {
            try
            {
                // Create arrays for the burnBatch function
                int[] ids = itemIds;
                int[] amounts = itemAmounts;

                for(int i = 0; i < itemIds.Length; i++){
                    await IssueItem(playerAddress, itemIds[i].ToString(), itemAmounts[i]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to burn: " + e.Message);
            }
        }

        public static async Task MintItem(string playerAddress, int id, int amount)
        {
            try
            {
                await IssueItem(playerAddress, id.ToString(), amount);

            }
            catch (Exception e)
            {
                Debug.LogError("Failed to burn: " + e.Message);
            }
        }

        /// @dev function to craft a specific item
        /// @dev burns the ids for corresponding amounts passed in params
        /// @param playerAddress - user's connected wallet address
        /// @param id - id of item to be burnt
        /// @param amount - amount/quantity to be burnt
        public static async Task BurnItems(string playerAddress, int[] itemIds, int[] itemAmounts)
        {
            try
            {
                // Create arrays for the burnBatch function
                int[] ids = itemIds;
                int[] amounts = itemAmounts;

                for(int i = 0; i < itemIds.Length; i++){
                    await BurnItemsGameshift(playerAddress, itemIds[i].ToString(), itemAmounts[i]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to burn: " + e.Message);
            }
        }

        public static async Task BurnItem(string playerAddress, int id, int amount)
        {
            try
            {
                await BurnItemsGameshift(playerAddress, id.ToString(), amount);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to burn: " + e.Message);
            }
        }


    }
}