using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Thirdweb.Unity;
using Thirdweb;

namespace Bakeland
{
    public class WalletConnector : MonoBehaviour
    {
        [SerializeField] private Button connectWalletButton;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private ulong chainId = 80069;

        [Header("address info display")]
        [SerializeField] private GameObject addressInfoPanel;
        [SerializeField] private TMP_Text walletAddress;
        [SerializeField] private TMP_Text walletBalance;
        [SerializeField] private Button disconnectButton;

        [SerializeField] private WalletProvider walletProvider = WalletProvider.WalletConnectWallet;
        [SerializeField] private bool forceMetaMaskOnWebGl = false;

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (addressInfoPanel != null) { 
                addressInfoPanel.SetActive(false);
            }

            if (connectWalletButton != null) { 
                connectWalletButton.gameObject.SetActive(true);
            }

            if (connectWalletButton != null)
            {
                connectWalletButton.onClick.RemoveAllListeners();
                connectWalletButton.onClick.AddListener(ConnectWallet);
            }
            else
            {
                Debug.LogError("ConnectWallet not assigned in the inspector");
                throw new System.Exception("ConnectWallet not assigned in the inspector");
            }
        }

        public void ConnectWallet()
        {
            if (statusText != null) {
                statusText.text = "connecting..";
            }

            ConnectExternalWallet();
        }

        public async void ConnectExternalWallet()
        {
            try
            {
                var provider = walletProvider;
                //wallet options
                var options = new WalletOptions(
                        provider: provider,
                        chainId: chainId
                 );

                var wallet = await ThirdwebManager.Instance.ConnectWallet(options);

                if (wallet != null) { 
                    
                    var address = await wallet.GetAddress();

                    if (connectWalletButton != null) {
                        connectWalletButton.gameObject.SetActive(false);
                    }

                    if (statusText != null)
                        statusText.text = "connected";
                
                    if(addressInfoPanel != null)
                    {
                        addressInfoPanel.SetActive(true);

                        if (walletAddress != null) {
                            string formattedAddy = address.Length > 10 ?
                                $"{address.Substring(0, 6)} ... {address.Substring(address.Length - 4)}"
                                : address;
                            walletAddress.text = formattedAddy;

                        }

                        if(walletBalance != null)
                        {
                            var bal = await wallet.GetBalance(chainId: chainId);
                            var chainDetails = await Thirdweb.Utils.GetChainMetadata(
                                client: ThirdwebManager.Instance.Client,
                                chainId: chainId
                             );

                            var symbol = chainDetails.NativeCurrency?.Symbol ?? "ETH";
                            var balanceEth = Thirdweb.Utils.ToEth(
                                wei: bal.ToString(),
                                decimalsToDisplay: 4,
                                addCommas: true
                            );
                            walletBalance.text = balanceEth;
                        }
                    }
                        
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError("Error connecting to wallet" + e.Message);
                if (statusText != null)
                {
                    statusText.text = "Errored";
                }
            }

        }


    }
}
