//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Nethereum.Hex.HexTypes;
//using Nethereum.RPC.HostWallet;
//using Newtonsoft.Json;
//using Org.BouncyCastle.Bcpg;
//using Thirdweb;
//using Thirdweb.Unity;
//using Thirdweb.Unity.Examples;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//[System.Serializable]
//public class ChainInfo
//{
//    public ulong chainId;
//    public Sprite chainSprite;
//    public string chainName;
//}

//public class ThirwebWalletScript : MonoBehaviour
//{
//    [field: SerializeField, Header("BakelandWallet")]
//    private BakelandWalletInteraction.UaserWalletInteractions userWallet;

//    [field: SerializeField, Header("Wallet")]
//    private GameObject walletObject;

//    [field: SerializeField, Header("ButtonConnected")]
//    private GameObject ButtonConnected;

//    [field: SerializeField, Header("Connected Dropdown")]
//    private GameObject dropdown;

//    [field: SerializeField, Header("Wallet Options")]
//    private ulong ActiveChainId = 80084;

//    //[field: SerializeField]
//    private bool WebglForceMetamaskExtension = false;

//    [field: SerializeField, Header("Connect Wallet")]
//    private GameObject ConnectWalletPanel;

//    [field: SerializeField]
//    private Button PrivateKeyWalletButton;

//    [field: SerializeField]
//    private Button EcosystemWalletButton;

//    [field: SerializeField]
//    private Button WalletConnectButton;

//    [field: SerializeField, Header("Wallet Panels")]
//    private List<WalletPanelUI> WalletPanels;

//    [field: SerializeField, Header("Connected Address")]
//    private TextMeshProUGUI[] addressText;

//    [field: SerializeField, Header("Chain Sprite")]
//    private Image[] chainSprite;

//    [field: SerializeField, Header("Connected Address")]
//    private TextMeshProUGUI[] chainName;

//    [field: SerializeField, Header("Balance")]
//    private TextMeshProUGUI[] balanceText;

//    [field: SerializeField, Header("ChainImages")]
//    private List<ChainInfo> chainInfo;

//    [field: SerializeField, Header("Switch Network Panel")]
//    private GameObject switchNetworkPanel;

//    private ThirdwebChainData _chainDetails;

//    private string FactoryAddressBera = "0x12ED8C1a291e0F0BFDcD02ff2635c4b95D14bE50";
//    private string FactoryAddressOtherChains = "0x9ef75aCe60Cb2A14bD05067C20D7e7009e958116";
//    private int currentChainIndex = 0;



//    private void Awake()
//    {
//        InitializePanels();
//    }

//    private async void Start()
//    {
//        try
//        {
//            _chainDetails = await Utils.GetChainMetadata(client: ThirdwebManager.Instance.Client, chainId: ActiveChainId);
//        }
//        catch
//        {
//            _chainDetails = new ThirdwebChainData()
//            {
//                NativeCurrency = new ThirdwebChainNativeCurrency()
//                {
//                    Decimals = 18,
//                    Name = "ETH",
//                    Symbol = "ETH"
//                }
//            };
//        }
//    }

//    private void InitializePanels()
//    {
//        CloseAllPanels();

//        ConnectWalletPanel.SetActive(true);
//        WalletConnectButton.gameObject.SetActive(true);

//        PrivateKeyWalletButton.onClick.RemoveAllListeners();
//        PrivateKeyWalletButton.onClick.AddListener(() =>
//        {
//            currentChainIndex = 0;
//            ActiveChainId = chainInfo[currentChainIndex].chainId;
//            var options = GetWalletOptions(WalletProvider.PrivateKeyWallet);
//            options.ChainId = chainInfo[currentChainIndex].chainId;
//            ConnectWallet(options);
//        });

//        EcosystemWalletButton.onClick.RemoveAllListeners();
//        EcosystemWalletButton.onClick.AddListener(() => InitializeEcosystemWalletPanel());

//        WalletConnectButton.onClick.RemoveAllListeners();
//        WalletConnectButton.onClick.AddListener(() =>
//        {
//            currentChainIndex = 0;
//            ActiveChainId = chainInfo[currentChainIndex].chainId;
//            WalletConnect();
//            //var options = GetWalletOptions(WalletProvider.WalletConnectWallet);
//            //options.ChainId = chainInfo[0].chainId;
//            //ConnectWallet(options);
//        });

//    }

//    private void WalletConnect()
//    {
//        if (WalletPanels[1].Panel != null)
//        {
//            WalletConnectButton.gameObject.SetActive(false);
//            WalletPanels[1].Panel.SetActive(true);
//        }
//    }

//    public void ConnectMetaMaskWallet()
//    {
//        currentChainIndex = 0;
//        WebglForceMetamaskExtension = true;
//        var options = GetWalletOptions(WalletProvider.WalletConnectWallet);
//        options.ChainId = chainInfo[currentChainIndex].chainId;
//        ConnectWallet(options);
//    }

//    public void ConnectToSmartWallet()
//    {
//        currentChainIndex = 0;
//        ActiveChainId = chainInfo[currentChainIndex].chainId;
//        var options = GetWalletOptions(WalletProvider.SmartWallet);
//        options.ChainId = chainInfo[currentChainIndex].chainId;
//        ConnectWallet(options);
//    }

//    public void ConnectOtherWallet()
//    {
//        WebglForceMetamaskExtension = false;
//        var options = GetWalletOptions(WalletProvider.WalletConnectWallet);
//        //options.ChainId = chainInfo[0].chainId;
//        ConnectWallet(options);
//    }

//    private async void ConnectWallet(WalletOptions options)
//    {

//        // Connect the wallet

//        var internalWalletProvider = options.Provider == WalletProvider.MetaMaskWallet ? WalletProvider.WalletConnectWallet : options.Provider;
//        var currentPanel = WalletPanels.Find(panel => panel.Identifier == internalWalletProvider.ToString());

//        //Log(currentPanel.LogText, $"Connecting...");

//        var wallet = await ThirdwebManager.Instance.ConnectWallet(options);
//        // Initialize the wallet panel

//        CloseAllPanels();

//        // If wallet is not null then wallet is connected:
//        var address = await wallet.GetAddress();
//        if (address != null)
//        {
//            InitializeBakelandWallet(address, ActiveChainId);
//            switchNetworkPanel.SetActive(false);
//            LoadingWheel.instance.DisableLoading();
//        }

//        // Setup actions
//        var balance = await wallet.GetBalance(chainId: ActiveChainId);
//        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);

//        for (int i = 0; i < balanceText.Length; i++)
//        {
//            balanceText[i].text = $"{balanceEth}{_chainDetails.NativeCurrency.Symbol}";
//        }

//        if (currentPanel == null || currentPanel.Panel == null) return;
//        ClearLog(currentPanel.LogText);
//        currentPanel.Panel.SetActive(true);

//        currentPanel.BackButton.onClick.RemoveAllListeners();
//        currentPanel.BackButton.onClick.AddListener(InitializePanels);

//        currentPanel.NextButton.onClick.RemoveAllListeners();
//        currentPanel.NextButton.onClick.AddListener(InitializeContractsPanel);

//        currentPanel.Action1Button.onClick.RemoveAllListeners();
//        currentPanel.Action1Button.onClick.AddListener(async () =>
//        {
//            var address = await wallet.GetAddress();
//            Log(currentPanel.LogText, $"Address: {address}");
//        });

//        currentPanel.Action2Button.onClick.RemoveAllListeners();
//        currentPanel.Action2Button.onClick.AddListener(async () =>
//        {
//            var message = "Hello World!";
//            var signature = await wallet.PersonalSign(message);
//            Log(currentPanel.LogText, $"Signature: {signature}");
//        });

//        currentPanel.Action3Button.onClick.RemoveAllListeners();
//        currentPanel.Action3Button.onClick.AddListener(async () =>
//        {
//            LoadingLog(currentPanel.LogText);
//            var balance = await wallet.GetBalance(chainId: ActiveChainId);
//            var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
//            Log(currentPanel.LogText, $"Balance: {balanceEth} {_chainDetails.NativeCurrency.Symbol}");
//        });
//    }

//    private WalletOptions GetWalletOptions(WalletProvider provider)
//    {
//        switch (provider)
//        {
//            case WalletProvider.PrivateKeyWallet:
//                return new WalletOptions(provider: WalletProvider.PrivateKeyWallet, chainId: ActiveChainId);
//            case WalletProvider.EcosystemWallet:
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Google);
//                return new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//            case WalletProvider.WalletConnectWallet:
//                var externalWalletProvider =
//                    Application.platform == RuntimePlatform.WebGLPlayer && WebglForceMetamaskExtension ? WalletProvider.MetaMaskWallet : WalletProvider.WalletConnectWallet;
//                return new WalletOptions(provider: externalWalletProvider, chainId: ActiveChainId);
//            case WalletProvider.SmartWallet:
//                string FactoryAddress = currentChainIndex != 0 ? FactoryAddressOtherChains : FactoryAddressBera;
//                ActiveChainId = chainInfo[currentChainIndex].chainId;
//                var smartWalletOptions = new SmartWalletOptions(sponsorGas: true, factoryAddress: FactoryAddress);
//                return new WalletOptions(provider: WalletProvider.SmartWallet, ActiveChainId, smartWalletOptions: smartWalletOptions);
//            default:
//                throw new System.NotImplementedException("Wallet provider not implemented for this example.");
//        }
//    }

//    public void SwitchChainId(int index)
//    {
//        currentChainIndex = index;
//        if (ActiveChainId == chainInfo[currentChainIndex].chainId) return;

//        Debug.Log(ThirdwebManager.Instance.ActiveWallet.AccountType.ToString());
//        if (string.Equals(ThirdwebManager.Instance.ActiveWallet.AccountType.ToString(), "PrivateKeyAccount"))
//        {
//            LoadingWheel.instance.EnableLoading();
//            ActiveChainId = chainInfo[currentChainIndex].chainId;
//            var PrivateWalletoptions = GetWalletOptions(WalletProvider.PrivateKeyWallet);
//            //var walletOptions = new WalletOptions(WalletProvider.WalletConnectWallet, ActiveChainId);
//            ConnectWallet(PrivateWalletoptions);
//        }
//        else if (ThirdwebManager.Instance.ActiveWallet.AccountType.ToString() == "SmartAccount")
//        {
//            LoadingWheel.instance.EnableLoading();
//            ActiveChainId = chainInfo[currentChainIndex].chainId;
//            var options = GetWalletOptions(WalletProvider.SmartWallet);
//            options.ChainId = chainInfo[currentChainIndex].chainId;
//            ConnectWallet(options);
//        }
//        else
//        {
//            LoadingWheel.instance.EnableLoading();
//            ActiveChainId = chainInfo[currentChainIndex].chainId;
//            var options = GetWalletOptions(WalletProvider.WalletConnectWallet);
//            //var walletOptions = new WalletOptions(WalletProvider.WalletConnectWallet, ActiveChainId);
//            ConnectWallet(options);
//        }

//        if (dropdown != null)
//        {
//            dropdown.SetActive(false);
//        }
//    }

//    private void InitializeEcosystemWalletPanel()
//    {
//        var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Authentication");

//        //CloseAllPanels();

//        ClearLog(panel.LogText);
//        panel.Panel.SetActive(true);

//        panel.Action1Button.onClick.RemoveAllListeners();
//        panel.Action1Button.onClick.AddListener(() =>
//        {
//            try
//            {
//                Log(panel.LogText, "Authenticating...");
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Google);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        panel.Action2Button.onClick.RemoveAllListeners();
//        panel.Action2Button.onClick.AddListener(() =>
//        {
//            try
//            {
//                Log(panel.LogText, "Authenticating...");
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Apple);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        panel.Action3Button.onClick.RemoveAllListeners();
//        panel.Action3Button.onClick.AddListener(() =>
//        {
//            try
//            {
//                Log(panel.LogText, "Authenticating...");
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Discord);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        panel.InputFieldSubmitButton.onClick.RemoveAllListeners();
//        panel.InputFieldSubmitButton.onClick.AddListener(() =>
//        {
//            try
//            {
//                var email = panel.InputField.text;
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", email: email);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        // Email
//        panel.Action1Button.onClick.RemoveAllListeners();
//        panel.Action1Button.onClick.AddListener(() =>
//        {
//            //InitializeEcosystemWalletPanel_Email();
//            var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Email");

//            ClearLog(panel.LogText);
//            panel.Panel.SetActive(true);

//            panel.InputFieldSubmitButton.onClick.RemoveAllListeners();
//            panel.InputFieldSubmitButton.onClick.AddListener(() =>
//            {
//                try
//                {
//                    var email = panel.InputField.text;
//                    var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", email: email);
//                    var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                    ConnectWallet(options);
//                }
//                catch (System.Exception e)
//                {
//                    Log(panel.LogText, e.Message);
//                }
//            });
//        });
//    }

//    private void InitializeEcosystemWalletPanel_Email()
//    {
//        var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Email");

//        //CloseAllPanels();

//        ClearLog(panel.LogText);
//        panel.Panel.SetActive(true);

//        panel.BackButton.onClick.RemoveAllListeners();
//        panel.BackButton.onClick.AddListener(InitializeEcosystemWalletPanel);

//        panel.InputFieldSubmitButton.onClick.RemoveAllListeners();
//        panel.InputFieldSubmitButton.onClick.AddListener(() =>
//        {
//            try
//            {
//                var email = panel.InputField.text;
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", email: email);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });
//    }

//    private void InitializeEcosystemWalletPanel_Phone()
//    {
//        var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Phone");

//        //CloseAllPanels();

//        ClearLog(panel.LogText);
//        panel.Panel.SetActive(true);

//        panel.BackButton.onClick.RemoveAllListeners();
//        panel.BackButton.onClick.AddListener(InitializeEcosystemWalletPanel);

//        panel.InputFieldSubmitButton.onClick.RemoveAllListeners();
//        panel.InputFieldSubmitButton.onClick.AddListener(() =>
//        {
//            try
//            {
//                var phone = panel.InputField.text;
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", phoneNumber: phone);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });
//    }

//    private void InitializeEcosystemWalletPanel_Socials()
//    {
//        var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "EcosystemWallet_Socials");

//        //CloseAllPanels();

//        ClearLog(panel.LogText);
//        panel.Panel.SetActive(true);

//        if (panel.BackButton != null)
//        {
//            panel.BackButton.onClick.RemoveAllListeners();
//            panel.BackButton.onClick.AddListener(InitializeEcosystemWalletPanel);
//        }

//        // socials action 1 is google, 2 is apple 3 is discord

//        panel.Action1Button.onClick.RemoveAllListeners();
//        panel.Action1Button.onClick.AddListener(() =>
//        {
//            try
//            {
//                Log(panel.LogText, "Authenticating...");
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Google);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        panel.Action2Button.onClick.RemoveAllListeners();
//        panel.Action2Button.onClick.AddListener(() =>
//        {
//            try
//            {
//                Log(panel.LogText, "Authenticating...");
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Apple);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        panel.Action3Button.onClick.RemoveAllListeners();
//        panel.Action3Button.onClick.AddListener(() =>
//        {
//            try
//            {
//                Log(panel.LogText, "Authenticating...");
//                var ecosystemWalletOptions = new EcosystemWalletOptions(ecosystemId: "ecosystem.the-bonfire", authprovider: AuthProvider.Discord);
//                var options = new WalletOptions(provider: WalletProvider.EcosystemWallet, chainId: ActiveChainId, ecosystemWalletOptions: ecosystemWalletOptions);
//                ConnectWallet(options);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });
//    }

//    private void InitializeContractsPanel()
//    {
//        var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "Contracts");

//        CloseAllPanels();

//        ClearLog(panel.LogText);
//        panel.Panel.SetActive(true);

//        panel.BackButton.onClick.RemoveAllListeners();
//        panel.BackButton.onClick.AddListener(InitializePanels);

//        panel.NextButton.onClick.RemoveAllListeners();
//        panel.NextButton.onClick.AddListener(InitializeAccountAbstractionPanel);

//        // Get NFT
//        panel.Action1Button.onClick.RemoveAllListeners();
//        panel.Action1Button.onClick.AddListener(async () =>
//        {
//            try
//            {
//                LoadingLog(panel.LogText);
//                var dropErc1155Contract = await ThirdwebManager.Instance.GetContract(address: "0x94894F65d93eb124839C667Fc04F97723e5C4544", chainId: ActiveChainId);
//                var nft = await dropErc1155Contract.ERC1155_GetNFT(tokenId: 1);
//                Log(panel.LogText, $"NFT: {JsonConvert.SerializeObject(nft.Metadata)}");
//                var sprite = await nft.GetNFTSprite(client: ThirdwebManager.Instance.Client);
//                // spawn image for 3s
//                var image = new GameObject("NFT Image", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
//                image.transform.SetParent(panel.Panel.transform, false);
//                image.GetComponent<Image>().sprite = sprite;
//                Destroy(image, 3f);
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        // Call contract
//        panel.Action2Button.onClick.RemoveAllListeners();
//        panel.Action2Button.onClick.AddListener(async () =>
//        {
//            try
//            {
//                LoadingLog(panel.LogText);
//                var contract = await ThirdwebManager.Instance.GetContract(address: "0x6A7a26c9a595E6893C255C9dF0b593e77518e0c3", chainId: ActiveChainId);
//                var result = await contract.ERC1155_URI(tokenId: 1);
//                Log(panel.LogText, $"Result (uri): {result}");
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        // Get ERC20 Balance
//        panel.Action3Button.onClick.RemoveAllListeners();
//        panel.Action3Button.onClick.AddListener(async () =>
//        {
//            try
//            {
//                LoadingLog(panel.LogText);
//                var dropErc20Contract = await ThirdwebManager.Instance.GetContract(address: "0xEBB8a39D865465F289fa349A67B3391d8f910da9", chainId: ActiveChainId);
//                var symbol = await dropErc20Contract.ERC20_Symbol();
//                var balance = await dropErc20Contract.ERC20_BalanceOf(ownerAddress: await ThirdwebManager.Instance.GetActiveWallet().GetAddress());
//                var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 0, addCommas: false);
//                Log(panel.LogText, $"Balance: {balanceEth} {symbol}");
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });
//    }

//    private async void InitializeAccountAbstractionPanel()
//    {
//        var currentWallet = ThirdwebManager.Instance.GetActiveWallet();
//        var smartWallet = await ThirdwebManager.Instance.UpgradeToSmartWallet(personalWallet: currentWallet, chainId: ActiveChainId, smartWalletOptions: new SmartWalletOptions(sponsorGas: true));

//        var panel = WalletPanels.Find(walletPanel => walletPanel.Identifier == "AccountAbstraction");

//        CloseAllPanels();

//        ClearLog(panel.LogText);
//        panel.Panel.SetActive(true);

//        panel.BackButton.onClick.RemoveAllListeners();
//        panel.BackButton.onClick.AddListener(InitializePanels);

//        // Personal Sign (1271)
//        panel.Action1Button.onClick.RemoveAllListeners();
//        panel.Action1Button.onClick.AddListener(async () =>
//        {
//            try
//            {
//                if (!await smartWallet.IsDeployed())
//                {
//                    Log(panel.LogText, "Account not deployed yet, deploying before signing...");
//                }
//                var message = "Hello, World!";
//                var signature = await smartWallet.PersonalSign(message);
//                Log(panel.LogText, $"Signature: {signature}");
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        // Create Session Key
//        panel.Action2Button.onClick.RemoveAllListeners();
//        panel.Action2Button.onClick.AddListener(async () =>
//        {
//            try
//            {
//                Log(panel.LogText, "Granting Session Key...");
//                var randomWallet = await PrivateKeyWallet.Generate(ThirdwebManager.Instance.Client);
//                var randomWalletAddress = await randomWallet.GetAddress();
//                var timeTomorrow = Utils.GetUnixTimeStampNow() + 60 * 60 * 24;
//                var sessionKey = await smartWallet.CreateSessionKey(
//                    signerAddress: randomWalletAddress,
//                    approvedTargets: new List<string> { Constants.ADDRESS_ZERO },
//                    nativeTokenLimitPerTransactionInWei: "0",
//                    permissionStartTimestamp: "0",
//                    permissionEndTimestamp: timeTomorrow.ToString(),
//                    reqValidityStartTimestamp: "0",
//                    reqValidityEndTimestamp: timeTomorrow.ToString()
//                );
//                Log(panel.LogText, $"Session Key Created for {randomWalletAddress}: {sessionKey.TransactionHash}");
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });

//        // Get Active Signers
//        panel.Action3Button.onClick.RemoveAllListeners();
//        panel.Action3Button.onClick.AddListener(async () =>
//        {
//            try
//            {
//                LoadingLog(panel.LogText);
//                var activeSigners = await smartWallet.GetAllActiveSigners();
//                Log(panel.LogText, $"Active Signers: {JsonConvert.SerializeObject(activeSigners)}");
//            }
//            catch (System.Exception e)
//            {
//                Log(panel.LogText, e.Message);
//            }
//        });
//    }

//    private void CloseAllPanels()
//    {
//        WalletConnectButton.gameObject.SetActive(false);
//        ConnectWalletPanel.SetActive(false);
//        if (WalletPanels.Count > 0)
//        {
//            foreach (var walletPanel in WalletPanels)
//            {
//                if (walletPanel.Panel != null) walletPanel.Panel.SetActive(false);
//            }
//        }
//    }

//    private void ClearLog(TMP_Text logText)
//    {
//        logText.text = string.Empty;
//    }

//    private void Log(TMP_Text logText, string message)
//    {
//        logText.text = message;
//        ThirdwebDebug.Log(message);
//    }

//    private void LoadingLog(TMP_Text logText)
//    {
//        logText.text = "Loading...";
//    }

//    private Sprite GetChainSprite(ulong id)
//    {
//        return chainInfo.FirstOrDefault(c => c.chainId == id)?.chainSprite;
//    }

//    private string GetChainName(ulong id)
//    {
//        return chainInfo.FirstOrDefault(c => c.chainId == id)?.chainName;
//    }

//    private void InitializeBakelandWallet(string address, ulong chainId)
//    {
//        for (int i = 0; i < addressText.Length; i++)
//        {
//            addressText[i].text = address.Substring(0, 4) + "..." + address.Substring(address.Length - 3);//address.ToString();
//        }
//        for (int j = 0; j < chainSprite.Length; j++)
//        {
//            chainSprite[j].sprite = GetChainSprite(ActiveChainId);
//        }
//        for (int k = 0; k < chainName.Length; k++)
//        {
//            chainName[k].text = GetChainName(ActiveChainId);
//        }
//        userWallet.GetContractsAndDetails(address, ActiveChainId);
//        walletObject.SetActive(false);
//        ButtonConnected.gameObject.SetActive(true);
//    }
//}

