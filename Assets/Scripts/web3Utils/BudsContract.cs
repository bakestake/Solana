using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Thirdweb;
using Thirdweb.Unity;
using UnityEngine;
using Bakeland;
using NBitcoin;

namespace Bakeland
{
    public class BudsContract : MonoBehaviour
    {
        public GetABI getABI;

        private void Awake()
        {
            getABI = new GetABI();
        }


        public async Task<string> Approve(string to, BigInteger amount, string tokenId, BigInteger activeChainId)
        {
            try
            {
                var Wallet = ThirdwebManager.Instance.GetActiveWallet();
                var allowance = await checkAllowance(await Wallet.GetAddress(), to, activeChainId);
                if(allowance.CompareTo(amount) >= 0)
                {
                    return "";
                }
                var contract = await ThirdwebManager.Instance.GetContract(ContractRegistry.GetContractAddress(activeChainId, "buds"), activeChainId, getABI.BudsABI());
                var tx = await contract.Write(Wallet, "approve", 0, to, BigInteger.Pow(2, 255));
                return tx.TransactionHash;

            }
            catch (Exception exception)
            {
                DebugUtils.LogError("Failed to approve buds" + exception.Message);
                throw new Exception("Failed to approve buds");
            }


        }

        public async Task<string> BalanceOf(string user, BigInteger activeChainId)
        {
            try
            {
                var contract = await ThirdwebManager.Instance.GetContract(ContractRegistry.GetContractAddress(activeChainId, "buds"), activeChainId, getABI.BudsABI());
                var balance = await contract.Read<BigInteger>("balanceOf", user);
                return balance.ToString();

            }
            catch (Exception exception)
            {
                DebugUtils.LogError("Failed to fetch balance" + exception.Message);
                throw new Exception("Failed to fetch balance");
            }


        }

        public async Task<string> checkAllowance(string user, string to, BigInteger activeChainId)
        {
            try
            {
                var contract = await ThirdwebManager.Instance.GetContract(ContractRegistry.GetContractAddress(activeChainId, "buds"), activeChainId, getABI.BudsABI());
                var balance = await contract.Read<BigInteger>("allowance", user, to);
                return balance.ToString();

            }
            catch (Exception exception)
            {
                DebugUtils.LogError("Failed to fetch allowance" + exception.Message);
                throw new Exception("Failed to fetch allowance");
            }


        }

        public async Task<string> Bridge(BigInteger to, BigInteger toChainId, BigInteger amount, BigInteger activeChainId)
        {
            try
            {
                var Wallet = ThirdwebManager.Instance.GetActiveWallet();
                var contract = await ThirdwebManager.Instance.GetContract(ContractRegistry.GetContractAddress(activeChainId, "game"), activeChainId, getABI.GameABI());
                var eid = ContractRegistry.GetContractAddress(activeChainId, "eid");
                var cctxFee = await Read.GetCctxFeeAsync(activeChainId.ToString(), to.ToString(), 2);
                var tx = await contract.Write(Wallet, "crossChainBudsTransfer", BigInteger.Parse(cctxFee), ContractRegistry.GetContractAddress(activeChainId, "eid"), to, amount);
                return tx.TransactionHash;

            }
            catch (Exception exception)
            {
                DebugUtils.LogError("Failed to fetch balance" + exception.Message);
                throw new Exception("Failed to fetch balance");
            }
        }


    }
}
