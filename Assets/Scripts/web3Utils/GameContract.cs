using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Bakeland
{
    public class GameContract : MonoBehaviour
    {
        string address;

        void Start()
        {
            /// get contract address from registry
            /// create contract instance and save it into a variable
        }

        // Update is called once per frame
        void Update()
        {

        }

        //public async Task<string> Stake(string poolId, string amount)
        //{
        //    try
        //    {
        //        List<object> paramList = new() { amount, poolId };
        //        string transaction = await EvmService.WriteContract(GetAddressFromUserInfo(), address, "addStake", paramList, null);
        //        return transaction;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Failed to stake", ex);
        //    }

        //}

        //public async Task<string> raid(bool isBoosted, int riskLevel, int poolId, )
        //{
        //    try
        //    {
        //        /// call wormhole api
        //        /// convert string to byte array refer from wallet interaction file
        //        List<object> paramList = new() { amount, poolId };
        //        string transaction = await EvmService.WriteContract(GetAddressFromUserInfo(), address, "raid", paramList, null);
        //        return transaction;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Failed to stake", ex);
        //    }

        //}

        //private string GetAddressFromUserInfo()
        //{
        //    string userInfo = ParticleAuth.Instance.GetUserInfo();
        //    var wallets = JObject.Parse(userInfo)["wallets"];

        //    var evmWallet = wallets.Children<JObject>()
        //        .FirstOrDefault(wallet => wallet["chain_name"].ToString() == "evm_chain");
        //    if (evmWallet != null) return evmWallet["public_address"].ToString();

        //    throw new Exception("Failed to fetch connected user address");
        //}


    }
}
