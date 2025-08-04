using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Thirdweb;
using Thirdweb.Unity;
using UnityEngine;
using static Bakeland.GetABI;

namespace Bakeland
{
    public class NftContracts : MonoBehaviour
    {

        public GetABI getABI;

        private void Awake()
        {
            getABI = new GetABI();
        }

        //
        // @notice This function is used to invoke the approve method on any NFT contract
        // @param nftAddress - address of NFT contract to invoke
        // @param to - address to approve
        // @param tokenId - id of token to approved for spending
        // @param activeChainId - chain id of currently active chain
        public async Task<string> Approve(string nftAddress, string to, string tokenId, BigInteger activeChainId)
        {
            try
            {
                var Wallet = ThirdwebManager.Instance.GetActiveWallet();
                var contract = await ThirdwebManager.Instance.GetContract(nftAddress, activeChainId, getABI.ERC721ABI());
                var tx = await contract.Write(Wallet, "approve", 0, to, tokenId);
                return tx.TransactionHash;

            }
            catch (Exception exception)
            {
                DebugUtils.LogError("Failed to approve NFT" + exception.Message);
                throw new Exception("Failed to approve NFT");
            }


        }

        public async Task<string> BalanceOf(string nftAddress, string user, BigInteger activeChainId)
        {
            try
            {
                var Wallet = ThirdwebManager.Instance.GetActiveWallet();
                var contract = await ThirdwebManager.Instance.GetContract(nftAddress, activeChainId, getABI.ERC721ABI());
                var balance = await contract.Read<BigInteger>("balanceOf", user);
                return balance.ToString();

            }
            catch (Exception exception)
            {
                DebugUtils.LogError("Failed to fetch balance" + exception.Message);
                throw new Exception("Failed to fetch balance");
            }


        }
    }
}
