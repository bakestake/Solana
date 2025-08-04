using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public class GetABI : MonoBehaviour
    {
        // Example: Called at runtime or from other scripts to get ABIs

        public string BudsABI()
        {
            // Mock ERC20 ABI
            return @"[
    {
      ""inputs"": [],
      ""name"": ""AccessControlBadConfirmation"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": ""neededRole"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""AccessControlUnauthorizedAccount"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""target"",
          ""type"": ""address""
        }
      ],
      ""name"": ""AddressEmptyCode"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""implementation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC1967InvalidImplementation"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""ERC1967NonPayable"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""allowance"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""needed"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ERC20InsufficientAllowance"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""balance"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""needed"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ERC20InsufficientBalance"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""approver"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidApprover"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""receiver"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidReceiver"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidSender"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidSpender"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""FailedCall"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidInitialization"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotInitializing"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""OwnableInvalidOwner"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""OwnableUnauthorizedAccount"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""UUPSUnauthorizedCallContext"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""slot"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""UUPSUnsupportedProxiableUUID"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""ZeroAddress"",
      ""type"": ""error""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Approval"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint64"",
          ""name"": ""version"",
          ""type"": ""uint64""
        }
      ],
      ""name"": ""Initialized"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""previousOwner"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""newOwner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""OwnershipTransferred"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""previousAdminRole"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""newAdminRole"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""RoleAdminChanged"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""RoleGranted"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""RoleRevoked"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Transfer"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""implementation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""Upgraded"",
      ""type"": ""event""
    },
    {
      ""inputs"": [],
      ""name"": ""DEFAULT_ADMIN_ROLE"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""DIAMOND_CONTRACT"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""UPGRADER_ROLE"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""UPGRADE_INTERFACE_VERSION"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""allowance"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""approve"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""balanceOf"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_from"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""burnFrom"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""decimals"",
      ""outputs"": [
        {
          ""internalType"": ""uint8"",
          ""name"": """",
          ""type"": ""uint8""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""getRoleAdmin"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""grantRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""hasRole"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""diamondAddress"",
          ""type"": ""address""
        }
      ],
      ""name"": ""initialize"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""mintTo"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""name"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""owner"",
      ""outputs"": [
        {
          ""internalType"": ""address"",
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""proxiableUUID"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""renounceOwnership"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""callerConfirmation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""renounceRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""revokeRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes4"",
          ""name"": ""interfaceId"",
          ""type"": ""bytes4""
        }
      ],
      ""name"": ""supportsInterface"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""symbol"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""totalSupply"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""transfer"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""transferFrom"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""newOwner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""transferOwnership"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""newImplementation"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""data"",
          ""type"": ""bytes""
        }
      ],
      ""name"": ""upgradeToAndCall"",
      ""outputs"": [],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    }
  ]";
        }

        public string StBudsABI()
        {
            return @"[
    {
      ""inputs"": [],
      ""name"": ""AccessControlBadConfirmation"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": ""neededRole"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""AccessControlUnauthorizedAccount"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""target"",
          ""type"": ""address""
        }
      ],
      ""name"": ""AddressEmptyCode"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""implementation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC1967InvalidImplementation"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""ERC1967NonPayable"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""allowance"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""needed"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ERC20InsufficientAllowance"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""balance"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""needed"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ERC20InsufficientBalance"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""approver"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidApprover"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""receiver"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidReceiver"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidSender"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC20InvalidSpender"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""FailedCall"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidInitialization"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotInitializing"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""OwnableInvalidOwner"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""OwnableUnauthorizedAccount"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""UUPSUnauthorizedCallContext"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""slot"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""UUPSUnsupportedProxiableUUID"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""ZeroAddress"",
      ""type"": ""error""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Approval"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint64"",
          ""name"": ""version"",
          ""type"": ""uint64""
        }
      ],
      ""name"": ""Initialized"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""previousOwner"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""newOwner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""OwnershipTransferred"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""previousAdminRole"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""newAdminRole"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""RoleAdminChanged"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""RoleGranted"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""RoleRevoked"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Transfer"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""implementation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""Upgraded"",
      ""type"": ""event""
    },
    {
      ""inputs"": [],
      ""name"": ""DEFAULT_ADMIN_ROLE"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""DIAMOND_CONTRACT"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""NTT_MANAGER"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""UPGRADER_ROLE"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""UPGRADE_INTERFACE_VERSION"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""allowance"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""spender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""approve"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""balanceOf"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""burn"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""burnFrom"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""decimals"",
      ""outputs"": [
        {
          ""internalType"": ""uint8"",
          ""name"": """",
          ""type"": ""uint8""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""getRoleAdmin"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""grantRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""hasRole"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""diamondAddress"",
          ""type"": ""address""
        }
      ],
      ""name"": ""initialize"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""mint"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""mintTo"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""name"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""owner"",
      ""outputs"": [
        {
          ""internalType"": ""address"",
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""proxiableUUID"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""renounceOwnership"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""callerConfirmation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""renounceRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""revokeRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes4"",
          ""name"": ""interfaceId"",
          ""type"": ""bytes4""
        }
      ],
      ""name"": ""supportsInterface"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""symbol"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""totalSupply"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""transfer"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""transferFrom"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""newOwner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""transferOwnership"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""newImplementation"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""data"",
          ""type"": ""bytes""
        }
      ],
      ""name"": ""upgradeToAndCall"",
      ""outputs"": [],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    }
  ]";

        }

        public string ERC721ABI()
        {
            // Mock ERC721 ABI
            return @"[
    {
      ""inputs"": [],
      ""name"": ""AccessControlBadConfirmation"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": ""neededRole"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""AccessControlUnauthorizedAccount"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""target"",
          ""type"": ""address""
        }
      ],
      ""name"": ""AddressEmptyCode"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""CapReached"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""implementation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC1967InvalidImplementation"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""ERC1967NonPayable"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""ERC721EnumerableForbiddenBatchMint"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC721IncorrectOwner"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""operator"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ERC721InsufficientApproval"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""approver"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC721InvalidApprover"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""operator"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC721InvalidOperator"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC721InvalidOwner"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""receiver"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC721InvalidReceiver"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""ERC721InvalidSender"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ERC721NonexistentToken"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""index"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ERC721OutOfBoundsIndex"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""FailedCall"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidInitialization"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotInitializing"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""UUPSUnauthorizedCallContext"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""slot"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""UUPSUnsupportedProxiableUUID"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""ZeroAddress"",
      ""type"": ""error""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""approved"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Approval"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""operator"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""bool"",
          ""name"": ""approved"",
          ""type"": ""bool""
        }
      ],
      ""name"": ""ApprovalForAll"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""_fromTokenId"",
          ""type"": ""uint256""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""_toTokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""BatchMetadataUpdate"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint64"",
          ""name"": ""version"",
          ""type"": ""uint64""
        }
      ],
      ""name"": ""Initialized"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""_tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""MetadataUpdate"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""previousAdminRole"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""newAdminRole"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""RoleAdminChanged"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""RoleGranted"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""RoleRevoked"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""indexed"": true,
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Transfer"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""implementation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""Upgraded"",
      ""type"": ""event""
    },
    {
      ""inputs"": [],
      ""name"": ""DEFAULT_ADMIN_ROLE"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""DIAMOND_CONTRACT"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""MINTER_ROLE"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""UPGRADER_ROLE"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""UPGRADE_INTERFACE_VERSION"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""_tokensLeft"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""approve"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        }
      ],
      ""name"": ""balanceOf"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""balance"",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""baseUri"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getApproved"",
      ""outputs"": [
        {
          ""internalType"": ""address"",
          ""name"": """",
          ""type"": ""address""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""getRoleAdmin"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""grantRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""hasRole"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_diamondAddress"",
          ""type"": ""address""
        }
      ],
      ""name"": ""initialize"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""operator"",
          ""type"": ""address""
        }
      ],
      ""name"": ""isApprovedForAll"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""name"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ownerOf"",
      ""outputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""proxiableUUID"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""callerConfirmation"",
          ""type"": ""address""
        }
      ],
      ""name"": ""renounceRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""role"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""account"",
          ""type"": ""address""
        }
      ],
      ""name"": ""revokeRole"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""safeMint"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""safeTransferFrom"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""data"",
          ""type"": ""bytes""
        }
      ],
      ""name"": ""safeTransferFrom"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""operator"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bool"",
          ""name"": ""approved"",
          ""type"": ""bool""
        }
      ],
      ""name"": ""setApprovalForAll"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""string"",
          ""name"": ""uri"",
          ""type"": ""string""
        }
      ],
      ""name"": ""setBaseUri"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""uriString"",
          ""type"": ""string""
        }
      ],
      ""name"": ""setUriForToken"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bytes4"",
          ""name"": ""interfaceId"",
          ""type"": ""bytes4""
        }
      ],
      ""name"": ""supportsInterface"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""symbol"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""index"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""tokenByIndex"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""owner"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""index"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""tokenOfOwnerByIndex"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""tokenURI"",
      ""outputs"": [
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""totalSupply"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""transferFrom"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""newImplementation"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""data"",
          ""type"": ""bytes""
        }
      ],
      ""name"": ""upgradeToAndCall"",
      ""outputs"": [],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    }
  ]";
        }

        public string GameABI()
        {
            // Mock Game Contract ABI
            return @"[
    {
      ""inputs"": [],
      ""name"": ""InsufficientBalance"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InsufficientFees"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidData"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidDelegate"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidEndpointCall"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint16"",
          ""name"": ""optionType"",
          ""type"": ""uint16""
        }
      ],
      ""name"": ""InvalidOptionType"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidParams"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidPoolId"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidRiskLevel"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""LzTokenUnavailable"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""MaxBoostReached"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""eid"",
          ""type"": ""uint32""
        }
      ],
      ""name"": ""NoPeer"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotANarc"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""msgValue"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""NotEnoughNative"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotOwner"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""addr"",
          ""type"": ""address""
        }
      ],
      ""name"": ""OnlyEndpoint"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""eid"",
          ""type"": ""uint32""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": ""sender"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""OnlyPeer"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint8"",
          ""name"": ""bits"",
          ""type"": ""uint8""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""value"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""SafeCastOverflowedUintDowncast"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""token"",
          ""type"": ""address""
        }
      ],
      ""name"": ""SafeERC20FailedOperation"",
      ""type"": ""error""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""bytes32"",
          ""name"": ""messageId"",
          ""type"": ""bytes32""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint32"",
          ""name"": ""chainSelector"",
          ""type"": ""uint32""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""amount"",
          ""type"": ""uint256""
        },
        {
          ""indexed"": false,
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        }
      ],
      ""name"": ""CrossChainBudsTransfer"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint32"",
          ""name"": ""eid"",
          ""type"": ""uint32""
        },
        {
          ""indexed"": false,
          ""internalType"": ""bytes32"",
          ""name"": ""peer"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""PeerSet"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""amount"",
          ""type"": ""uint256""
        },
        {
          ""indexed"": false,
          ""internalType"": ""address"",
          ""name"": ""from"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""address"",
          ""name"": ""to"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""srcId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""ReceivedBuds"",
      ""type"": ""event""
    },
    {
      ""inputs"": [
        {
          ""components"": [
            {
              ""internalType"": ""uint32"",
              ""name"": ""srcEid"",
              ""type"": ""uint32""
            },
            {
              ""internalType"": ""bytes32"",
              ""name"": ""sender"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint64"",
              ""name"": ""nonce"",
              ""type"": ""uint64""
            }
          ],
          ""internalType"": ""struct Origin"",
          ""name"": ""origin"",
          ""type"": ""tuple""
        }
      ],
      ""name"": ""allowInitializePath"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_endpoint"",
          ""type"": ""address""
        }
      ],
      ""name"": ""changeEndpoint"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""_dstEid"",
          ""type"": ""uint32""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""_to"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""crossChainBudsTransfer"",
      ""outputs"": [
        {
          ""components"": [
            {
              ""internalType"": ""bytes32"",
              ""name"": ""guid"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint64"",
              ""name"": ""nonce"",
              ""type"": ""uint64""
            },
            {
              ""components"": [
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""nativeFee"",
                  ""type"": ""uint256""
                },
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""lzTokenFee"",
                  ""type"": ""uint256""
                }
              ],
              ""internalType"": ""struct MessagingFee"",
              ""name"": ""fee"",
              ""type"": ""tuple""
            }
          ],
          ""internalType"": ""struct MessagingReceipt"",
          ""name"": ""receipt"",
          ""type"": ""tuple""
        }
      ],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""_amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""crossChainPolStake"",
      ""outputs"": [
        {
          ""components"": [
            {
              ""internalType"": ""bytes32"",
              ""name"": ""guid"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint64"",
              ""name"": ""nonce"",
              ""type"": ""uint64""
            },
            {
              ""components"": [
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""nativeFee"",
                  ""type"": ""uint256""
                },
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""lzTokenFee"",
                  ""type"": ""uint256""
                }
              ],
              ""internalType"": ""struct MessagingFee"",
              ""name"": ""fee"",
              ""type"": ""tuple""
            }
          ],
          ""internalType"": ""struct MessagingReceipt"",
          ""name"": ""receipt"",
          ""type"": ""tuple""
        }
      ],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""destChainId"",
          ""type"": ""uint32""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""riskLevel"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""bool"",
          ""name"": ""boosted"",
          ""type"": ""bool""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""response"",
          ""type"": ""bytes""
        },
        {
          ""components"": [
            {
              ""internalType"": ""bytes32"",
              ""name"": ""r"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""bytes32"",
              ""name"": ""s"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint8"",
              ""name"": ""v"",
              ""type"": ""uint8""
            },
            {
              ""internalType"": ""uint8"",
              ""name"": ""guardianIndex"",
              ""type"": ""uint8""
            }
          ],
          ""internalType"": ""struct IWormhole.Signature[]"",
          ""name"": ""signatures"",
          ""type"": ""tuple[]""
        }
      ],
      ""name"": ""crossChainRaid"",
      ""outputs"": [
        {
          ""components"": [
            {
              ""internalType"": ""bytes32"",
              ""name"": ""guid"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint64"",
              ""name"": ""nonce"",
              ""type"": ""uint64""
            },
            {
              ""components"": [
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""nativeFee"",
                  ""type"": ""uint256""
                },
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""lzTokenFee"",
                  ""type"": ""uint256""
                }
              ],
              ""internalType"": ""struct MessagingFee"",
              ""name"": ""fee"",
              ""type"": ""tuple""
            }
          ],
          ""internalType"": ""struct MessagingReceipt"",
          ""name"": ""receipt"",
          ""type"": ""tuple""
        }
      ],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""destChainId"",
          ""type"": ""uint32""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_budsAmount"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""crossChainStake"",
      ""outputs"": [
        {
          ""components"": [
            {
              ""internalType"": ""bytes32"",
              ""name"": ""guid"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint64"",
              ""name"": ""nonce"",
              ""type"": ""uint64""
            },
            {
              ""components"": [
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""nativeFee"",
                  ""type"": ""uint256""
                },
                {
                  ""internalType"": ""uint256"",
                  ""name"": ""lzTokenFee"",
                  ""type"": ""uint256""
                }
              ],
              ""internalType"": ""struct MessagingFee"",
              ""name"": ""fee"",
              ""type"": ""tuple""
            }
          ],
          ""internalType"": ""struct MessagingReceipt"",
          ""name"": ""receipt"",
          ""type"": ""tuple""
        }
      ],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""endpoint"",
      ""outputs"": [
        {
          ""internalType"": ""contract ILayerZeroEndpointV2"",
          ""name"": ""iEndpoint"",
          ""type"": ""address""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""eId"",
          ""type"": ""uint32""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""budsAmount"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""sender"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": ""msgtype"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""getCctxFees"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""components"": [
            {
              ""internalType"": ""uint32"",
              ""name"": ""srcEid"",
              ""type"": ""uint32""
            },
            {
              ""internalType"": ""bytes32"",
              ""name"": ""sender"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint64"",
              ""name"": ""nonce"",
              ""type"": ""uint64""
            }
          ],
          ""internalType"": ""struct Origin"",
          ""name"": """",
          ""type"": ""tuple""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": """",
          ""type"": ""bytes""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""_sender"",
          ""type"": ""address""
        }
      ],
      ""name"": ""isComposeMsgSender"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""components"": [
            {
              ""internalType"": ""uint32"",
              ""name"": ""srcEid"",
              ""type"": ""uint32""
            },
            {
              ""internalType"": ""bytes32"",
              ""name"": ""sender"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint64"",
              ""name"": ""nonce"",
              ""type"": ""uint64""
            }
          ],
          ""internalType"": ""struct Origin"",
          ""name"": ""_origin"",
          ""type"": ""tuple""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": ""_guid"",
          ""type"": ""bytes32""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""_message"",
          ""type"": ""bytes""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""_executor"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""_extraData"",
          ""type"": ""bytes""
        }
      ],
      ""name"": ""lzReceive"",
      ""outputs"": [],
      ""stateMutability"": ""payable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": """",
          ""type"": ""uint32""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": """",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""nextNonce"",
      ""outputs"": [
        {
          ""internalType"": ""uint64"",
          ""name"": ""nonce"",
          ""type"": ""uint64""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""oAppVersion"",
      ""outputs"": [
        {
          ""internalType"": ""uint64"",
          ""name"": ""senderVersion"",
          ""type"": ""uint64""
        },
        {
          ""internalType"": ""uint64"",
          ""name"": ""receiverVersion"",
          ""type"": ""uint64""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""_eid"",
          ""type"": ""uint32""
        }
      ],
      ""name"": ""peers"",
      ""outputs"": [
        {
          ""internalType"": ""bytes32"",
          ""name"": ""peer"",
          ""type"": ""bytes32""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""_delegate"",
          ""type"": ""address""
        }
      ],
      ""name"": ""setDelegate"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint32"",
          ""name"": ""_eid"",
          ""type"": ""uint32""
        },
        {
          ""internalType"": ""bytes32"",
          ""name"": ""_peer"",
          ""type"": ""bytes32""
        }
      ],
      ""name"": ""setPeer"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""AlreadyDelegated"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""AlreadyFunded"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""CannotClaim"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""DueRewardsUnclaimed"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidPoolId"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NoDelagtionFound"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotOwner"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotOwnerOfAsset"",
      ""type"": ""error""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""epochNumber"",
          ""type"": ""uint256""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""lastEpoch"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""EpochAdvanced"",
      ""type"": ""event""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""changeDelegation"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""claimDelagationReward"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""delegateFarmer"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""fundFarmerBonus"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""epoch"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getBonusAllocationForEpoch"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""epoch"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getBonusForPoolByEpoch"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""epoch"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getDelegatedFarmersInEpoch"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""removeDelegation"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""InsuffficientXP"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""LevelMaxedOut"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""levelUpFarmer"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""tokenId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""levelUpNarc"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""InsufficientRaidFees"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidData"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidPoolId"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidRiskLevel"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""MaxBoostReached"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotANarc"",
      ""type"": ""error""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""prod1"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""denominator"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""PRBMath__MulDivOverflow"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""Unauthorized"",
      ""type"": ""error""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""epochNumber"",
          ""type"": ""uint256""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""lastEpoch"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""EpochAdvanced"",
      ""type"": ""event""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": true,
          ""internalType"": ""address"",
          ""name"": ""raider"",
          ""type"": ""address""
        },
        {
          ""indexed"": false,
          ""internalType"": ""bool"",
          ""name"": ""isSuccess"",
          ""type"": ""bool""
        },
        {
          ""indexed"": false,
          ""internalType"": ""bool"",
          ""name"": ""isBoosted"",
          ""type"": ""bool""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""rewardTaken"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""Raided"",
      ""type"": ""event""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": ""boosted"",
          ""type"": ""bool""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""riskLevel"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""poolId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""bytes"",
          ""name"": ""response"",
          ""type"": ""bytes""
        },
        {
          ""components"": [
            {
              ""internalType"": ""bytes32"",
              ""name"": ""r"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""bytes32"",
              ""name"": ""s"",
              ""type"": ""bytes32""
            },
            {
              ""internalType"": ""uint8"",
              ""name"": ""v"",
              ""type"": ""uint8""
            },
            {
              ""internalType"": ""uint8"",
              ""name"": ""guardianIndex"",
              ""type"": ""uint8""
            }
          ],
          ""internalType"": ""struct IWormhole.Signature[]"",
          ""name"": ""signatures"",
          ""type"": ""tuple[]""
        }
      ],
      ""name"": ""raid"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""requestId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""randomNumber"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""randomNumberCallback"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [],
      ""name"": ""BlackListed"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InsufficientStake"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidData"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""InvalidPoolId"",
      ""type"": ""error""
    },
    {
      ""inputs"": [],
      ""name"": ""NotOwner"",
      ""type"": ""error""
    },
    {
      ""anonymous"": false,
      ""inputs"": [
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""epochNumber"",
          ""type"": ""uint256""
        },
        {
          ""indexed"": false,
          ""internalType"": ""uint256"",
          ""name"": ""lastEpoch"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""EpochAdvanced"",
      ""type"": ""event""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""_budsAmount"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""addStake"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""_poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""claimRewards"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""_poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""discardPool"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""_poolId"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_amount"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""fundPool"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""_budsAmount"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""_poolId"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""unStakeBuds"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    
  ]";
        }
    }
}
