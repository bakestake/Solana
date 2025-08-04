using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Threading.Tasks;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using Solana.Unity.Wallet.Bip39;
using System.Text;
using Random = System.Random;

namespace Bakeland
{
    public class WalletConnectScript : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showAccessCodePanel;

        [Header("Chain Configuration")]
        [SerializeField] private ulong activeChainId = 80069;
        [SerializeField] private bool webglForceMetamaskExtension = false;

        [SerializeField] private GameObject backgroundPanel;

        [Header("Modal Panel")]
        [SerializeField] private GameObject modalPanel;
        [SerializeField] private Button socialsButton;
        [SerializeField] private Button guestWalletButton;
        [SerializeField] private Button accountAbstractionButton;
        [SerializeField] private Button walletConnectButton;
        [SerializeField] private Button metamaskButton;
        [SerializeField] private Button rabbyButton;
        [SerializeField] private Button phantomButton;
        [SerializeField] private Button closeModalButton;
        [SerializeField] private Button addressInfoButton;

        [Header("Connection Button")]
        [SerializeField] private Button connectWalletButton;

        [Header("Wallet Info Panel")]
        [SerializeField] private GameObject walletInfoPanel;
        [SerializeField] private TMP_Text addressText;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private GameObject disconnectPanel;
        [SerializeField] private Button disconnectButton;

        [Header("Email panel")]
        [SerializeField] private GameObject socialsPanel;
        [SerializeField] private GameObject emailInputPanel;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_Text emailLabel;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button closeEmailPanelButton;

        [Header("User Registration Panel")]
        [SerializeField] private GameObject userRegPanel;
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private TMP_InputField referrerCodeField;
        [SerializeField] private TMP_InputField referralCodeField;

        [Header("Access code panel")]
        [SerializeField] private GameObject accessCodePanel;
        [SerializeField] private TMP_InputField accessCodeField;
        [SerializeField] private Button submitAccessCodeButton;
        [SerializeField] private TMP_Text errorCodeLabel;

        [Header("Social auth buttons")]
        [SerializeField] private Button googleButton;
        [SerializeField] private Button discordButton;
        [SerializeField] private Button twitterButton;

        [Header("Password login")]
        [SerializeField] private Button emailLoginButton;

        [Serializable]
        class ChainDetails
        {
            public string Name;
            public string Symbol;
        }

        [Header("Global vars")]
        private ChainDetails _chainDetails;
        private bool _isWalletConnected = false;
        private bool _isWalletConnectInitialized = false;
        public static string connectedWalletAddress = "";

        private readonly Random _random = new Random();


        //private void Awake()
        //{
        //   InitializeUI();
        //}

        private void Start()
        {
            try
            {
                InitializeUI();
                _chainDetails = new ChainDetails()
                {
                    Name =  "Solana",
                    Symbol = "SOL"
                };
            }
            catch
            {
                _chainDetails = new ChainDetails() 
                {
                    Name = "Solana",
                    Symbol = "SOL"
                };
            }

            Web3.Instance.rpcCluster = RpcCluster.MainNet;
            
            PlayerController.canMove = false;
            PlayerController.canInteract = false;
            backgroundPanel.SetActive(true);
        }

        private async void LoginChecker(string password)
        {
            if (password.IsNullOrEmpty())
            {
                throw new ArgumentNullException("password");
            } 
            Account account = await Web3.Instance.LoginInGameWallet(password);
            connectedWalletAddress = account.PublicKey;
            await ConnectWallet();
        }

        private async void LoginCheckerSms()
        {
            var account = await Web3.Instance.LoginWalletAdapter();
            connectedWalletAddress = account.PublicKey;
            await ConnectWallet();
        }

        private async void LoginCheckerWeb3Auth(Provider provider)
        {
            var account = await Web3.Instance.LoginWeb3Auth(provider);
            connectedWalletAddress = account.PublicKey;
            await ConnectWallet();
        }

        private async void LoginCheckerWalletAdapter()
        {
            if (Web3.Instance == null) return;
            var account = await Web3.Instance.LoginWalletAdapter();
            connectedWalletAddress = account.PublicKey;
            await ConnectWallet();
        }


        private async Task<string> GenerateNewAccount(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            try
            {
                string mnemonic =  new Mnemonic(WordList.English, WordCount.TwentyFour).ToString();
                await Web3.Instance.CreateAccount(mnemonic, password);
                return mnemonic;
            }
            catch (Exception ex)
            {
                emailField.gameObject.SetActive(true);
                emailField.text = "Failed to create account, retry";
                throw new Exception(ex.Message);
            }
        }

        private void Update()
        {
            // band-aid solution
            // force disable player interaction because other out-of-order elements are manipulating this value
            if (backgroundPanel.activeSelf)
            {
                PlayerController.canMove = false;
                PlayerController.canInteract = false;
            }
        }

        private void OnDestroy()
        {
            // Ensure we clean up resources when the object is destroyed
            CleanupWalletConnectResources();
        }

        private void InitializeUI()
        {
            // Initial state
            modalPanel.SetActive(false);
            walletInfoPanel.SetActive(false);
            emailInputPanel.SetActive(false);
            disconnectPanel.SetActive(false);
            // backgroundPanel.SetActive(false);

            // Connect button
            if (connectWalletButton != null)
            {
                connectWalletButton.onClick.RemoveAllListeners();
                connectWalletButton.onClick.AddListener(OpenWalletModal);
            }

            // Modal options
            if (emailLoginButton != null)
            {
                emailLoginButton.onClick.RemoveAllListeners();
                emailLoginButton.onClick.AddListener(openEmailPanel);
            }

            if (guestWalletButton != null)
            {
                guestWalletButton.onClick.RemoveAllListeners();
                guestWalletButton.onClick.AddListener(() => ConnectGuestWallet());
                guestWalletButton.onClick.AddListener(() => socialsPanel.SetActive(false));
            }

            if (metamaskButton != null)
            {
                metamaskButton.onClick.RemoveAllListeners();
                metamaskButton.onClick.AddListener(() => LoginCheckerWalletAdapter());
                walletConnectButton.onClick.AddListener(() => socialsPanel.SetActive(false));
            }

            if (phantomButton != null)
            {
                phantomButton.onClick.RemoveAllListeners();
                phantomButton.onClick.AddListener(() => LoginCheckerWalletAdapter());
                walletConnectButton.onClick.AddListener(() => socialsPanel.SetActive(false));
            }

            if (closeModalButton != null)
            {
                closeModalButton.onClick.RemoveAllListeners();
                closeModalButton.onClick.AddListener(CloseWalletModal);
            }

            // Disconnect button
            if (disconnectButton != null)
            {
                disconnectButton.onClick.RemoveAllListeners();
                disconnectButton.onClick.AddListener(DisconnectWallet);
            }

            if (closeEmailPanelButton != null)
            {
                closeEmailPanelButton.onClick.RemoveAllListeners();
                closeEmailPanelButton.onClick.AddListener(closeEmailPanel);
            }

            if (submitAccessCodeButton != null)
            {
                submitAccessCodeButton.onClick.RemoveAllListeners();
                submitAccessCodeButton.onClick.AddListener(async () => await VerifyAccessCode());
            }

            if (socialsButton != null)
            {
                socialsButton.onClick.RemoveAllListeners();
                socialsButton.onClick.AddListener(() => socialsPanel.SetActive(true));
            }

            if (googleButton != null)
            {
                googleButton.onClick.RemoveAllListeners();
                googleButton.onClick.AddListener(() => LoginCheckerWeb3Auth(Provider.GOOGLE));
            }

            if (discordButton != null)
            {
                discordButton.onClick.RemoveAllListeners();
                discordButton.onClick.AddListener(() => LoginCheckerWeb3Auth(Provider.DISCORD));
            }

            if (twitterButton != null)
            {
                twitterButton.onClick.RemoveAllListeners();
                twitterButton.onClick.AddListener(() => LoginCheckerWeb3Auth(Provider.TWITTER));
            }

            if (addressInfoButton != null)
            {
                addressInfoButton.onClick.RemoveAllListeners();
                addressInfoButton.onClick.AddListener(() => CopyAddressToClipboard());
            }

            if (submitButton != null)
            {
                submitButton.onClick.RemoveAllListeners();
                submitButton.onClick.AddListener(() => LoginChecker(emailField.text));
            }

            //CheckeExisitingConnection();
        }

        private async Task VerifyAccessCode()
        {
            try
            {
                if (accessCodeField.text == $"<color=#{"FF0000"}>{"Invalid code"}</color>")
                {
                    accessCodeField.text = string.Empty;
                }
                if (accessCodePanel != null)
                {
                    string accessCode = accessCodeField.text;
                    if (string.IsNullOrEmpty(accessCode))
                    {
                        throw new Exception("No Access code entered");
                    }

                    LoadingWheel.instance.EnableLoading();
                    if (!await UserAndReferralApi.ConsumeAccessCode(accessCode))
                    {
                        errorCodeLabel.text = $"<color=#{"FF0000"}>{"Invalid code"}</color>";
                    }
                    else
                    {
                        CloseAccessCodePanel();
                        // OpenUserRegPanel();

                        backgroundPanel.SetActive(false);
                    }
                    LoadingWheel.instance.DisableLoading();

                    if (PlayerController.Instance.username == "" && !PlayerPrefs.HasKey("first"))
                    {
                        CharacterSelector.instance.OpenCharacterSelection();
                    }
                    backgroundPanel.SetActive(false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to verify access code : {ex.Message}");
            }

        }

        private void OpenWalletModal()
        {
            Debug.Log("OpenWalletModal");
            PlayerController.canMove = false;
            PlayerController.canInteract = false;
            modalPanel.SetActive(true);
            backgroundPanel.SetActive(true);
        }

        private void closeEmailPanel()
        {
            emailInputPanel.SetActive(false);
        }

        private void openEmailPanel()
        {
            emailInputPanel.SetActive(true);
            modalPanel.SetActive(false);
        }

        private void CloseWalletModal()
        {
            modalPanel.SetActive(false);
        }

        private void ShowWalletInfo()
        {
            connectWalletButton.gameObject.SetActive(false);
            backgroundPanel.SetActive(false);
            walletInfoPanel.SetActive(true);
            disconnectPanel.SetActive(true);
        }

        private async void UpdateWalletInfo()
        {
            var wallet = connectedWalletAddress;
            if (wallet == null)
            {
                Debug.LogWarning("Cannot update wallet info: No active wallet found");
                return;
            }

            try
            {
                // Get and display address
                addressText.text = $"{FormatAddress(wallet)}";

                // Get and display balance
                var balance = await Web3.Instance.WalletBase.GetBalance();
                balanceText.text = $"{balance/1e9} {_chainDetails.Symbol}";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating wallet info: {e.Message}");
            }
        }

        private string FormatAddress(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length < 10)
                return address;

            // Format as: 0x1234...5678
            return $"{address.Substring(0, 6)}...{address.Substring(address.Length - 4)}";
        }

        private async void DisconnectWallet()
        {
            try
            {
                var wallet = Web3.Instance.WalletBase.Account.PublicKey;
                if (wallet != null)
                {
             
                    Web3.Instance.WalletBase.Logout();

                    // Clean up WalletConnect resources if needed
                    CleanupWalletConnectResources();
                }

                _isWalletConnected = false;
                connectWalletButton.gameObject.SetActive(true);
                backgroundPanel.SetActive(true);
                walletInfoPanel.SetActive(false);
                PlayerController.canMove = false;
                PlayerController.canInteract = false;

                LoadingWheel.instance.EnableLoading();
                // destroy all objects in DontDestroyOnLoad
                GameObject[] dontDestroyObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in dontDestroyObjects)
                {
                    if (obj.scene.name == "DontDestroyOnLoad")
                    {
                        Destroy(obj);
                    }
                }
                // go back to menu
                var loadScene = await SceneManager.LoadSceneAsync(0);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during wallet disconnection: {e.Message}");
            }
        }

        private void CleanupWalletConnectResources()
        {
            // This ensures the WalletConnect resources are cleaned up properly
            // to prevent the ObjectDisposedException
            if (_isWalletConnectInitialized)
            {
                try
                {
                    var walletConnectModal = GameObject.FindObjectOfType<WalletConnectUnity.Modal.WalletConnectModal>();
                    if (walletConnectModal != null)
                    {
                        // Manually clean up WalletConnect resources if possible
                        Destroy(walletConnectModal.gameObject);
                    }
                    _isWalletConnectInitialized = false;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"WalletConnect cleanup warning: {e.Message}");
                }
            }
        }

        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65�90 / 97�122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.

            // char is a single Unicode character
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }


        private async void ConnectGuestWallet()
        {
            LoadingWheel.instance.EnableLoading();

            try
            {
                string mnemonic = new Mnemonic(WordList.English, WordCount.TwentyFour).ToString();
                string password = RandomString(5);
                await Web3.Instance.CreateAccount(mnemonic, password);
                connectedWalletAddress = Web3.Account.PublicKey;
                await ConnectWallet();
            }
            catch (Exception e)
            {
                Debug.LogError($"Guest wallet connection error: {e.Message}");
                HandleConnectionFailure();
            }

            LoadingWheel.instance.DisableLoading();
        }


        private async Task ConnectWallet()
        {
            // try
            {
                _isWalletConnected = true;

                CloseWalletModal();
                UpdateWalletInfo();
                ShowWalletInfo();

                if (connectedWalletAddress == null || connectedWalletAddress.Length == 0)
                {
                    connectWalletButton.gameObject.SetActive(true);
                    backgroundPanel.SetActive(true);
                }

                bool userExists = await UserAndReferralApi.GetUser(connectedWalletAddress);

                /// if user has an account associated with wallet then do not open reg
                if (userExists == false)
                {
                    CharacterSelector.instance.OpenCharacterSelection();
                    FindObjectOfType<PlayerCurrency>().SetLocalGoldBalance(int.Parse("0"));
                }
                else
                {
                    PlayerController.Instance.SetUsername(UserRegistration.username);

                    var inventoryResponse = await InventoryApi.GetUserInventory(connectedWalletAddress);
                    LocalGameManager.Instance.inventoryContainer.LoadInventory(inventoryResponse);

                    // load gold balance
                    InventoryApi.goldBalance = await InventoryApi.GetGoldBalance(connectedWalletAddress);
                    FindObjectOfType<PlayerCurrency>().SetLocalGoldBalance(int.Parse(InventoryApi.goldBalance));

                    // load completed quests
                    await QuestManager.Instance.API_DisableCompletedQuests();

                    backgroundPanel.SetActive(false);
                    PlayerController.canMove = true;
                    PlayerController.canInteract = true;
                }
                
            }

            LoadingWheel.instance.DisableLoading();
            Debug.Log($"{name}:connect wallet end");
        }

        private void OpenAccessCodePanel()
        {
            try
            {
                accessCodePanel.SetActive(true);
                accessCodeField.gameObject.SetActive(true);
                submitAccessCodeButton.gameObject.SetActive(true);
                PlayerController.canMove = false;
                PlayerController.canInteract = false;

            }
            catch (Exception e)
            {
                Debug.Log("Failed to open access code panel");
                throw new Exception("Failed to open access code panel" + e.Message);
            }
        }

        private void CloseAccessCodePanel()
        {
            try
            {
                accessCodePanel.SetActive(false);
                accessCodeField.gameObject.SetActive(false);
                submitAccessCodeButton.gameObject.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to close access code panel");
                throw new Exception("failed to open access code panel" + e.Message);
            }
        }

        private void HandleConnectionFailure()
        {
            // Clean up the UI
            if (emailInputPanel.activeSelf)
            {
                closeEmailPanel();
            }

            CloseWalletModal();

            // Make sure the connect button is visible again
            connectWalletButton.gameObject.SetActive(true);
            backgroundPanel.SetActive(true);
            walletInfoPanel.SetActive(false);
        }

        private void OpenUserRegPanel()
        {
            try
            {
                userRegPanel.gameObject.SetActive(true);
                usernameField.gameObject.SetActive(true);
                referralCodeField.gameObject.SetActive(true);
                referrerCodeField.gameObject.SetActive(true);
                PlayerController.canMove = false;
                PlayerController.canInteract = false;
            }
            catch (Exception e)
            {
                Debug.Log("Failed to open user Reg panel");
                throw new Exception("failed to open user reg panel" + e.Message);
            }
        }

        // For use in app shutdown or scene transitions
        public async void CleanupBeforeExit()
        {
            if (_isWalletConnected)
            {
                await DisconnectWalletAsync();
            }
            CleanupWalletConnectResources();
        }

        // Async version for clean shutdown
        private async Task DisconnectWalletAsync()
        {
            try
            {
                // TODO: disconnect solana
                
                _isWalletConnected = false;

                connectedWalletAddress = "";
                UserRegistration.username = "";
                UserRegistration.referralCode = "";
                InventoryApi.goldBalance = "0";
                LocalGameManager.Instance.inventoryContainer.LoadInventory(new InventoryResponse());
                PlayerController.Instance.SetUsername("");
                PlayerController.canMove = false;
                PlayerController.canInteract = false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during async wallet disconnection: {e.Message}");
            }
        }

        public void CopyAddressToClipboard()
        {
            GUIUtility.systemCopyBuffer = connectedWalletAddress;
            Debug.Log(GUIUtility.systemCopyBuffer);
        }
    }
}