using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Bakeland
{

    /// Manages contract addresses for multiple chains.
    /// Add more chains or contracts easily using the dictionary.
    public static class ContractRegistry
    {
        ///  Nested dictionary: [chain][name] = contractAddress
        private static readonly Dictionary<BigInteger, Dictionary<string, string>> contracts =
            new()
            {
                {
                    80069, new Dictionary<string, string>() // Bepolia
                    {
                        { "game", "0x2e1ca4cAAE7376C36A0A8655ed9E47b8cada2E19" },
                        { "buds", "0x51D283ac2B1b8337daDC923dA30881b8443969d1" },
                        { "stBuds", "0xDAF30CeeA3bFcdD8ec23c19Ac9668E91e58A6B3e" },
                        { "walletFactory", "0xd24c5f2A83246e3642631dAcCb8bFb6cdEf81B0F"},// TODO - add an address for account factory here
                        { "eid", "40371" },
                        { "inventory", "0xFfd951Ac580b61F94C1302e3c5a0a5d989F44aAa" }
                    }
                },
                {
                    421614, new Dictionary<string, string>() // Arbitrum Sepolia
                    {
                        { "game", "0x3FC9c156f03f23E461DE38c24B330b2c7329023F" },
                        { "buds", "0x889Cab3a0b08a4563d00Aa3Add78C51b8529C937" },
                        { "stBuds", "0xd4D865C482DCe718f004261C7682C223C70e9d5f" },
                        { "walletFactory", "0x9ef75aCe60Cb2A14bD05067C20D7e7009e958116"},
                        { "eid", "40231" },
                        { "inventory", "0xFfd951Ac580b61F94C1302e3c5a0a5d989F44aAa" }
                    }
                },
                {
                    43113, new Dictionary<string, string>() // Fuji
                    {
                        { "game", "0x74307E5E2F794d5aAd9d49b7e6DaC74BC3a9b42e" },
                        { "buds", "0x0068F425D8ed05ef663d662c78cb7180D61EB490" },
                        { "stBuds", "0xAE7dB9C003AAF01e4ef2CeC0e7728DB2f974028a" },
                        { "walletFactory", "0x9ef75aCe60Cb2A14bD05067C20D7e7009e958116"},
                        { "eid", "40106" },
                        { "inventory", "0xFfd951Ac580b61F94C1302e3c5a0a5d989F44aAa" }
                    }
                },
                {
                    10143, new Dictionary<string, string>() // monad
                    {
                        { "game", "0x74307E5E2F794d5aAd9d49b7e6DaC74BC3a9b42e" },
                        { "buds", "0x0068F425D8ed05ef663d662c78cb7180D61EB490" },
                        { "stBuds", "0xAE7dB9C003AAF01e4ef2CeC0e7728DB2f974028a" },
                        { "walletFactory", "0x9ef75aCe60Cb2A14bD05067C20D7e7009e958116"},
                        { "eid", "40106" },
                        { "inventory", "0xFfd951Ac580b61F94C1302e3c5a0a5d989F44aAa" }
                    }
                },

            };

        /// Returns the contract address for a given chain and contract type.
        /// @param chainId - chainId of chain
        /// @param name - name of the contract
        /// @returns address - address of the contract or null if doesn't exists
        public static string GetContractAddress(BigInteger chainId, string name)
        {
            name = name.Trim();

            if (contracts.TryGetValue(chainId, out var chainContracts))
            {
                if (chainContracts.TryGetValue(name, out var address))
                {
                    return address;
                }
                else
                {
                    Debug.LogWarning($"Contract '{name}' not found for chain ID {chainId}.");
                }
            }
            else
            {
                Debug.LogWarning($"Chain ID {chainId} not found in contract registry.");
            }

            return null;
        }

    }
}
