using CandyCoded.env;
using NBitcoin.Protocol;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Bakeland
{

    /// <summary>
    /// API class for handling user and referral related operations
    /// </summary>
    /// <remarks>
    /// This class provides methods to interact with the user and referral API endpoints.
    /// It handles user creation, retrieval, wallet updates, and referral operations.
    public static class UserAndReferralApi
    {
        [Serializable]
        public class GetUserResponse
        {
            public string username;
            public string referralCode;

            [JsonConstructor]
            public GetUserResponse(string username, string referralCode)
            {
                this.username = username;
                this.referralCode = referralCode;
            }
        }


        [Serializable]
        public class LeaderboardEntry
        {
            public string userAddress;
            public string username;
            public int score;
            public int rank;
            public string questName;
            public string unit;

            // Parameterless constructor for deserialization
            public LeaderboardEntry() {}

            [JsonConstructor]
            public LeaderboardEntry(string userAddress, string username, int score, int rank, string questName, string unit)
            {
                this.userAddress = userAddress;
                this.username = username;
                this.score = score;
                this.rank = rank;
                this.questName = questName;
                this.unit = unit;
            }
        }

        [System.Serializable]
        public class GetUserFarmlandResponse
        {
            public string userAddress;
            public List<CellData> cells;
        }

        [System.Serializable]
        public class CellData
        {
            public int x;
            public int y;
            public string cropId;
            public string plantedAt;
            public bool harvestable;
        }


        [Serializable]
        public class LeaderboardResponse
        {
            public string questId;
            public string questName;
            public string scoreType;
            public string unit;
            public List<LeaderboardEntry> leaderboard;
            public int totalEntries;

            // Parameterless constructor for deserialization
            public LeaderboardResponse() {}

            [JsonConstructor]
            public LeaderboardResponse(string questId, string questName, string scoreType, string unit, List<LeaderboardEntry> leaderboard, int totalEntries)
            {
                this.questId = questId;
                this.questName = questName;
                this.scoreType = scoreType;
                this.unit = unit;
                this.leaderboard = leaderboard;
                this.totalEntries = totalEntries;
            }
        }

        [Serializable]
        public class QuestCompletion
        {
            public string questId;
            public bool completed;
            public string completedAt;
            public string createdAt;
            public string updatedAt;

            // Parameterless constructor for deserialization
            public QuestCompletion() {}
        }

        [Serializable]
        public class GetQuestCompletionsResponse
        {
            public string userAddress;
            public QuestCompletion[] questCompletions;
            public QuestSummary summary;
            public QuestPagination pagination;

            // Parameterless constructor for deserialization
            public GetQuestCompletionsResponse() {}

            [JsonConstructor]
            public GetQuestCompletionsResponse(string userAddress, QuestCompletion[] questCompletions, QuestSummary summary, QuestPagination pagination)
            {
                this.userAddress = userAddress;
                this.questCompletions = questCompletions;
                this.summary = summary;
                this.pagination = pagination;
            }
        }



        [Serializable]
        public class QuestSummary
        {
            public int totalQuests;
            public int completedQuests;
            public int incompletedQuests;
            public int completionRate;

            // Parameterless constructor for deserialization
            public QuestSummary() {}
        }

        [Serializable]
        public class QuestPagination
        {
            public int count;
            public string lastEvaluatedKey;
            public bool hasMore;

            // Parameterless constructor for deserialization
            public QuestPagination() {}
        }

        [Serializable]
        public class CreateUserRequestBody
        {
            public string userAddress;
            public string username;
            public string referralCode;
            public string referrerCode;
            public string inAppWallet;
        }

        [Serializable]
        public class SubmitQuestScoreRequestBody
        {
            public string userAddress;
            public string questId;
            public int score;
            public string username;
        }

        [Serializable]
        public class UpdateWalletRequestBody
        {
            public string wallet;

        }

        [Serializable]
        public class UpdateUserNameRequest
        {
            public string username;

        }

        [Serializable]
        public class ConsumeCodeRequestBody
        {
            public string code;

        }

        [Serializable]
        public class UpdateQuestScoreRequestBody
        {
            public int score;

        }

        [Serializable]
        public class UpdateUserFarmlandRequestBody
        {
            public List<CellData> cells;
        }


        /// <summary>
        /// Creates a new user in the system
        /// </summary>
        /// <param name="userAddress">The active wallet address of the user</param>
        /// <param name="username">The desired username for the user</param>
        /// <param name="referralCode">The unique referral code entered by user</param>
        /// <param name="referrerCode">The referral code of the user who referred this user (if any)</param>
        /// <param name="inAppWallet">The in-app wallet address associated with the user</param>
        /// <returns>Response string or null if operation failed</returns>
        public static async Task<string> CreateUser(string userAddress, string username, string referralCode, string referrerCode, string inAppWallet)
        {
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(username))
            {
                Debug.LogError("Invalid parameters: userAddress and username must not be empty");
                return null;
            }

            string path = "/users";
            var payload = new CreateUserRequestBody
            {
                userAddress = userAddress,
                username = username,
                referralCode = referralCode,
                referrerCode = referrerCode,
                inAppWallet = inAppWallet
            };

            string json = JsonUtility.ToJson(payload);
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendPostAsync - Request URL: {url}");
            Debug.Log($"SendPostAsync - Request Body: {json}");
            Debug.Log($"SendPostAsync - JSON Length: {json.Length}");

            // Ensure we have valid JSON
            if (string.IsNullOrEmpty(json) || json == "{}")
            {
                Debug.LogError("Generated JSON is empty or invalid");
                return null;
            }

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            // For WebGL compatibility
            request.timeout = 30;

            Debug.Log($"Request headers set. Content-Length should be: {bodyRaw.Length}");

            var operation = request.SendWebRequest();
            await operation;

            // Enhanced error logging
            Debug.Log($"Request completed. Result: {request.result}");
            Debug.Log($"Response Code: {request.responseCode}");
            Debug.Log($"Upload bytes sent: {request.uploadedBytes}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"CREATED USER SUCCESSFULLY: {path}");
                PlayerController.canMove = true;
                PlayerController.canInteract = true;
                await InventoryApi.RegisterToGameShift(userAddress);

                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"POST error on {path}: {request.error}");
                Debug.LogError($"Response code: {request.responseCode}");
                Debug.LogError($"Response body: {request.downloadHandler.text}");

                // Log request details for debugging
                Debug.LogError($"Final request URL: {request.url}");
                Debug.LogError($"Request method: {request.method}");

                return null;
            }
        }

        /// <summary>
        /// Retrieves user data based on the provided user address
        /// </summary>
        /// <param name="userAddress">The active wallet address of the user</param>
        /// <returns>Response string or null if operation failed</returns>
        public static async Task<bool> GetUser(string userAddress)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress))
            {
                Debug.LogError("Invalid parameter: userAddress must not be empty");
                return false;
            }

            string path = $"/users/{userAddress}";

            Debug.Log($"Getting user data for address: {userAddress}");

            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("USER EXISTS WITH CONNECTED ADDRESS");
                Debug.Log($"SendGetAsync successful: {path}");
                Debug.Log(request.downloadHandler.text);

                try
                {
                    GetUserResponse response = JsonUtility.FromJson<GetUserResponse>(request.downloadHandler.text);

                    // Check if deserialization was successful
                    if (response != null)
                    {
                        UserRegistration.SetUserInfo(response.username, response.referralCode);
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Failed to deserialize user response - response is null");
                        return false;
                    }
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"JSON deserialization error: {ex.Message}");
                    Debug.LogError($"Raw response: {request.downloadHandler.text}");
                    return false;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Unexpected error during deserialization: {ex.Message}");
                    return false;
                }
            }
            else
            {
                Debug.Log("NO USER EXISTS WITH CONNECTED ADDRESS, INVOKE REGISTRATION");
                return false;
            }
        }



        /// <summary>
        /// Updates the wallet address for a user
        /// </summary>
        /// <param name="userAddress">The active wallet address of the user</param>
        /// <param name="wallet">The new wallet address to update</param>
        /// <returns>Response string or null if operation failed</returns>
        public static async Task<string> UpdateWallet(string userAddress, string wallet)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(wallet))
            {
                Debug.LogError("Invalid parameters: userAddress and wallet must not be empty");
                return null;
            }

            string path = $"/users/{userAddress}/wallet";
            var payload = new UpdateWalletRequestBody { wallet = wallet };

            string json = JsonUtility.ToJson(payload);
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendPostAsync - Request URL: {url}");
            Debug.Log($"SendPostAsync - Request Body: {json}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"UPDATE WALLET SUCCESSFULL: {path}");
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"UPDATE WALLET FAILED ON {path}: {request.error}, Response code: {request.responseCode}");
                Debug.LogError($"Response body: {request.downloadHandler.text}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves all user data for a user
        /// </summary>
        /// <param name="userAddress">The active wallet address of the user</param>
        /// <returns>Response string or null if operation failed</returns>
        public static async Task<string> GetAllUserData(string userAddress)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress))
            {
                Debug.LogError("Invalid parameter: userAddress must not be empty");
                return null;
            }

            string path = $"/users/{userAddress}/all";

            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"SendGetAsync successful: {path}");
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"GET error on {path}: {request.error}, Response code: {request.responseCode}");
                Debug.LogError($"Response body: {request.downloadHandler.text}");
                return null;
            }
        }

        /// <summary>
        /// Updates the username for a user
        /// </summary>
        /// <param name="userAddress">The active wallet address of the user</param>
        /// <param name="username">The new username to update</param>
        /// <returns>Response string or null if operation failed</returns>
        public static async Task<string> UpdateUsername(string userAddress, string username)
        {

            var userInfo = await GetUser(userAddress);
            if (userInfo == false)
            {
                await CreateUser(userAddress, username, "0x"+username, "", "");
                return null;
            }
            

            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(username))
            {
                Debug.LogError("Invalid parameters: userAddress and username must not be empty");
                return null;
            }

            string path = $"/users/{userAddress}/username";
            var payload = new UpdateUserNameRequest { username = username };

            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendPutAsync - Request URL: {url}");

            string json = JsonUtility.ToJson(payload);

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new(url, "PUT")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"UPDATE USERNAME SUCCESSFUL: {path}");
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"PUT error on {path}: {request.error}, Response code: {request.responseCode}");
                Debug.LogError($"Response body: {request.downloadHandler.text}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves referral data based on the provided referral code
        /// </summary>
        /// <param name="referralCode">The unique referral code to retrieve</param>
        /// <returns>Response string or null if operation failed</returns>
        public static async Task<string> GetReferralByCode(string referralCode)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(referralCode))
            {
                Debug.LogError("Invalid parameter: referralCode must not be empty");
                return null;
            }

            string path = $"/referrals/{referralCode}";

            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"FETCHED REFERRAL CODE: {path}");
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"GET error on {path}: {request.error}, Response code: {request.responseCode}");
                Debug.LogError($"Response body: {request.downloadHandler.text}");
                return null;
            }
        }

        /// <summary>
        /// Retrieves user data based on the provided referral code
        /// </summary>
        /// <param name="referralCode">The unique referral code to retrieve</param>
        /// <returns>Response string or null if operation failed</returns>
        public static async Task<string> GetUserByReferralCode(string referralCode)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(referralCode))
            {
                Debug.LogError("Invalid parameter: referralCode must not be empty");
                return null;
            }

            string path = $"/users/referral/{referralCode}";

            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"FETCHED USER FROM REFERRAL CODE: {path}");
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"GET error on {path}: {request.error}, Response code: {request.responseCode}");
                Debug.LogError($"Response body: {request.downloadHandler.text}");
                return null;
            }
        }


        public static async void AddUserScore(string userAddress, string questId, int score)
        {
            int existingScore = await GetUserQuestScoreByQuestId(userAddress, questId);

            if (existingScore == -1)
            {
                // Score not found, so submit a new score
                await SubmitQuestScore(userAddress, questId, score);
            }
            else
            {
                if (existingScore < score)
                {
                    // Score exists, so update it
                    await UpdateUserQuestScore(userAddress, questId, score);
                }
            }
        }


        public static async Task<int> GetUserQuestScoreByQuestId(string userAddress, string questId)
        {
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(questId))
            {
                Debug.LogError("Invalid parameters: userAddress and questId must not be empty");
                return -1;
            }

            string path = $"/users/{userAddress}/quests/{questId}/score";
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                    if (response.ContainsKey("score"))
                    {
                        return Convert.ToInt32(response["score"]);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing score response: {e}");
                }
            }
            else if (request.responseCode == 404)
            {
                return -1; // score not found
            }

            Debug.LogError($"GetUserQuestScoreByQuestId failed: {request.error}");
            return -1;
        }


        public static async Task<bool> SubmitQuestScore(string userAddress, string questId, int score)
        {
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(questId))
            {
                Debug.LogError("Invalid parameters: userAddress and questId must not be empty");
                return false;
            }

            string path = "/quests/score";
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendPostAsync - Request URL: {url}");
            Debug.Log($"Submitting quest score for user: {userAddress}, quest: {questId}, score: {score}");

            var payload = new SubmitQuestScoreRequestBody
            {
                userAddress = userAddress,
                questId = questId,
                score = score,
                username = UserRegistration.username
            };

            string json = JsonUtility.ToJson(payload);
            Debug.Log($"SendPostAsync - Request Body: {json}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"SubmitQuestScore successful: {path}");
                Debug.Log(request.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError($"SubmitQuestScore failed: {request.error}");
                return false;
            }
        }

        // GET - Get Quest Leaderboard
        public static async Task<LeaderboardResponse> GetQuestLeaderboard(string questId)
        {
            string path = $"/quests/{questId}/leaderboard";
            string url = GetEnvVars.Get("USERDB_API_URL") + path;
            Debug.Log($"[Leaderboard] Requesting URL: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Set headers
                request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));
                request.SetRequestHeader("Content-Type", "application/json");

                // Important for WebGL
                request.timeout = 30;

                var operation = request.SendWebRequest();
                await operation;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string json = request.downloadHandler.text;
                        Debug.Log($"[Leaderboard] Raw response: {json}");
                        LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);
                        return response;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[Leaderboard] JSON parse error: {e}");
                        return null;
                    }
                }
                else
                {
                    Debug.LogError($"[Leaderboard] Request failed: {request.error}");
                    Debug.LogError($"[Leaderboard] Response Code: {request.responseCode}");
                    Debug.LogError($"[Leaderboard] Response Text: {request.downloadHandler?.text}");
                    return null;
                }
            }
        }



        // GET - Get User Quest Scores
        public static async Task<bool> GetUserQuestScores(string userAddress)
        {
            if (string.IsNullOrEmpty(userAddress))
            {
                Debug.LogError("Invalid parameter: userAddress must not be empty");
                return false;
            }

            string path = $"/users/{userAddress}/quest-scores";
            Debug.Log($"Getting quest scores for user: {userAddress}");
            string url = GetEnvVars.Get("USERDB_API_URL") + path;
            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"GetUserQuestScores successful: {path}");
                Debug.Log(request.downloadHandler.text);
                // Deserialize response if needed
                // UserQuestScoresResponse response = JsonConvert.DeserializeObject<UserQuestScoresResponse>(request.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError($"GetUserQuestScores failed: {request.error}");
                return false;
            }
        }


        // PUT - Update User Quest Score
        public static async Task<bool> UpdateUserQuestScore(string userAddress, string questId, int score)
        {
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(questId))
            {
                Debug.LogError("Invalid parameters: userAddress and questId must not be empty");
                return false;
            }

            string path = $"/users/{userAddress}/quests/{questId}/score";
            Debug.Log($"Updating score for user: {userAddress}, quest: {questId}, score: {score}");
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            var payload = new UpdateQuestScoreRequestBody { score = score };
            string json = JsonUtility.ToJson(payload);
            Debug.Log($"SendPutAsync - Request Body: {json}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new(url, "PUT")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"UpdateUserQuestScore successful: {path}");
                Debug.Log(request.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError($"UpdateUserQuestScore failed: {request.error}");
                return false;
            }
        }


        // GET - Get Overall Leaderboard
        public static async Task<bool> GetOverallLeaderboard()
        {
            string path = "/leaderboard/overall";
            Debug.Log($"Getting overall leaderboard");
            string url = GetEnvVars.Get("USERDB_API_URL") + path;
            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"GetOverallLeaderboard successful: {path}");
                Debug.Log(request.downloadHandler.text);
                // Deserialize response if needed
                // OverallLeaderboardResponse response = JsonConvert.DeserializeObject<OverallLeaderboardResponse>(request.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError($"GetOverallLeaderboard failed: {request.error}");
                return false;
            }
        }

        // GET - Get User Quest Rank
        public static async Task<bool> GetUserQuestRank(string userAddress, string questId)
        {
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(questId))
            {
                Debug.LogError("Invalid parameters: userAddress and questId must not be empty");
                return false;
            }

            string path = $"/users/{userAddress}/quests/{questId}/rank";
            Debug.Log($"Getting rank for user: {userAddress}, quest: {questId}");
            string url = GetEnvVars.Get("USERDB_API_URL") + path;
            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"GetUserQuestRank successful: {path}");
                Debug.Log(request.downloadHandler.text);
                // Deserialize response if needed
                // UserRankResponse response = JsonConvert.DeserializeObject<UserRankResponse>(request.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError($"GetUserQuestRank failed: {request.error}");
                return false;
            }
        }

        public static async Task<bool> MarkQuestComplete(string userAddress, string questId)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(questId))
            {
                Debug.LogError("Invalid parameters: userAddress and questId must not be empty");
                return false;
            }

            string path = $"/users/{userAddress}/quests/{questId}/complete";
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"Marking quest complete for user: {userAddress}, quest: {questId}");
            Debug.Log($"SendPutAsync - Request URL: {url}");

            UnityWebRequest request = new UnityWebRequest(url, "PUT")
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Successfully marked quest complete: {questId}");
                Debug.Log($"Response: {request.downloadHandler.text}");
                return true;
            }
            else
            {
                Debug.LogError($"Failed to mark quest complete: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                return false;
            }
        }

        public static async Task<bool> MarkQuestInComplete(string userAddress, string questId)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(questId))
            {
                Debug.LogError("Invalid parameters: userAddress and questId must not be empty");
                return false;
            }

            string path = $"/users/{userAddress}/quests/{questId}/incomplete";
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"Marking quest complete for user: {userAddress}, quest: {questId}");
            Debug.Log($"SendPutAsync - Request URL: {url}");

            UnityWebRequest request = new UnityWebRequest(url, "PUT")
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Successfully marked quest incomplete: {questId}");
                Debug.Log($"Response: {request.downloadHandler.text}");
                return true;
            }
            else
            {
                Debug.LogError($"Failed to mark quest incomplete: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                return false;
            }
        }

        //userAddress,
        //questId,
        //completed: false,
        //completedAt: null,
        //message: 'Quest not completed'

        public class IsQuestCompletedResponse
        {
            public string userAddress;
            public string questId;
            public bool completed;
            public int completedAt;
            public string message;
        }


        public static async Task<bool> IsQuestCompleted(string userAddress, string questId)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress) || string.IsNullOrEmpty(questId))
            {
                Debug.LogError("Invalid parameters: userAddress and questId must not be empty");
                return false;
            }

            string path = $"/users/{userAddress}/quests/{questId}/completion";
            string url = GetEnvVars.Get("USERDB_API_URL") + path;
            Debug.Log($"Checking quest completion for user: {userAddress}, quest: {questId}");
            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Quest completion check successful for quest: {questId}");
                Debug.Log($"Response: {request.downloadHandler.text}");

                try
                {
                    IsQuestCompletedResponse response = JsonUtility.FromJson<IsQuestCompletedResponse>(request.downloadHandler.text);

                    if (response.completed)
                    {
                        Debug.Log($"Quest {questId} is completed");
                    }
                    else
                    {
                        Debug.Log($"Quest {questId} is not completed");
                    }

                    return response.completed;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse response: {ex.Message}");
                    return false;
                }
            }
            else
            {
                Debug.LogError($"Failed to check quest completion: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                return false;
            }
        }


        public static async Task<GetQuestCompletionsResponse> GetCompletedQuestsByUser(
            string userAddress,
            bool completedOnly = true,
            int limit = 10,
            string lastEvaluatedKey = null
            )
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(userAddress))
            {
                Debug.LogError("Invalid parameter: userAddress must not be empty");
                return null;
            }

            string path = $"/users/{userAddress}/completed-quests";

            var queryParams = new List<string>();
            if (completedOnly)
                queryParams.Add("completedOnly=true");
            if (limit != 50)
                queryParams.Add($"limit={limit}");
            if (!string.IsNullOrEmpty(lastEvaluatedKey))
                queryParams.Add($"lastEvaluatedKey={Uri.EscapeDataString(lastEvaluatedKey)}");

            if (queryParams.Count > 0)
                path += "?" + string.Join("&", queryParams);

            Debug.Log($"Getting quest completions for address: {userAddress}");
            string url = GetEnvVars.Get("USERDB_API_URL") + path;
            Debug.Log($"SendGetAsync - Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("QUEST COMPLETIONS RETRIEVED SUCCESSFULLY");
                Debug.Log($"SendGetAsync successful: {path}");
                Debug.Log(request.downloadHandler.text);

                GetQuestCompletionsResponse response = JsonUtility.FromJson<GetQuestCompletionsResponse>(request.downloadHandler.text);
                return response;
            }
            else
            {
                Debug.LogError($"Failed to get quest completions: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                return null;
            }
        }


        public static async Task<bool> ConsumeAccessCode(string accessCode)
        {
            // Validate required inputs
            if (string.IsNullOrEmpty(accessCode))
            {
                Debug.LogError("Invalid parameters: accessCode must not be empty");
                return false;
            }

            string path = "/access-codes/consume";
            var payload = new ConsumeCodeRequestBody { code = accessCode };


            string json = JsonUtility.ToJson(payload);
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendPostAsync - Request URL: {url}");
            Debug.Log($"SendPostAsync - Request Body: {json}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new(url, "POST")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Access code verified SUCCESSFULLY: {path}");
                return true;
            }
            else
            {
                Debug.Log($"POST error on {path}: {request.error}, Response code: {request.responseCode}");
                Debug.Log($"Response body: {request.downloadHandler.text}");
                return false;
            }
        }


        /// <summary>
        /// Sends a PUT request to the API
        /// </summary>
        /// <param name="path">The path to the API endpoint</param>
        /// <param name="jsonBody">The JSON body to send in the request</param>
        /// <returns>Response string or null if operation failed</returns>
        private static async Task<string> SendPutAsync(string path, string jsonBody)
        {
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"SendPutAsync - Request URL: {url}");
            Debug.Log($"SendPutAsync - Request Body: {jsonBody}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            UnityWebRequest request = new(url, "PUT")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"SendPutAsync successful: {path}");
                return request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"PUT error on {path}: {request.error}, Response code: {request.responseCode}");
                Debug.LogError($"Response body: {request.downloadHandler.text}");
                return null;
            }
        }

        public static async Task<GetUserFarmlandResponse> GetUserFarmland(string userAddress)
        {
            if (string.IsNullOrEmpty(userAddress))
            {
                Debug.LogError("Invalid parameter: userAddress must not be empty");
                return null;
            }

            string path = $"/users/{userAddress}/farmland";
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            Debug.Log($"Fetching farmland data for: {userAddress}");
            Debug.Log($"Request URL: {url}");

            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("FARMLAND DATA RETRIEVED SUCCESSFULLY");
                Debug.Log(request.downloadHandler.text);

                GetUserFarmlandResponse response = JsonUtility.FromJson<GetUserFarmlandResponse>(request.downloadHandler.text);
                return response;
            }
            else
            {
                Debug.LogError($"Failed to get farmland data: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                return null;
            }
        }

        public static async Task<bool> UpdateUserFarmland(string userAddress, List<CellData> updatedCells)
        {
            if (string.IsNullOrEmpty(userAddress) || updatedCells == null || updatedCells.Count == 0)
            {
                Debug.LogError("Invalid parameters: userAddress must not be empty and updatedCells must not be null or empty");
                return false;
            }

            string path = $"/users/{userAddress}/farmland";
            Debug.Log($"Updating farmland for user: {userAddress} with {updatedCells.Count} cells");
            string url = GetEnvVars.Get("USERDB_API_URL") + path;

            var payload = new UpdateUserFarmlandRequestBody { cells = updatedCells };
            string json = JsonUtility.ToJson(payload);
            Debug.Log($"SendPutAsync - Request Body: {json}");

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            UnityWebRequest request = new(url, "PUT")
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", GetEnvVars.Get("USERDB_API_KEY"));

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Farmland updated successfully.");
                Debug.Log(request.downloadHandler.text);
                return true;
            }
            else
            {
                Debug.LogError($"Failed to update farmland: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                return false;
            }
        }



    }

    
}