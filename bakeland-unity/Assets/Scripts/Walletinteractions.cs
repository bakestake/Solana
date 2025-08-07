using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Thirdweb;
using System.Threading.Tasks;
using System.Numerics;
using Nethereum.Util;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.LowLevel;
using System.Net.Mail;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Collections.Generic;
using UnityEngine.Experimental.AI;
using static System.Net.WebRequestMethods;
using Nethereum.Signer;
using Nethereum.Web3;
using WalletConnectUnity.Core;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using Thirdweb.Unity;
using Org.BouncyCastle.Bcpg;
using static WalletConnectUnity.Core.ChainConstants;
using Thirdweb.Unity.Examples;
using ZXing.Common;
using NBitcoin;
using Unity.Collections;

namespace BakelandWalletInteraction
{
    [RequireComponent(typeof(TMP_Text))]
    public class UaserWalletInteractions : MonoBehaviour
    {
        [Tooltip("Contract ABI(s)")]
        private string BUDS_ABI = "[{\"constant\":true,\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_spender\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_from\",\"type\":\"address\"},{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"name\":\"\",\"type\":\"uint8\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"balance\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_value\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"},{\"name\":\"_spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"payable\":true,\"stateMutability\":\"payable\",\"type\":\"fallback\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"}]";
        // These four are same 
        private string FARMER_ABI = "[{\"inputs\":[],\"name\":\"AccessControlBadConfirmation\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"neededRole\",\"type\":\"bytes32\"}],\"name\":\"AccessControlUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"}],\"name\":\"AddressEmptyCode\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"CapReached\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"ERC1967InvalidImplementation\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC1967NonPayable\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC721EnumerableForbiddenBatchMint\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721IncorrectOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721InsufficientApproval\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"approver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidApprover\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOperator\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"receiver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidReceiver\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"ERC721InvalidSender\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721NonexistentToken\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"ERC721OutOfBoundsIndex\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FailedInnerCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UUPSUnauthorizedCallContext\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"slot\",\"type\":\"bytes32\"}],\"name\":\"UUPSUnsupportedProxiableUUID\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UnauthorizedAccess\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ZeroAddress\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_fromTokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_toTokenId\",\"type\":\"uint256\"}],\"name\":\"BatchMetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"MetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"previousAdminRole\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"newAdminRole\",\"type\":\"bytes32\"}],\"name\":\"RoleAdminChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleGranted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleRevoked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"Upgraded\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"DEFAULT_ADMIN_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"MINTER_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"STAKING_CONTRACT\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADER_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADE_INTERFACE_VERSION\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"balance\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burnFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"}],\"name\":\"getRoleAdmin\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"grantRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"hasRole\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_seed\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_stakingAddress\",\"type\":\"address\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"mintTokenId\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proxiableUUID\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"revokeRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"}],\"name\":\"upgradeTo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"upgradeToAndCall\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
        private string STONNER_ABI = "[{\"inputs\":[],\"name\":\"AccessControlBadConfirmation\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"neededRole\",\"type\":\"bytes32\"}],\"name\":\"AccessControlUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"}],\"name\":\"AddressEmptyCode\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"ERC1967InvalidImplementation\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC1967NonPayable\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC721EnumerableForbiddenBatchMint\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721IncorrectOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721InsufficientApproval\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"approver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidApprover\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOperator\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"receiver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidReceiver\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"ERC721InvalidSender\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721NonexistentToken\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"ERC721OutOfBoundsIndex\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FailedInnerCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UUPSUnauthorizedCallContext\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"slot\",\"type\":\"bytes32\"}],\"name\":\"UUPSUnsupportedProxiableUUID\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UnauthorizedAccess\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ZeroAddress\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_fromTokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_toTokenId\",\"type\":\"uint256\"}],\"name\":\"BatchMetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"MetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"previousAdminRole\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"newAdminRole\",\"type\":\"bytes32\"}],\"name\":\"RoleAdminChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleGranted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleRevoked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"Upgraded\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"DEFAULT_ADMIN_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"STAKING_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADER_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADE_INTERFACE_VERSION\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burnFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"}],\"name\":\"getRoleAdmin\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"grantRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"hasRole\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"seed\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_stakingAddress\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_uri\",\"type\":\"string\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"mintTokenId\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proxiableUUID\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"revokeRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"}],\"name\":\"upgradeTo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"upgradeToAndCall\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"}]\n";
        private string INFORMANT_ABI = "[{\"inputs\":[],\"name\":\"AccessControlBadConfirmation\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"neededRole\",\"type\":\"bytes32\"}],\"name\":\"AccessControlUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"}],\"name\":\"AddressEmptyCode\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"ERC1967InvalidImplementation\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC1967NonPayable\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC721EnumerableForbiddenBatchMint\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721IncorrectOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721InsufficientApproval\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"approver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidApprover\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOperator\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"receiver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidReceiver\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"ERC721InvalidSender\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721NonexistentToken\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"ERC721OutOfBoundsIndex\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FailedInnerCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UUPSUnauthorizedCallContext\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"slot\",\"type\":\"bytes32\"}],\"name\":\"UUPSUnsupportedProxiableUUID\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UnauthorizedAccess\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ZeroAddress\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_fromTokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_toTokenId\",\"type\":\"uint256\"}],\"name\":\"BatchMetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"MetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"previousAdminRole\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"newAdminRole\",\"type\":\"bytes32\"}],\"name\":\"RoleAdminChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleGranted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleRevoked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"Upgraded\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"DEFAULT_ADMIN_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"STAKING_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADER_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADE_INTERFACE_VERSION\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burnFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"}],\"name\":\"getRoleAdmin\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"grantRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"hasRole\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"seed\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_stakingAddress\",\"type\":\"address\"},{\"internalType\":\"string\",\"name\":\"_uri\",\"type\":\"string\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"mintTokenId\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proxiableUUID\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"revokeRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"}],\"name\":\"upgradeTo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"upgradeToAndCall\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"}]\n";
        private string NARC_ABI = "[{\"inputs\":[],\"name\":\"AccessControlBadConfirmation\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"neededRole\",\"type\":\"bytes32\"}],\"name\":\"AccessControlUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"}],\"name\":\"AddressEmptyCode\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"CapReached\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"ERC1967InvalidImplementation\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC1967NonPayable\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC721EnumerableForbiddenBatchMint\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721IncorrectOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721InsufficientApproval\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"approver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidApprover\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOperator\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"ERC721InvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"receiver\",\"type\":\"address\"}],\"name\":\"ERC721InvalidReceiver\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"ERC721InvalidSender\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ERC721NonexistentToken\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"ERC721OutOfBoundsIndex\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FailedInnerCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UUPSUnauthorizedCallContext\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"slot\",\"type\":\"bytes32\"}],\"name\":\"UUPSUnsupportedProxiableUUID\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UnauthorizedAccess\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ZeroAddress\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_fromTokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_toTokenId\",\"type\":\"uint256\"}],\"name\":\"BatchMetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"MetadataUpdate\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"previousAdminRole\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"newAdminRole\",\"type\":\"bytes32\"}],\"name\":\"RoleAdminChanged\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleGranted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"RoleRevoked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"Upgraded\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"DEFAULT_ADMIN_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"MINTER_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"STAKING_CONTRACT\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADER_ROLE\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADE_INTERFACE_VERSION\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"balance\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"burnFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"}],\"name\":\"getRoleAdmin\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"grantRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"hasRole\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_seed\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"_stakingAddress\",\"type\":\"address\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"mintTokenId\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proxiableUUID\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"role\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"revokeRole\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"}],\"name\":\"upgradeTo\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"upgradeToAndCall\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
        // Till here
        private string STAKE_ABI = "[{\"inputs\":[],\"name\":\"burnForInformant\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"requestId\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"burnForStoner\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"requestId\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_nonce\",\"type\":\"uint256\"},{\"internalType\":\"uint256[]\",\"name\":\"_rngList\",\"type\":\"uint256[]\"}],\"name\":\"mintBooster\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"boosterType\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_budsAmount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_farmerTokenId\",\"type\":\"uint256\"}],\"name\":\"addStake\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"stakeIndex\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"response\",\"type\":\"bytes\"},{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"r\",\"type\":\"bytes32\"},{\"internalType\":\"bytes32\",\"name\":\"s\",\"type\":\"bytes32\"},{\"internalType\":\"uint8\",\"name\":\"v\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"guardianIndex\",\"type\":\"uint8\"}],\"internalType\":\"struct IWormhole.Signature[]\",\"name\":\"signatures\",\"type\":\"tuple[]\"}],\"name\":\"boostStake\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"response\",\"type\":\"bytes\"},{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"r\",\"type\":\"bytes32\"},{\"internalType\":\"bytes32\",\"name\":\"s\",\"type\":\"bytes32\"},{\"internalType\":\"uint8\",\"name\":\"v\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"guardianIndex\",\"type\":\"uint8\"}],\"internalType\":\"struct IWormhole.Signature[]\",\"name\":\"signatures\",\"type\":\"tuple[]\"}],\"name\":\"claimAndUnstake\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"response\",\"type\":\"bytes\"},{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"r\",\"type\":\"bytes32\"},{\"internalType\":\"bytes32\",\"name\":\"s\",\"type\":\"bytes32\"},{\"internalType\":\"uint8\",\"name\":\"v\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"guardianIndex\",\"type\":\"uint8\"}],\"internalType\":\"struct IWormhole.Signature[]\",\"name\":\"signatures\",\"type\":\"tuple[]\"}],\"name\":\"claimRewards\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"response\",\"type\":\"bytes\"},{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"r\",\"type\":\"bytes32\"},{\"internalType\":\"bytes32\",\"name\":\"s\",\"type\":\"bytes32\"},{\"internalType\":\"uint8\",\"name\":\"v\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"guardianIndex\",\"type\":\"uint8\"}],\"internalType\":\"struct IWormhole.Signature[]\",\"name\":\"signatures\",\"type\":\"tuple[]\"}],\"name\":\"raid\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"riskLevel\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"response\",\"type\":\"bytes\"},{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"r\",\"type\":\"bytes32\"},{\"internalType\":\"bytes32\",\"name\":\"s\",\"type\":\"bytes32\"},{\"internalType\":\"uint8\",\"name\":\"v\",\"type\":\"uint8\"},{\"internalType\":\"uint8\",\"name\":\"guardianIndex\",\"type\":\"uint8\"}],\"internalType\":\"struct IWormhole.Signature[]\",\"name\":\"signatures\",\"type\":\"tuple[]\"}],\"name\":\"raidCustom\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"responsePrefix\",\"outputs\":[{\"internalType\":\"bytes\",\"name\":\"\",\"type\":\"bytes\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_budsAmount\",\"type\":\"uint256\"}],\"name\":\"unStakeBuds\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"unStakeFarmer\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint32\",\"name\":\"_dstEid\",\"type\":\"uint32\"},{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"crossChainBudsTransfer\",\"outputs\":[{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"guid\",\"type\":\"bytes32\"},{\"internalType\":\"uint64\",\"name\":\"nonce\",\"type\":\"uint64\"},{\"components\":[{\"internalType\":\"uint256\",\"name\":\"nativeFee\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"lzTokenFee\",\"type\":\"uint256\"}],\"internalType\":\"struct MessagingFee\",\"name\":\"fee\",\"type\":\"tuple\"}],\"internalType\":\"struct MessagingReceipt\",\"name\":\"receipt\",\"type\":\"tuple\"}],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint32\",\"name\":\"_dstEid\",\"type\":\"uint32\"},{\"internalType\":\"address\",\"name\":\"_to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint8\",\"name\":\"tokenNumber\",\"type\":\"uint8\"}],\"name\":\"crossChainNFTTransfer\",\"outputs\":[{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"guid\",\"type\":\"bytes32\"},{\"internalType\":\"uint64\",\"name\":\"nonce\",\"type\":\"uint64\"},{\"components\":[{\"internalType\":\"uint256\",\"name\":\"nativeFee\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"lzTokenFee\",\"type\":\"uint256\"}],\"internalType\":\"struct MessagingFee\",\"name\":\"fee\",\"type\":\"tuple\"}],\"internalType\":\"struct MessagingReceipt\",\"name\":\"receipt\",\"type\":\"tuple\"}],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint32\",\"name\":\"destChainId\",\"type\":\"uint32\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"crossChainRaid\",\"outputs\":[{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"guid\",\"type\":\"bytes32\"},{\"internalType\":\"uint64\",\"name\":\"nonce\",\"type\":\"uint64\"},{\"components\":[{\"internalType\":\"uint256\",\"name\":\"nativeFee\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"lzTokenFee\",\"type\":\"uint256\"}],\"internalType\":\"struct MessagingFee\",\"name\":\"fee\",\"type\":\"tuple\"}],\"internalType\":\"struct MessagingReceipt\",\"name\":\"receipt\",\"type\":\"tuple\"}],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_budsAmount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_farmerTokenId\",\"type\":\"uint256\"},{\"internalType\":\"uint32\",\"name\":\"destChainId\",\"type\":\"uint32\"}],\"name\":\"crossChainStake\",\"outputs\":[{\"components\":[{\"internalType\":\"bytes32\",\"name\":\"guid\",\"type\":\"bytes32\"},{\"internalType\":\"uint64\",\"name\":\"nonce\",\"type\":\"uint64\"},{\"components\":[{\"internalType\":\"uint256\",\"name\":\"nativeFee\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"lzTokenFee\",\"type\":\"uint256\"}],\"internalType\":\"struct MessagingFee\",\"name\":\"fee\",\"type\":\"tuple\"}],\"internalType\":\"struct MessagingReceipt\",\"name\":\"receipt\",\"type\":\"tuple\"}],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"endpoint\",\"outputs\":[{\"internalType\":\"contract ILayerZeroEndpointV2\",\"name\":\"iEndpoint\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint32\",\"name\":\"eId\",\"type\":\"uint32\"},{\"internalType\":\"uint256\",\"name\":\"budsAmount\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"sender\",\"type\":\"address\"}],\"name\":\"getCctxFees\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"}],\"name\":\"getRewardsForUser\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"rewards\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"mintedBooster\",\"type\":\"string\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Burned\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"messageId\",\"type\":\"bytes32\"},{\"indexed\":false,\"internalType\":\"uint32\",\"name\":\"chainSelector\",\"type\":\"uint32\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"}],\"name\":\"CrossChainBudsTransfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"messageId\",\"type\":\"bytes32\"},{\"indexed\":false,\"internalType\":\"uint32\",\"name\":\"chainSelector\",\"type\":\"uint32\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"}],\"name\":\"CrossChainNFTTransfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"raider\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"isSuccess\",\"type\":\"bool\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"isBoosted\",\"type\":\"bool\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"rewardTaken\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"boostsUsedInLastSevenDays\",\"type\":\"uint256\"}],\"name\":\"Raided\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"budsAmount\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"timeStamp\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"localStakedBudsCount\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"latestAPR\",\"type\":\"uint256\"}],\"name\":\"Staked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"budsAmount\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"timeStamp\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"localStakedBudsCount\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"latestAPR\",\"type\":\"uint256\"}],\"name\":\"UnStaked\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"messageId\",\"type\":\"bytes32\"},{\"indexed\":false,\"internalType\":\"bytes\",\"name\":\"reason\",\"type\":\"bytes\"}],\"name\":\"crossChainReceptionFailed\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"messageId\",\"type\":\"bytes32\"}],\"name\":\"recoveredFailedReceipt\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"messageId\",\"type\":\"bytes32\"},{\"indexed\":false,\"internalType\":\"bytes\",\"name\":\"reason\",\"type\":\"bytes\"}],\"name\":\"crossChainStakeFailed\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"bytes32\",\"name\":\"messageId\",\"type\":\"bytes32\"}],\"name\":\"recoveredFailedStake\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"rewards\",\"type\":\"uint256\"}],\"name\":\"rewardsClaimed\",\"type\":\"event\"}]";
        private string SMART_WALLET_ABI = "[{\"type\":\"constructor\",\"name\":\"\",\"inputs\":[{\"type\":\"address\",\"name\":\"_entrypoint\",\"internalType\":\"contract IEntryPoint\"},{\"type\":\"address\",\"name\":\"_factory\",\"internalType\":\"address\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"event\",\"name\":\"AdminUpdated\",\"inputs\":[{\"type\":\"address\",\"name\":\"signer\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"bool\",\"name\":\"isAdmin\",\"indexed\":false,\"internalType\":\"bool\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"event\",\"name\":\"ContractURIUpdated\",\"inputs\":[{\"type\":\"string\",\"name\":\"prevURI\",\"indexed\":false,\"internalType\":\"string\"},{\"type\":\"string\",\"name\":\"newURI\",\"indexed\":false,\"internalType\":\"string\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"event\",\"name\":\"Initialized\",\"inputs\":[{\"type\":\"uint8\",\"name\":\"version\",\"indexed\":false,\"internalType\":\"uint8\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"event\",\"name\":\"SignerPermissionsUpdated\",\"inputs\":[{\"type\":\"address\",\"name\":\"authorizingSigner\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"address\",\"name\":\"targetSigner\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"tuple\",\"name\":\"permissions\",\"components\":[{\"type\":\"address\",\"name\":\"signer\",\"internalType\":\"address\"},{\"type\":\"uint8\",\"name\":\"isAdmin\",\"internalType\":\"uint8\"},{\"type\":\"address[]\",\"name\":\"approvedTargets\",\"internalType\":\"address[]\"},{\"type\":\"uint256\",\"name\":\"nativeTokenLimitPerTransaction\",\"internalType\":\"uint256\"},{\"type\":\"uint128\",\"name\":\"permissionStartTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"permissionEndTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"reqValidityStartTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"reqValidityEndTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"bytes32\",\"name\":\"uid\",\"internalType\":\"bytes32\"}],\"indexed\":false,\"internalType\":\"struct IAccountPermissions.SignerPermissionRequest\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"function\",\"name\":\"addDeposit\",\"inputs\":[],\"outputs\":[],\"stateMutability\":\"payable\"},{\"type\":\"function\",\"name\":\"contractURI\",\"inputs\":[],\"outputs\":[{\"type\":\"string\",\"name\":\"\",\"internalType\":\"string\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"entryPoint\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"contract IEntryPoint\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"execute\",\"inputs\":[{\"type\":\"address\",\"name\":\"_target\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"_value\",\"internalType\":\"uint256\"},{\"type\":\"bytes\",\"name\":\"_calldata\",\"internalType\":\"bytes\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"executeBatch\",\"inputs\":[{\"type\":\"address[]\",\"name\":\"_target\",\"internalType\":\"address[]\"},{\"type\":\"uint256[]\",\"name\":\"_value\",\"internalType\":\"uint256[]\"},{\"type\":\"bytes[]\",\"name\":\"_calldata\",\"internalType\":\"bytes[]\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"factory\",\"inputs\":[],\"outputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getAllActiveSigners\",\"inputs\":[],\"outputs\":[{\"type\":\"tuple[]\",\"name\":\"signers\",\"components\":[{\"type\":\"address\",\"name\":\"signer\",\"internalType\":\"address\"},{\"type\":\"address[]\",\"name\":\"approvedTargets\",\"internalType\":\"address[]\"},{\"type\":\"uint256\",\"name\":\"nativeTokenLimitPerTransaction\",\"internalType\":\"uint256\"},{\"type\":\"uint128\",\"name\":\"startTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"endTimestamp\",\"internalType\":\"uint128\"}],\"internalType\":\"struct IAccountPermissions.SignerPermissions[]\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getAllAdmins\",\"inputs\":[],\"outputs\":[{\"type\":\"address[]\",\"name\":\"\",\"internalType\":\"address[]\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getAllSigners\",\"inputs\":[],\"outputs\":[{\"type\":\"tuple[]\",\"name\":\"signers\",\"components\":[{\"type\":\"address\",\"name\":\"signer\",\"internalType\":\"address\"},{\"type\":\"address[]\",\"name\":\"approvedTargets\",\"internalType\":\"address[]\"},{\"type\":\"uint256\",\"name\":\"nativeTokenLimitPerTransaction\",\"internalType\":\"uint256\"},{\"type\":\"uint128\",\"name\":\"startTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"endTimestamp\",\"internalType\":\"uint128\"}],\"internalType\":\"struct IAccountPermissions.SignerPermissions[]\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getNonce\",\"inputs\":[],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"getPermissionsForSigner\",\"inputs\":[{\"type\":\"address\",\"name\":\"signer\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"tuple\",\"name\":\"\",\"components\":[{\"type\":\"address\",\"name\":\"signer\",\"internalType\":\"address\"},{\"type\":\"address[]\",\"name\":\"approvedTargets\",\"internalType\":\"address[]\"},{\"type\":\"uint256\",\"name\":\"nativeTokenLimitPerTransaction\",\"internalType\":\"uint256\"},{\"type\":\"uint128\",\"name\":\"startTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"endTimestamp\",\"internalType\":\"uint128\"}],\"internalType\":\"struct IAccountPermissions.SignerPermissions\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"initialize\",\"inputs\":[{\"type\":\"address\",\"name\":\"_defaultAdmin\",\"internalType\":\"address\"},{\"type\":\"bytes\",\"name\":\"_data\",\"internalType\":\"bytes\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"isActiveSigner\",\"inputs\":[{\"type\":\"address\",\"name\":\"signer\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"bool\",\"name\":\"\",\"internalType\":\"bool\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"isAdmin\",\"inputs\":[{\"type\":\"address\",\"name\":\"_account\",\"internalType\":\"address\"}],\"outputs\":[{\"type\":\"bool\",\"name\":\"\",\"internalType\":\"bool\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"isValidSignature\",\"inputs\":[{\"type\":\"bytes32\",\"name\":\"_hash\",\"internalType\":\"bytes32\"},{\"type\":\"bytes\",\"name\":\"_signature\",\"internalType\":\"bytes\"}],\"outputs\":[{\"type\":\"bytes4\",\"name\":\"magicValue\",\"internalType\":\"bytes4\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"isValidSigner\",\"inputs\":[{\"type\":\"address\",\"name\":\"_signer\",\"internalType\":\"address\"},{\"type\":\"tuple\",\"name\":\"_userOp\",\"components\":[{\"type\":\"address\",\"name\":\"sender\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"nonce\",\"internalType\":\"uint256\"},{\"type\":\"bytes\",\"name\":\"initCode\",\"internalType\":\"bytes\"},{\"type\":\"bytes\",\"name\":\"callData\",\"internalType\":\"bytes\"},{\"type\":\"uint256\",\"name\":\"callGasLimit\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"verificationGasLimit\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"preVerificationGas\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"maxFeePerGas\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"maxPriorityFeePerGas\",\"internalType\":\"uint256\"},{\"type\":\"bytes\",\"name\":\"paymasterAndData\",\"internalType\":\"bytes\"},{\"type\":\"bytes\",\"name\":\"signature\",\"internalType\":\"bytes\"}],\"internalType\":\"struct UserOperation\"}],\"outputs\":[{\"type\":\"bool\",\"name\":\"\",\"internalType\":\"bool\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"setAdmin\",\"inputs\":[{\"type\":\"address\",\"name\":\"_adminSigner\",\"internalType\":\"address\"},{\"type\":\"bool\",\"name\":\"_isAdmin\",\"internalType\":\"bool\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"setContractURI\",\"inputs\":[{\"type\":\"string\",\"name\":\"_uri\",\"internalType\":\"string\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"setPermissionsForSigner\",\"inputs\":[{\"type\":\"tuple\",\"name\":\"_req\",\"components\":[{\"type\":\"address\",\"name\":\"signer\",\"internalType\":\"address\"},{\"type\":\"uint8\",\"name\":\"isAdmin\",\"internalType\":\"uint8\"},{\"type\":\"address[]\",\"name\":\"approvedTargets\",\"internalType\":\"address[]\"},{\"type\":\"uint256\",\"name\":\"nativeTokenLimitPerTransaction\",\"internalType\":\"uint256\"},{\"type\":\"uint128\",\"name\":\"permissionStartTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"permissionEndTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"reqValidityStartTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"uint128\",\"name\":\"reqValidityEndTimestamp\",\"internalType\":\"uint128\"},{\"type\":\"bytes32\",\"name\":\"uid\",\"internalType\":\"bytes32\"}],\"internalType\":\"struct IAccountPermissions.SignerPermissionRequest\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"supportsInterface\",\"inputs\":[{\"type\":\"bytes4\",\"name\":\"interfaceId\",\"internalType\":\"bytes4\"}],\"outputs\":[{\"type\":\"bool\",\"name\":\"\",\"internalType\":\"bool\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"withdrawDepositTo\",\"inputs\":[{\"type\":\"address\",\"name\":\"withdrawAddress\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"amount\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]";
        private string BUDS_FAUCET_ABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"}],\"name\":\"AddressEmptyCode\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"ERC1967InvalidImplementation\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC1967NonPayable\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FailedInnerCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"OwnableInvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"OwnableUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UUPSUnauthorizedCallContext\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"slot\",\"type\":\"bytes32\"}],\"name\":\"UUPSUnsupportedProxiableUUID\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"Upgraded\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"UPGRADE_INTERFACE_VERSION\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"_budsToken\",\"outputs\":[{\"internalType\":\"contract IERC20\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_receiver\",\"type\":\"address\"}],\"name\":\"claim\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_budsAddress\",\"type\":\"address\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"lastClaimeBy\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_sender\",\"type\":\"address\"}],\"name\":\"nextClaimTimeInSeconds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proxiableUUID\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_budsToken_\",\"type\":\"address\"}],\"name\":\"setBudsAddress\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"upgradeToAndCall\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"}]";
        private string NFT_FAUCET_ABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"}],\"name\":\"AddressEmptyCode\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"ERC1967InvalidImplementation\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC1967NonPayable\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FailedInnerCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"OwnableInvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"OwnableUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UUPSUnauthorizedCallContext\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"slot\",\"type\":\"bytes32\"}],\"name\":\"UUPSUnsupportedProxiableUUID\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"Upgraded\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"UPGRADE_INTERFACE_VERSION\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"_farmerToken\",\"outputs\":[{\"internalType\":\"contract IChars\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"_narcToken\",\"outputs\":[{\"internalType\":\"contract IChars\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"canClaimFarmer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"canClaimNarc\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"claimFarmer\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"claimNarc\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"farmerClaimedBy\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_farmer\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_narcs\",\"type\":\"address\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"narcClaimedBy\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proxiableUUID\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_farmer\",\"type\":\"address\"}],\"name\":\"setFarmer\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_narc\",\"type\":\"address\"}],\"name\":\"setNarc\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"upgradeToAndCall\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"}]";
        private string InventoryItemABI = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[],\"name\":\"AlreadyInitialized\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ApprovalCallerNotOwnerNorApproved\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ApprovalQueryForNonexistentToken\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"BalanceQueryForZeroAddress\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"CallbackExecutionReverted\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"CallbackFunctionAlreadyInstalled\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"CallbackFunctionNotSupported\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"CallbackFunctionRequired\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"CallbackFunctionUnauthorizedCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FallbackFunctionAlreadyInstalled\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FallbackFunctionNotInstalled\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"IndexOutOfBounds\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidQueryRange\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"MintERC2309QuantityExceedsLimit\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"MintToZeroAddress\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"MintZeroQuantity\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ModuleAlreadyInstalled\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"requiredInterfaceId\",\"type\":\"bytes4\"}],\"name\":\"ModuleInterfaceNotCompatible\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ModuleNotInstalled\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ModuleOutOfSync\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NewOwnerIsZeroAddress\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NoHandoverRequest\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotCompatibleWithSpotMints\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"OwnerQueryForNonexistentToken\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"OwnershipNotInitializedForExtraData\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"Reentrancy\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"SequentialMintExceedsLimit\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"SequentialUpToTooSmall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"SpotMintTokenIdTooSmall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"TokenAlreadyExists\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"TransferCallerNotOwnerNorApproved\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"TransferFromIncorrectOwner\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"TransferToNonERC721ReceiverImplementer\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"TransferToZeroAddress\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"URIQueryForNonexistentToken\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"Unauthorized\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"approved\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"fromTokenId\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"toTokenId\",\"type\":\"uint256\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"}],\"name\":\"ConsecutiveTransfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[],\"name\":\"ContractURIUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"caller\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"installedModule\",\"type\":\"address\"}],\"name\":\"ModuleInstalled\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"caller\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"address\",\"name\":\"installedModule\",\"type\":\"address\"}],\"name\":\"ModuleUninstalled\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"pendingOwner\",\"type\":\"address\"}],\"name\":\"OwnershipHandoverCanceled\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"pendingOwner\",\"type\":\"address\"}],\"name\":\"OwnershipHandoverRequested\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"oldOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"roles\",\"type\":\"uint256\"}],\"name\":\"RolesUpdated\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"stateMutability\":\"payable\",\"type\":\"fallback\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"burn\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"cancelOwnershipHandover\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"pendingOwner\",\"type\":\"address\"}],\"name\":\"completeOwnershipHandover\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"contractURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"eip712Domain\",\"outputs\":[{\"internalType\":\"bytes1\",\"name\":\"fields\",\"type\":\"bytes1\"},{\"internalType\":\"string\",\"name\":\"name\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"version\",\"type\":\"string\"},{\"internalType\":\"uint256\",\"name\":\"chainId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"verifyingContract\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"salt\",\"type\":\"bytes32\"},{\"internalType\":\"uint256[]\",\"name\":\"extensions\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"explicitOwnershipOf\",\"outputs\":[{\"components\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"uint64\",\"name\":\"startTimestamp\",\"type\":\"uint64\"},{\"internalType\":\"bool\",\"name\":\"burned\",\"type\":\"bool\"},{\"internalType\":\"uint24\",\"name\":\"extraData\",\"type\":\"uint24\"}],\"internalType\":\"struct IERC721AUpgradeable.TokenOwnership\",\"name\":\"ownership\",\"type\":\"tuple\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"tokenIds\",\"type\":\"uint256[]\"}],\"name\":\"explicitOwnershipsOf\",\"outputs\":[{\"components\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"},{\"internalType\":\"uint64\",\"name\":\"startTimestamp\",\"type\":\"uint64\"},{\"internalType\":\"bool\",\"name\":\"burned\",\"type\":\"bool\"},{\"internalType\":\"uint24\",\"name\":\"extraData\",\"type\":\"uint24\"}],\"internalType\":\"struct IERC721AUpgradeable.TokenOwnership[]\",\"name\":\"\",\"type\":\"tuple[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getInstalledModules\",\"outputs\":[{\"components\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"},{\"components\":[{\"internalType\":\"bool\",\"name\":\"registerInstallationCallback\",\"type\":\"bool\"},{\"internalType\":\"bytes4[]\",\"name\":\"requiredInterfaces\",\"type\":\"bytes4[]\"},{\"internalType\":\"bytes4[]\",\"name\":\"supportedInterfaces\",\"type\":\"bytes4[]\"},{\"components\":[{\"internalType\":\"bytes4\",\"name\":\"selector\",\"type\":\"bytes4\"}],\"internalType\":\"struct IModuleConfig.CallbackFunction[]\",\"name\":\"callbackFunctions\",\"type\":\"tuple[]\"},{\"components\":[{\"internalType\":\"bytes4\",\"name\":\"selector\",\"type\":\"bytes4\"},{\"internalType\":\"uint256\",\"name\":\"permissionBits\",\"type\":\"uint256\"}],\"internalType\":\"struct IModuleConfig.FallbackFunction[]\",\"name\":\"fallbackFunctions\",\"type\":\"tuple[]\"}],\"internalType\":\"struct IModuleConfig.ModuleConfig\",\"name\":\"config\",\"type\":\"tuple\"}],\"internalType\":\"struct ICore.InstalledModule[]\",\"name\":\"_installedModules\",\"type\":\"tuple[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getSupportedCallbackFunctions\",\"outputs\":[{\"components\":[{\"internalType\":\"bytes4\",\"name\":\"selector\",\"type\":\"bytes4\"},{\"internalType\":\"enum ICore.CallbackMode\",\"name\":\"mode\",\"type\":\"uint8\"}],\"internalType\":\"struct ICore.SupportedCallbackFunction[]\",\"name\":\"supportedCallbackFunctions\",\"type\":\"tuple[]\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"roles\",\"type\":\"uint256\"}],\"name\":\"grantRoles\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"roles\",\"type\":\"uint256\"}],\"name\":\"hasAllRoles\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"roles\",\"type\":\"uint256\"}],\"name\":\"hasAnyRole\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_name\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"_symbol\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"_contractURI\",\"type\":\"string\"},{\"internalType\":\"address\",\"name\":\"_owner\",\"type\":\"address\"},{\"internalType\":\"address[]\",\"name\":\"_modules\",\"type\":\"address[]\"},{\"internalType\":\"bytes[]\",\"name\":\"_moduleInstallData\",\"type\":\"bytes[]\"}],\"name\":\"initialize\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_module\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"installModule\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"baseURI\",\"type\":\"string\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"baseURI\",\"type\":\"string\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"},{\"internalType\":\"bytes\",\"name\":\"signature\",\"type\":\"bytes\"}],\"name\":\"mintWithSignature\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes[]\",\"name\":\"data\",\"type\":\"bytes[]\"}],\"name\":\"multicall\",\"outputs\":[{\"internalType\":\"bytes[]\",\"name\":\"\",\"type\":\"bytes[]\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"result\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"pendingOwner\",\"type\":\"address\"}],\"name\":\"ownershipHandoverExpiresAt\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"result\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"roles\",\"type\":\"uint256\"}],\"name\":\"renounceRoles\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"requestOwnershipHandover\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"roles\",\"type\":\"uint256\"}],\"name\":\"revokeRoles\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"}],\"name\":\"rolesOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"roles\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"uri\",\"type\":\"string\"}],\"name\":\"setContractURI\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"startTokenId\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"tokensOfOwner\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"start\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"stop\",\"type\":\"uint256\"}],\"name\":\"tokensOfOwnerIn\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalMinted\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"result\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_module\",\"type\":\"address\"}],\"name\":\"uninstallModule\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"}]\n\n\n\n\n ";
        private string BERA_RAID_ABI = "[{ \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"_budsAmount\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"_farmerTokenId\", \"type\": \"uint256\" }], \"name\": \"addStake\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"stakeIndex\", \"type\": \"uint256\" }], \"name\": \"boostStake\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"claimAndUnstake\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"claimRewards\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" }, { \"internalType\": \"bytes\", \"name\": \"data\", \"type\": \"bytes\" }], \"name\": \"onERC721Received\", \"outputs\": [{ \"internalType\": \"bytes4\", \"name\": \"\", \"type\": \"bytes4\" }], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" }], \"name\": \"raid\", \"outputs\": [], \"stateMutability\": \"payable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"riskLevel\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" }], \"name\": \"raidCustom\", \"outputs\": [], \"stateMutability\": \"payable\", \"type\": \"function\" }, { \"inputs\": [{ \"internalType\": \"uint256\", \"name\": \"_budsAmount\", \"type\": \"uint256\" }], \"name\": \"unStakeBuds\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"unStakeFarmer\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }]";
        private string Yeet_ABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"target\",\"type\":\"address\"}],\"name\":\"AddressEmptyCode\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"AlreadyClaimed\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"ERC1967InvalidImplementation\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ERC1967NonPayable\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"FailedCall\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InsufficientValue\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidInitialization\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NoContribution\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotInitializing\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"OwnableInvalidOwner\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"OwnableUnauthorizedAccount\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"UUPSUnauthorizedCallContext\",\"type\":\"error\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"slot\",\"type\":\"bytes32\"}],\"name\":\"UUPSUnsupportedProxiableUUID\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ZeroAddress\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint64\",\"name\":\"version\",\"type\":\"uint64\"}],\"name\":\"Initialized\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"implementation\",\"type\":\"address\"}],\"name\":\"Upgraded\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"UserYeetSubmitted\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"WonRound\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Yeeted\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"FEE_RATE\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"MIN_YEET\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"UPGRADE_INTERFACE_VERSION\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"}],\"name\":\"calculateContribution\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"}],\"name\":\"claimReward\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"}],\"name\":\"claimedRewards\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"claimed\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"currentRound\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getPotToWinner\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_yeet\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"_treasury\",\"type\":\"address\"}],\"name\":\"init\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"}],\"name\":\"pooledYeetsByRound\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"pooledYeets\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"proxiableUUID\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"}],\"name\":\"setTreasury\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"addr\",\"type\":\"address\"}],\"name\":\"setYeedAddress\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"timeRemaining\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"treasury\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newImplementation\",\"type\":\"address\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"upgradeToAndCall\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"}],\"name\":\"userYeets\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"}],\"name\":\"winningRecord\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"amountWon\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"yeet\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"yeetAddy\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"round\",\"type\":\"uint256\"}],\"name\":\"yeetSentInRound\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"totalYeetSent\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
        [Tooltip("Thirdweb SDK")]
        private string connectedAddress = "";
        private string previousConnectedAddress = "";
        private static ulong currentChain = 0;
        public InteractManager interactManager;
        public BigInteger CurrentBalance;
        private string currentRaidFee;

        [Tooltip("Api variables")]
        public TextMeshProUGUI CCIP_txn;

        private string ReadApi_BaseURL = "https://t8kmnpwzy5.execute-api.eu-west-3.amazonaws.com/dev";
        private string TransactionConfirmation_BaseURL = "https://iuss5miew4.execute-api.eu-west-3.amazonaws.com/dev";
        private string Heymint_BaseURL = "https://i5wylasq33.execute-api.eu-west-3.amazonaws.com/dev";
        private string Wormhole_BaseUrl = "https://24rya9omd6.execute-api.eu-west-3.amazonaws.com/dev";

        private string InventoryServiceAPIBase = "https://iuss5miew4.execute-api.eu-west-3.amazonaws.com/dev";
        private string UserDB_BaseUrl = "https://w8d389nd3l.execute-api.eu-west-3.amazonaws.com/dev";
        private string PATH_CreateUser = "user";
        private string PATH_GetCurrentBlockNumber = "getCurrentBlockNumber";
        private string PATH_GetAPR = "getApr";
        private string PATH_GetLocalBudsCount = "getLocalBudsCount";
        private string PATH_GetEvents = "getEvents";
        private string PATH_TotalStakedOnAllChains = "totalStakedBudsAcrossAllChains";
        private string PATH_GetRewards = "getRewards";
        private string PATH_GetStakersCount = "getStakersCount";
        private string PATH_GetChainBudsBalance = "getBudsBalance";
        private string PATH_GetFees = "getFees";
        private string PATH_GetDashboardData = "getDashboardData";
        private string PATH_GetUserStake = "getUsersStake";
        private string PATH_globalBudsCcq = "globalBudsCcq";
        private string PATH_updateChainState = "updateChainState";
        private string PATH_GetUserInventory = "getUserInventory";
        private string PATH_GetYeetRewards = "getYeetRewards";
        private string PATH_GetLivePrizePool = "getPotToWinner";
        private string PATH_GetTimeRemaining = "getTimeRemaining";
        private string PATH_GetCurrentRound = "getCurrentRound";
        private string PATH_GetAmountYeeted = "getAmountYeeted";
        private string PATH_GetPooledYeets = "getPooledYeets";

        [Tooltip("Contract Addresses")]

        public string BUDS_ADDRESS = "0x9F43e40093327697Ff92d4b353D0A7B88b0DbBb6";
        public string FARMER_ADDRESS = "0x94A620fcd803a06Bd89396AeFBcC79e357b6f5B1";
        public string STONNER_ADDRESS = "0x0555F906722627D4c2Db92FA932f03A26421E2F5";
        public string INFORMANT_ADDRESS = "0xEf9cb7bc565B40710665a39f0FB26F2DB3f7B2d3";
        public string NARC_ADDRESS = "0xdF07199321B114DbfB3cf8a1b36fcA8aDE86C00E";
        public string STAKE_ADDRESS = "0x29Bab8dfA5d950561a8a5ec47d1739C41024B7f7";
        public string FACTORY_ADDRESS = "0x9ef75aCe60Cb2A14bD05067C20D7e7009e958116";
        public string BUDS_FAUCET_ADDRESS = "0xA2526388618629fA416684f3e176eA56B57641a0";
        public string NFT_FAUCET_ADDRESS = "0xe6ee977D209cf7b635cEDc9e6EA2b4002349eA10";
        public string BERA_ADDRESS = "0x29Bab8dfA5d950561a8a5ec47d1739C41024B7f7";
        public string Chainsaw_Address = "0xA176B041bf2fDA662c9C6c8F083C28DEEbB0522b";
        public string Amulate_Address = "0xB99DD7B5E0a779C233F09704B2Cd1264e15C73bb";
        public string Yeet_Address = "0xe49B1DA0bAd3d4a1462C4720efF586c71B1a58c9";

        // These are crosschain Id [Different from Thirdweb chain Id]
        [Tooltip("Chain Ids")]
        private ulong amoy_chain = 40267;
        private ulong baseSepolia_chain = 40245;
        private ulong beraTestnet_chain = 40291;
        private ulong bscTestnet_chain = 40102;
        private ulong fuji_chain = 40106;
        private ulong arbitrum_chain = 40231;

        [Tooltip("Game Contracts")]
        private ThirdwebContract BUDS;
        private ThirdwebContract STAKE;
        private ThirdwebContract FARMER;
        private ThirdwebContract INFORMANT;
        private ThirdwebContract STONNER;
        private ThirdwebContract NARC;
        private ThirdwebContract BUDS_FAUCET;
        private ThirdwebContract NFT_FAUCET;
        private ThirdwebContract SW;
        private ThirdwebContract InventoryItemChainsaw;
        private ThirdwebContract InventoryItemAmulate;
        private ThirdwebContract BERA_RAID;
        private ThirdwebContract Yeet_Contract;

        [Tooltip("Global variables")]
        public TextMeshProUGUI BudsBalance;
        public TextMeshProUGUI BudsFaucetChainName;

        [Tooltip("Staking variables")]
        public TextMeshProUGUI STAKE_BudsBalance;
        public TextMeshProUGUI STAKE_RewardedBuds;
        public TextMeshProUGUI STAKE_StakedBalance;
        public TextMeshProUGUI STAKE_StakingAPR;
        public Toggle StakeFarmer;
        public TMP_Dropdown StakingChain;
        public TMP_InputField StakeAmt;
        public Button Stake;
        public TextMeshProUGUI StakeStatus;

        [Tooltip("Unstaking Varibles")]
        public TextMeshProUGUI UNSTAKE_StakedBalance;
        public TextMeshProUGUI UNSTAKE_RewardedBuds;
        public Toggle UnStakeFarmer;
        public TMP_InputField UnStakeAmt;
        public Button UnStakeNClaim;
        public TextMeshProUGUI UnStakeStatus;

        // Yeet Region

        [Tooltip("Yeet Panel variabless")]
        [Header("Yeet Panel variables")]
        public TextMeshProUGUI Yeet_LivePrizePool;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI AmountYeeted;
        public TextMeshProUGUI Yeet_CurrentRound;
        public TextMeshProUGUI PooledYeet;
        public TMP_InputField YeetAmtBera;
        public Button YeetButton;
        public GameObject YeetPanel;
        public TextMeshProUGUI YeetStatus;

        [Tooltip("Yeet Rewards Panel Varibles")]
        [Header("Yeet Rewards Panel Varibles")]
        public TMP_InputField RoundNumberInput;
        public Button YeetRewardsClaim;
        public GameObject ScrollContentHolder;
        public GameObject YeetRoundContent;
        public TextMeshProUGUI YeetRewardsStatus; // Previously it was UnstakeStatus
        private bool bYeetRewardsPanelOpen;

        [HideInInspector]
        [System.Serializable]
        public class YeetRewardsContent
        {
            public YeetRewardsField[] response;
        }

        [HideInInspector]
        [System.Serializable]
        public class YeetRewardsField
        {
            public int round;
            public int amountWon;
        }
        // Time Remaining
        [HideInInspector]
        [System.Serializable]
        public class YeetInfo
        {
            public string response;
        }
        private BigInteger timeRemaining;
        private float fractionalSeconds;
        //Yeet Region end

        [Tooltip("Info board")]
        [Header("Info Board")]
        public TextMeshProUGUI INFO_BaseSepoliaAPR;
        public TextMeshProUGUI INFO_BeraTestnetAPR;
        public TextMeshProUGUI INFO_AmoyAPR;
        public TextMeshProUGUI INFO_BscAPR;
        public TextMeshProUGUI INFO_FujiAPR;
        public TextMeshProUGUI INFO_ArbitrumAPR;
        [Tooltip("Staked Buds")]
        public TextMeshProUGUI INFO_BaseSepoliaStakedBuds;
        public TextMeshProUGUI INFO_BeraTestnetStakedBuds;
        public TextMeshProUGUI INFO_AmoyStakedBuds;
        public TextMeshProUGUI INFO_BscStakedBuds;
        public TextMeshProUGUI INFO_FujiStakedBuds;
        public TextMeshProUGUI INFO_ArbitrumStakedBuds;
        public TextMeshProUGUI INFO_ChadChain;
        public TextMeshProUGUI INFO_RektChain;
        public TextMeshProUGUI INFO_BakedChain;

        [Tooltip("Raid Variables")]
        public TextMeshProUGUI RAID_BaseSepoliaStakedBuds;
        public TextMeshProUGUI RAID_BeraTestnetStakedBuds;
        public TextMeshProUGUI RAID_AmoyStakedBuds;
        public TextMeshProUGUI RAID_BscStakedBuds;
        public TextMeshProUGUI RAID_FujiStakedBuds;
        public TextMeshProUGUI RAID_ArbitrumStakedBuds;
        [Tooltip("Raid Rewards")]
        public TextMeshProUGUI RAID_BaseSepoliaRaidRewards;
        public TextMeshProUGUI RAID_BeraTestnetRaidRewards;
        public TextMeshProUGUI RAID_AmoyRaidRewards;
        public TextMeshProUGUI RAID_BscRaidRewards;
        public TextMeshProUGUI RAID_FujiRaidRewards;
        public TextMeshProUGUI RAID_ArbitrumRaidRewards;
        public TMP_Dropdown RaidingChain;
        public Toggle BoostRaid;
        public Button Raid;
        public TextMeshProUGUI RaidStatus;

        [Tooltip("Burn Variables")]
        public Button BurnForStoner_B;
        public Button BurnForInformant_B;
        public TextMeshProUGUI BurnStatus;

        public class CustomException : Exception
        {
            public CustomException(string message) : base(message)
            {
            }
        }

        [Tooltip("API call results Variables")]
        [Serializable]
        public class RES_ChainAPR
        {
            public ulong apr;
        }
        [Serializable]
        public class RaidFees
        {
            public float raidFees;
        }
        [Serializable]
        public class UserStake
        {
            public string staked_Buds;
        }
        [Serializable]
        public class RES_CCIPFee
        {
            public ulong cctx_fees;
        }
        [Serializable]
        public class RES_RaidFee
        {
            public ulong raid_fees;
        }
        [System.Serializable]
        public class RES_CurrentBlockNumber
        {
            public ulong currentBlock;
        }
        [Serializable]
        public class RES_StakeResponse
        {
            public bool tx_completed;
        }
        [Serializable]
        public struct EVENT_Burn
        {
            public string boosterMinted;
            public string tokenId;
        }
        [Serializable]
        public struct Burn
        {
            public EVENT_Burn status;
        }
        [Serializable]
        public class RES_LocalBudsCount
        {
            public ulong localBudsCount;
        }
        [System.Serializable]
        public class RES_GlobalAPRs
        {
            public GlobalAPRs apr;
        }
        [System.Serializable]
        public class GlobalAPRs
        {
            //public ulong sepolia;
            //public ulong mumbai;
            public ulong amoy;
            public ulong arbSepolia;
            public ulong baseSepolia;
            public ulong bscTestnet;
            public ulong fuji;
            public ulong beraTestnet;
        }

        [Serializable]
        public class RES_GlobalStakedBuds
        {
            public LocalBudsCount localBudsCount;
        }

        [Serializable]
        public class LocalBudsCount
        {
            public ulong beraTestnet;
            public ulong amoy;
            public ulong arbSepolia;
            public ulong baseSepolia;
            public ulong bscTestnet;
            public ulong fuji;
        }
        [Serializable]
        public class RES_GlobalRaidRewards
        {
            public GlobalRaidRewards RaidRewards;
        }

        [Serializable]
        public class GlobalRaidRewards
        {
            public ulong beraTestnet;
            public ulong amoy;
            public ulong baseSepolia;
            public ulong bscTestnet;
            public ulong arbSepolia;
            public ulong fuji;
        }
        [Serializable]
        public struct EVENT_Raid
        {
            public string raider;
            public bool isSuccess;
            public bool isBoosted;
            public ulong rewardTaken;
            public long boostsUsedInLastSevenDays;
        }
        [Serializable]
        public class RES_CrossChainRaiding
        {
            public bool tx_completed;
        }
        [Serializable]
        public class RES_CCIPRaiding
        {
            public bool eventOccurred;
            public bool isSuccess;
        }

        [Serializable]
        public class RES_BakedChain
        {
            public BakedChain bakedChain;
        }
        [Serializable]
        public class RES_RektChain
        {
            public RektChain rektChain;
        }
        [Serializable]
        public class BakedChain
        {
            public string chain;
            public ulong staked;
        }
        [Serializable]
        public class RektChain
        {
            public string chain;
            public ulong raided;
        }
        [Serializable]
        public class RES_globalBudsCcq
        {
            public string bytes;
            public RES_Signature[] sig;
        }

        [Serializable]
        public class RES_Signature
        {
            public string r;
            public string s;
            public string v;
            public string guardianIndex;
        }

        [Serializable]
        public class RES_updateChainState
        {
            public bool status;
        }
        //Inventory
        [Serializable]
        public class RES_InventoryData
        {
            public RES_InventoryItems[] res_InventoryItems;
        }
        [Serializable]
        public class RES_InventoryItems
        {
            public string chainName;
            public RES_InventoryItemData[] res_InventoryItemDatas;
        }
        [Serializable]
        public class RES_InventoryItemData
        {
            public string name;
            public string address;
            public RES_Tokens[] res_tokens;
        }
        [Serializable]
        public class RES_Tokens
        {
            public int tokens;
        }
        // Create User Message
        [Serializable]
        public class RES_CreateUser
        {
            public string message;
        }

        private void Start()
        {
            if (RoundNumberInput == null) return;
            RoundNumberInput.onValueChanged.AddListener(ValidateInput);
        }

        private void Update()
        {
            // Timer panel in Yeet Rewards Panel
            if (!bYeetRewardsPanelOpen)
            {
                if (timeRemaining > 0)
                {
                    fractionalSeconds -= Time.deltaTime;

                    if (fractionalSeconds <= 0)
                    {
                        timeRemaining--;
                        fractionalSeconds = 1f;
                        UpdateTimerDisplay();
                    }
                }
            }
            else return;
        }


        public async void GetContractsAndDetails(string activeaddress, ulong activechainId)
        {
            
            BudsBalance.text = "$BUDS: 0";
            try
            {

                DebugUtils.Log("Inside GetContractsAndDetails function()");
                BigInteger _cr;
                connectedAddress = activeaddress;
                _cr = activechainId;

                if (_cr == 97)
                {
                    currentChain = bscTestnet_chain;
                }
                else if (_cr == 84532)
                {
                    currentChain = baseSepolia_chain;
                }
                else if (_cr == 80084)
                {
                    currentChain = beraTestnet_chain;
                }

                else if (_cr == 80002)
                {
                    currentChain = amoy_chain;
                }
                else if (_cr == 421614)
                {
                    currentChain = arbitrum_chain;
                }
                else
                {
                    currentChain = fuji_chain;
                }

                BUDS = await ThirdwebManager.Instance.GetContract(BUDS_ADDRESS, _cr, BUDS_ABI);
                STAKE = await ThirdwebManager.Instance.GetContract(STAKE_ADDRESS, _cr, STAKE_ABI);
                FARMER = await ThirdwebManager.Instance.GetContract(FARMER_ADDRESS, _cr, FARMER_ABI);
                INFORMANT = await ThirdwebManager.Instance.GetContract(INFORMANT_ADDRESS, _cr, INFORMANT_ABI);
                STONNER = await ThirdwebManager.Instance.GetContract(STONNER_ADDRESS, _cr, STONNER_ABI);
                NARC = await ThirdwebManager.Instance.GetContract(NARC_ADDRESS, _cr, NARC_ABI);
                GetBudsBalance();
                BUDS_FAUCET = await ThirdwebManager.Instance.GetContract(BUDS_FAUCET_ADDRESS, _cr, BUDS_FAUCET_ABI);
                NFT_FAUCET = await ThirdwebManager.Instance.GetContract(NFT_FAUCET_ADDRESS, _cr, NFT_FAUCET_ABI);
                SW = await ThirdwebManager.Instance.GetContract(connectedAddress, _cr, SMART_WALLET_ABI);
                InventoryItemChainsaw = await ThirdwebManager.Instance.GetContract(Chainsaw_Address, _cr, InventoryItemABI);
                InventoryItemAmulate = await ThirdwebManager.Instance.GetContract(Amulate_Address, _cr, InventoryItemABI);
                BERA_RAID = await ThirdwebManager.Instance.GetContract(STAKE_ADDRESS, _cr, BERA_RAID_ABI);
                Yeet_Contract = await ThirdwebManager.Instance.GetContract(Yeet_Address, _cr, Yeet_ABI);

                GetInfoDialogData();
                GetRaidData_INTERNAL();
                GetStakeDialogData();
                if (!string.Equals(previousConnectedAddress, connectedAddress) && LocalSaveManager.instance != null)
                {
                    LocalSaveManager.instance.SetWalletAddress(connectedAddress);
                    previousConnectedAddress = connectedAddress;
                }

            }
            catch (Exception ep)
            {
                DebugUtils.LogError("Error occurred on loading contracts " + ep.Message);
            }
        }

        private async Task SendRequestAsync(string apiUrl, Action<string> callback, bool isWormhole = false)
        {
            try
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
                {
                    if (isWormhole)
                    {
                        webRequest.SetRequestHeader("API-KEY", "B1q2a3z4w5s-A0p9lh-K5t6f7b-E4r5t6y");
                    }
                    webRequest.SetRequestHeader("Origin", "*");
                    await webRequest.SendWebRequest();
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        DebugUtils.Log(apiUrl);
                        callback(null);
                    }
                    else
                    {
                        string jsonResponse = webRequest.downloadHandler.text;
                        callback(jsonResponse);
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtils.LogError("Api error occurred: " + e.ToString());
            }
        }

        // Create a new User

        private async Task PostRequestAsync(string apiUrl, string userName, string accesscode, Action<string> callback)
        {
            try
            {
                DebugUtils.Log($"{{\"userAddress\":\"{connectedAddress}\",\"username\":\"{userName}\",\"access_code\":\"{accesscode}\"}}");
                using (UnityWebRequest webRequest = UnityWebRequest.Post(apiUrl, $"{{\"userAddress\":\"{connectedAddress}\",\"username\":\"{userName}\",\"access_code\":\"{accesscode}\"}}", "application/json"))
                {
                    webRequest.SetRequestHeader("x-api-key", "Wfmll4Rvqs4vSclGoL5p57JcILLyvv8X8ybcQ53e");
                    await webRequest.SendWebRequest();
                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        DebugUtils.Log(apiUrl);
                        callback(null);
                    }
                    else
                    {
                        string jsonResponse = webRequest.downloadHandler.text;
                        callback(jsonResponse);
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtils.LogError("Api error occurred: " + e.Message + "reason" + e.ToString());
            }
        }

        public async Task<string> CreateUser(string userName, string accessCode)
        {
            string message = "null";
            try
            {
                await PostRequestAsync($"{UserDB_BaseUrl}/user/", userName, accessCode, (string res) =>
                {
                    RES_CreateUser response = JsonUtility.FromJson<RES_CreateUser>(res);
                    DebugUtils.Log("The unparsed json value is:" + res);

                    if (response != null)
                    {
                        message = response.message;
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception e)
            {
                DebugUtils.LogError("Failed to get Response from function GerRaidFee" + e);
            }
            return message;
        }

        // End of creating a new User

        public string FormatNumber(BigInteger number)
        {
            if (number >= 1000000000) // More than or equal to 1 billion
            {
                double billions = (double)number / 1000000000;
                return billions.ToString("F2") + "B";
            }
            else if (number >= 1000000) // More than or equal to 1 million
            {
                double millions = (double)number / 1000000;
                return millions.ToString("F2") + "M";
            }
            else if (number >= 1000) // More than or equal to 1 thousand
            {
                double thousands = (double)number / 1000;
                return thousands.ToString("F2") + "K";
            }
            else
            {
                return number.ToString();
            }
        }

        public async void GetBudsBalance()
        {
            try
            {
                DebugUtils.Log("The connected address is:" + connectedAddress);
                BigInteger balance = await BUDS.Read<BigInteger>("balanceOf", connectedAddress);
                Canvas.ForceUpdateCanvases();

                BudsBalance.text = "$BUDS: " + FormatNumber((BigInteger)UnitConversion.Convert.FromWei(balance));
                CurrentBalance = (BigInteger)UnitConversion.Convert.FromWei(balance);
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Getting Buds balance error: " + ex.Message);
            }
        }

        private async void GetStakeDialogData()
        {
            GetBudsBalance();
            BigInteger balance = await BUDS.Read<BigInteger>("balanceOf", connectedAddress);
            STAKE_BudsBalance.text = FormatNumber((BigInteger)UnitConversion.Convert.FromWei(balance)) + " BUDS";

            string stakedBUDSamount = await GetUserStake();
            DebugUtils.Log("The staked Budsamount is:" + stakedBUDSamount);
            //Changed from rewards to getRewardsForUser
            BigInteger stake_rewarded_buds = await STAKE.Read<BigInteger>("getRewardsForUser", connectedAddress);
            RES_GlobalAPRs globalAprs = await GetGlobalAPR();
            ulong BASE_APR = 0;
            if (currentChain == baseSepolia_chain)
            {
                BASE_APR = globalAprs.apr.baseSepolia;
            }
            else if (currentChain == beraTestnet_chain)
            {
                BASE_APR = globalAprs.apr.beraTestnet;
            }
            else if (currentChain == bscTestnet_chain)
            {
                BASE_APR = globalAprs.apr.bscTestnet;
            }
            else if (currentChain == fuji_chain)
            {
                BASE_APR = globalAprs.apr.fuji;
            }
            else if (currentChain == amoy_chain)
            {
                BASE_APR = globalAprs.apr.amoy;
            }
            else
            {
                BASE_APR = globalAprs.apr.arbSepolia;
            }

            BigInteger farmerTokens = await FARMER.Read<BigInteger>("balanceOf", connectedAddress);

            STAKE_StakingAPR.text = (BASE_APR / 100.0f).ToString() + " %";

            if (stake_rewarded_buds != 0)
            {
                STAKE_RewardedBuds.text = FormatNumber((BigInteger)UnitConversion.Convert.FromWei(stake_rewarded_buds)).ToString() + " BUDS";
            }
            else
            {
                STAKE_RewardedBuds.text = "0";
            }

            STAKE_StakedBalance.text = stakedBUDSamount + " BUDS";
        }

        public void OpenStakeDialog()
        {
            StakeFarmer.interactable = true;
            StakeAmt.interactable = true;
            StakingChain.interactable = true;
            Stake.interactable = false;
            interactManager.FarmIn();
            try
            {
                GetStakeDialogData();
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("OpenStakeDialog error: " + ex.Message);
            }
            Stake.interactable = true;
        }

        public void StakeDialogUnInteractable()
        {
            StakeFarmer.interactable = false;
            StakingChain.interactable = false;
            StakeAmt.interactable = false;
            Stake.interactable = false;
        }

        public void StakeDialogInteractable()
        {
            StakeFarmer.interactable = true;
            StakingChain.interactable = true;
            StakeAmt.interactable = true;
            Stake.interactable = true;
        }

        public ulong GetStakeAmountFromInput(string parseText)
        {
            if (ulong.TryParse(parseText, out ulong number))
            {
                return number;
            }
            else
            {
                Debug.LogError("Invalid input: " + StakeAmt.text);
                return 0;
            }
        }

        public ulong GetStakeChainCCIPId()
        {
            int selectedOptionIndex = StakingChain.value;
            string selectedOption = StakingChain.options[selectedOptionIndex].text;
            return GetChainIdFromFormalChainName(selectedOption);
        }

        public async void StakeBuds()
        {
            StakeDialogUnInteractable();

            // add interactManager scripts here
            interactManager.FarmStakeStart();

            ulong stakeAmount = GetStakeAmountFromInput(StakeAmt.text);
            string chainName = GetParameterChainNameFromChainNameId(currentChain);
            ulong desiredChain = GetStakeChainCCIPId();
            ulong farmerTokenID = 0;

            if (stakeAmount <= 0 && !StakeFarmer.isOn)
            {
                StakeDialogInteractable();
                return;
            }
            DebugUtils.Log("Started staking: " + stakeAmount);

            // Querying for Approval
            int IsApproved = await CheckForAllowance(stakeAmount);
            if (IsApproved == 0)
            {
                if (StakeFarmer.isOn)
                {
                    StakeStatus.text = "Sending your farmer the go-sign";
                    farmerTokenID = await ApproveTheFarmer();
                    if (farmerTokenID == 0)
                    {
                        TransactionTracker.Instance.CallingEndTransaction("approveFarmer", false);
                        return;
                    }
                    TransactionTracker.Instance.CallingEndTransaction("approveFarmer", true);
                    DebugUtils.Log(StakeStatus.text);
                }
                if (stakeAmount > 0)
                {
                    StakeStatus.text = "Hang tight while we prepare your $BUDS for transfer";
                    uint result = await ApproveTheBuds(stakeAmount);
                    if (result == 0)
                    {
                        TransactionTracker.Instance.CallingEndTransaction("approveBuds", false);
                        return;
                    }
                    TransactionTracker.Instance.CallingEndTransaction("approveBuds", true);
                    DebugUtils.Log(StakeStatus.text);
                }
            }
            //End of Querying

            StakeStatus.text = "Your $BUDS are ready to ship to " + GetFormalNameFromChainNameId(desiredChain);
            DebugUtils.Log(StakeStatus.text + " " + currentChain + " " + desiredChain);
            if (currentChain == desiredChain)
            {
                DebugUtils.Log("Current chain stake");
                uint result = await StakeBudsInCurrentChain(stakeAmount, farmerTokenID);
                if (result == 0)
                {
                    TransactionTracker.Instance.CallingEndTransaction("AddStakeCurr", false);
                    return;
                }
                TransactionTracker.Instance.CallingEndTransaction("AddStakeCurr", true);
            }
            else
            {
                DebugUtils.Log("cross chain stake");
                uint result = await StakeBudsCrossChain(chainName, stakeAmount, farmerTokenID, desiredChain);
                if (result == 0)
                {
                    TransactionTracker.Instance.CallingEndTransaction("crossChainStake", false);
                    return;
                }
                TransactionTracker.Instance.CallingEndTransaction("crossChainStake", true);
            }
            StakeDialogInteractable();
            StakeAmt.text = "";
        }

        private async Task<int> CheckForAllowance(ulong stakeAmount)
        {
            try
            {
                ulong allowanceResponse = await BUDS.Read<ulong>("allowance", connectedAddress, STAKE_ADDRESS);
                DebugUtils.Log("The allowance response is:" + allowanceResponse);
                DebugUtils.Log("The stake amount is:" + stakeAmount);

                DebugUtils.LogWarning(string.Equals(allowanceResponse.ToString(), stakeAmount.ToString()));
                if (!string.Equals(allowanceResponse.ToString(), stakeAmount.ToString()))
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                DebugUtils.LogError("The error is:" + e.Message);
                return 0;
            }
            return 1;
        }

        public async Task<ulong> ApproveTheFarmer()
        {
            ulong farmerTokenId = 0;
            try
            {

                farmerTokenId = await FARMER.Read<ulong>("tokenOfOwnerByIndex", connectedAddress, "0");
                DebugUtils.Log("farmerTokenId: " + farmerTokenId);
                TransactionTracker.Instance.AddTransaction("approveFarmer", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var FarmerApprovalResult = await FARMER.Write("approve", new TransactionRequest { gasLimit = "1500000" }, STAKE_ADDRESS, farmerTokenId.ToString());
                var farmerApproval = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, FARMER, "approve", 0, STAKE_ADDRESS, farmerTokenId.ToString());
                DebugUtils.Log("farmerApprovalResult: " + farmerApproval);
                //if (string.IsNullOrEmpty(FarmerApprovalResult.IsFaulted)))

                if (string.IsNullOrEmpty(farmerApproval.TransactionHash))
                {
                    StakeStatus.text = "Farmer approval";
                    return 0;
                }
            }
            catch (Exception e)
            {
                StakeStatus.text = "ERROR: Farmer approval";
                DebugUtils.Log("CATCH: farmer approval error " + e.Message);
                return 0;
            }
            return farmerTokenId;
        }

        public async Task<uint> ApproveTheBuds(ulong approveAmount)
        {
            try
            {
                TransactionTracker.Instance.AddTransaction("approveBuds", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var approvalResult = await BUDS.Write("approve", new TransactionRequest { gasLimit = "1500000" }, STAKE_ADDRESS, UnitConversion.Convert.ToWei(approveAmount.ToString(), UnitConversion.EthUnit.Ether).ToString());
                var approvalResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, BUDS, "approve", 0, STAKE_ADDRESS, UnitConversion.Convert.ToWei(approveAmount.ToString(), UnitConversion.EthUnit.Ether).ToString());
                if (string.IsNullOrEmpty(approvalResult.TransactionHash))
                {
                    StakeStatus.text = "Buds approval";
                    return 0;
                }
            }
            catch (Exception e)
            {
                StakeStatus.text = "ERROR: Buds approval";
                DebugUtils.LogError("CATCH: Buds approval error " + e.Message);
                return 0;
            }
            return 1;
        }

        public async Task<uint> StakeBudsInCurrentChain(ulong _stakeAmount, ulong _farmerID)
        {
            try
            {
                TransactionTracker.Instance.AddTransaction("AddStakeCurr", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                var normalStakeResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "addStake", 0, UnitConversion.Convert.ToWei(_stakeAmount.ToString(), UnitConversion.EthUnit.Ether), _farmerID.ToString());

                if (normalStakeResult.TransactionHash.Length > 0)
                {
                    StakeStatus.text = "Your $BUDS have reached the farm! \n Don't forget to check your yield and claim your rewards.";
                    GetStakeDialogData();
                    return 1;
                }
                else
                {
                    StakeStatus.text = "Damn, it seems your $BUDS didn't make it to the farm! \n Try again or drop us a message on Discord for help";
                    return 0;
                }
            }
            catch (Exception e)
            {
                StakeStatus.text = "Damn, it seems your $BUDS didn't make it to the farm! \n Try again or drop us a message on Discord for help";
                DebugUtils.LogError("CATCH: Normal Stake error " + e.Message);
                return 0;
            }
        }

        public async Task<ulong> GetCCIPFees(string _currentChain, string _stakeAmount, string _farmerTokenId, ulong _desiredChainSelector)
        {
            ulong ccipFees = 0;
            try
            {
                //getFees /:networkName /:budsAmount /:tokenId /:userAddress /:destEid
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetFees}/{_currentChain}/{_stakeAmount}/{_farmerTokenId}/{connectedAddress}/{_desiredChainSelector}", (string res) =>
                {
                    RES_CCIPFee response = JsonUtility.FromJson<RES_CCIPFee>(res);
                    DebugUtils.Log("The unparsed json value is:" + res);
                    DebugUtils.Log("The response is:" + response.cctx_fees);

                    if (response != null)
                    {
                        ccipFees = response.cctx_fees;
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception e)
            {
                DebugUtils.LogError("CCIP fee getting error:" + e);
            }
            return ccipFees;
        }

        public async Task<BigInteger> GetCurrentBlockNumber(string _chain)
        {
            BigInteger currentBlock;
            try
            {
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetCurrentBlockNumber}/{_chain}", (string res) =>
                {
                    RES_CurrentBlockNumber response = JsonUtility.FromJson<RES_CurrentBlockNumber>(res);
                    if (response != null)
                    {
                        currentBlock = response.currentBlock;
                        if (_chain == "bscTestnet")
                        {
                            currentBlock -= 30;
                        }
                        DebugUtils.Log("Current Block: " + currentBlock);
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception e)
            {
                DebugUtils.LogError("ERROR getting currecnt block number " + e.Message);
            }
            return currentBlock;
        }

        public static void InitUniTaskLoop()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            Cysharp.Threading.Tasks.PlayerLoopHelper.Initialize(ref loop);
        }

        public async Task<ulong> delayOneMin()
        {
            InitUniTaskLoop();
            await UniTask.Delay(TimeSpan.FromSeconds(30));
            return 0;
        }

        //Cross Chain
        public async Task<uint> ListenForStakeConfirmation(string _chainName, string transactionHash)
        {
            int num_of_loops = 0;
            bool shouldBreak = false;
            try
            {
                while (!shouldBreak)
                {
                    try
                    {
                        DebugUtils.Log("Waiting for 30 seconds");
                        await delayOneMin();
                        DebugUtils.Log("30 seconds done, Started Listening");
                        num_of_loops++;
                        if (num_of_loops >= 6)
                        {
                            shouldBreak = true;
                            throw new CustomException("TX DONE");
                        }

                        RES_StakeResponse response = new RES_StakeResponse();
                        //await SendRequestAsync($"{TransactionConfirmation_BaseURL}/{PATH_GetEvents}/{_chainName}/stake/{_startBlock}/{connectedAddress}", (string res) =>
                        await SendRequestAsync($"{TransactionConfirmation_BaseURL}/getStatus/{_chainName}/{transactionHash}", (string res) =>
                        {
                            response = JsonUtility.FromJson<RES_StakeResponse>(res);
                            DebugUtils.Log("Stake event: " + response);
                            DebugUtils.Log("Stake event: " + response.tx_completed);
                        });
                        if (response.tx_completed == true)
                        {
                            shouldBreak = true;
                            TransactionTracker.Instance.CallingEndTransaction("crossChainStake", true);
                            throw new CustomException("TX DONE");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugUtils.LogError("The reason for exception is " + ex.Message);
                        return 0;
                    }
                }
            }
            catch (CustomException ex)
            {
                DebugUtils.LogError("TX MONITORING DONE with message: " + ex.Message);
            }
            return 1;
        }

        public void OpenCCIPTxn()
        {
            //Application.OpenURL(CCIPTxnLink);
        }

        public async Task<uint> StakeBudsCrossChain(string _currentChain, ulong _stakeAmount, ulong _farmerTokenId, ulong _desiredChainSelector)
        {
            try
            {
                string stakeAmountString = UnitConversion.Convert.ToWei(_stakeAmount.ToString(), UnitConversion.EthUnit.Ether).ToString();

                ulong ccipFees = await GetCCIPFees(_currentChain, stakeAmountString, _farmerTokenId.ToString(), _desiredChainSelector);

                if (ccipFees == 0) return 0;

                TransactionTracker.Instance.AddTransaction("crossChainStake", GetFormalNameFromChainNameId(currentChain), GetFormalNameFromChainNameId(_desiredChainSelector), 70f);

                object[] parametersObjects = { stakeAmountString, _farmerTokenId.ToString(), $"{_desiredChainSelector}" };

                var crossChainStake = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "crossChainStake", ccipFees, parametersObjects);


                if (crossChainStake.TransactionHash.Length > 0)
                {
                    StakeStatus.text = "$BUDS are en route. Light one up while it gets there.\n TIP: You can close this menu and track your transactions anytime by pressing the TAB button";

                    string desiredChainName = GetParameterChainNameFromChainNameId(_desiredChainSelector);

                    uint result = await ListenForStakeConfirmation(desiredChainName, crossChainStake.TransactionHash);
                    if (result == 0) return 0;
                    StakeStatus.text = "Your $BUDS have reached the farm! \n Don't forget to check your yield and claim your rewards.";
                }
                else
                {
                    StakeStatus.text = "Damn, it seems your $BUDS didn't make it to the farm! \n Please check if you've got enough gas for transport and try again.";
                    return 0;
                }
            }
            catch (Exception e)
            {
                StakeStatus.text = "Damn, it seems your $BUDS didn't make it to the farm! \n Please check if you've got enough gas for transport and try again.";
                DebugUtils.LogError("Crosschain stake error: " + e.Message);
                return 0;
            }

            CCIP_txn.text = "";

            return 1;
        }

        public async void OpenUnStakeDialog()
        {
            UnStakeNClaim.interactable = true;
            UnStakeAmt.interactable = true;
            try
            {
                GetBudsBalance();

                BigInteger stake_rewarded_buds = await STAKE.Read<BigInteger>("getRewardsForUser", connectedAddress);

                string stakedBUDSamount = await GetUserStake();

                BigInteger farmerTokens = await FARMER.Read<BigInteger>("balanceOf", connectedAddress);

                if (stake_rewarded_buds != 0)
                {
                    UNSTAKE_RewardedBuds.text = FormatNumber((BigInteger)UnitConversion.Convert.FromWei(stake_rewarded_buds)).ToString() + " BUDS";
                }

                if (string.Equals(stakedBUDSamount, "0"))
                {
                    UnStakeNClaim.interactable = false;
                    UnStakeAmt.interactable = false;
                }
                else
                {
                    UNSTAKE_StakedBalance.text = stakedBUDSamount + " BUDS";
                }
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Unstake open error " + ex.Message);

            }
        }

        public async Task<string> GetUserStake()
        {
            UserStake response = new UserStake();
            try
            {
                DebugUtils.Log($"{ReadApi_BaseURL}/{PATH_GetUserStake}/{GetParameterChainNameFromChainNameId(currentChain)}/{connectedAddress}");
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetUserStake}/{GetParameterChainNameFromChainNameId(currentChain)}/{connectedAddress}", (string res) =>
                {
                    response = JsonUtility.FromJson<UserStake>(res);
                    DebugUtils.Log("The response is :" + res);
                    DebugUtils.Log("The response data is:" + response.staked_Buds);
                });
            }
            catch (Exception e)
            {
                DebugUtils.LogError("Failed to get Response from function GerRaidFee" + e);
            }


            return response.staked_Buds;
        }

        public async void UnStakeTheFarmer()
        {
            UnStakeFarmer.interactable = false;
            UnStakeAmt.interactable = false;
            UnStakeNClaim.interactable = false;

            UnStakeStatus.text = "WAITING: Txn approval";

            try
            {
                TransactionTracker.Instance.AddTransaction("UnstakeFarmer", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var unstakeResult = await STAKE.Write("unStakeFarmer", new TransactionRequest { gasLimit = "1500000" });
                var unstakeResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "unStakeFarmer",0);
                DebugUtils.Log(unstakeResult.TransactionHash);

                if (!string.IsNullOrEmpty(unstakeResult.TransactionHash))
                {
                    TransactionTracker.Instance.CallingEndTransaction("UnstakeFarmer", true);
                    UnStakeStatus.text = "SUCCESS: Farmer unstake";

                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("UnstakeFarmer", false);
                    UnStakeStatus.text = "ERROR: Farmer unstake";
                }
            }
            catch (Exception ex)
            {
                TransactionTracker.Instance.CallingEndTransaction("UnstakeFarmer", false);
                UnStakeStatus.text = "ERROR: Farmer unstake";
                DebugUtils.LogError("Farmer unstake error: " + ex.Message);
            }

            UnStakeFarmer.interactable = true;
            UnStakeAmt.interactable = true;
            UnStakeNClaim.interactable = true;
            OpenUnStakeDialog();
        }

        public async void UnStakeTheBuds()
        {
            UnStakeFarmer.interactable = false;
            UnStakeAmt.interactable = false;
            UnStakeNClaim.interactable = false;

            UnStakeStatus.text = "WAITING: Txn approval";

            try
            {
                TransactionTracker.Instance.AddTransaction("UnstakeBuds", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                object[] UnstakeParams = { UnitConversion.Convert.ToWei(UnStakeAmt.text, UnitConversion.EthUnit.Ether).ToString() };

                var unstakeResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "unStakeBuds", 0, UnstakeParams);

                if (unstakeResult.TransactionHash.Length > 0)
                {
                    TransactionTracker.Instance.CallingEndTransaction("UnstakeBuds", true);
                    UnStakeStatus.text = "SUCCESS: Unstake";
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("UnstakeBuds", false);
                    UnStakeStatus.text = "ERROR: Unstake";
                }
            }
            catch (Exception ex)
            {
                TransactionTracker.Instance.CallingEndTransaction("UnstakeBuds", false);
                UnStakeStatus.text = "ERROR: Unstake";
                DebugUtils.LogError("Unstake buds error: " + ex.Message);
            }

            UnStakeFarmer.interactable = true;
            UnStakeAmt.interactable = true;
            UnStakeNClaim.interactable = true;
            OpenUnStakeDialog();
        }

        public async void ClaimTheRewards()
        {
            UnStakeFarmer.interactable = false;
            UnStakeAmt.interactable = false;
            UnStakeNClaim.interactable = false;

            UnStakeStatus.text = "WAITING: Txn approval";

            try
            {
                RES_globalBudsCcq result = await ClaimRewardsWormholeAPI();
                DebugUtils.Log("The result.bytes:" + result.bytes);
                DebugUtils.Log("The result.sig:" + result.sig);
                byte[] byteArray = ConvertHexStringToByteArray(result.bytes);

                object[] claimRewardsParameters = { byteArray, result.sig };

                TransactionTracker.Instance.AddTransaction("claimRewards", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                var claimResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "claimRewards", 0,claimRewardsParameters);

                if (claimResult.TransactionHash.Length > 0)
                {
                    TransactionTracker.Instance.CallingEndTransaction("claimRewards", true);
                    UnStakeStatus.text = "Claiming buds rewards";
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("claimRewards", false);
                    UnStakeStatus.text = "Unsuccessful Claim";
                    return;
                }

            }
            catch (Exception ex)
            {
                TransactionTracker.Instance.CallingEndTransaction("claimRewards", false);
                DebugUtils.LogError(" Claim rewards error: " + ex.ToString());
            }

            UnStakeFarmer.interactable = true;
            UnStakeAmt.interactable = true;
            UnStakeNClaim.interactable = true;
            OpenUnStakeDialog();
        }

        public byte[] ConvertHexStringToByteArray(string hex)
        {
            // Remove "0x" prefix if present
            if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hex = hex.Substring(2);
            }

            // Make sure the hex string has an even number of characters
            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        public async Task<RES_globalBudsCcq> ClaimRewardsWormholeAPI()
        {
            RES_globalBudsCcq response = new RES_globalBudsCcq();
            try
            {
                DebugUtils.Log("the current chain is:" + currentChain);
                await SendRequestAsync($"{Wormhole_BaseUrl}/{PATH_globalBudsCcq}/{currentChain}", (string res) =>
                {
                    response = JsonUtility.FromJson<RES_globalBudsCcq>(res);
                    DebugUtils.Log("The response is :" + res);
                    DebugUtils.Log("The response data is:" + response);
                }, true);
            }
            catch (Exception e)
            {
                DebugUtils.Log("The error is:" + e);
            }
            return response;
        }

        public async void Unstake()
        {
            UnStakeFarmer.interactable = false;
            UnStakeNClaim.interactable = false;
            UnStakeAmt.interactable = false;
            try
            {
                if (UnStakeFarmer.isOn == true)
                {
                    DebugUtils.Log("Unstaking farmer");
                    UnStakeTheFarmer();
                }
                DebugUtils.Log("Unstaking buds");
                UnStakeTheBuds();
            }
            catch (Exception e)
            {
                DebugUtils.Log("Unstake error occurred");
            }
            UnStakeFarmer.interactable = true;
            UnStakeNClaim.interactable = true;
            UnStakeAmt.interactable = true;
        }
        public async Task<BigInteger> GetStakedBuds(string chainName)
        {
            BigInteger budsStaked;
            try
            {
                await SendRequestAsync($"{Heymint_BaseURL}/{PATH_GetUserStake}/{chainName}/{connectedAddress}", (string res) =>
                {
                    RES_LocalBudsCount response = JsonUtility.FromJson<RES_LocalBudsCount>(res);
                    if (response != null)
                    {
                        budsStaked = response.localBudsCount;
                    }
                    else
                    {
                        Debug.LogError("Failed to deserialize JSON response in GetStakedBuds()");
                    }
                });
            }
            catch (Exception e)
            {
                DebugUtils.Log("GetStakedBuds error: " + e.Message);
            }
            return budsStaked;
        }

        public async Task<RES_GlobalAPRs> GetGlobalAPR()
        {
            DebugUtils.Log("This is working");
            RES_GlobalAPRs response = new RES_GlobalAPRs();
            await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetAPR}", (string res) =>
            {
                response.apr = JsonUtility.FromJson<GlobalAPRs>(res);
                DebugUtils.Log("global aprs res: " + res);
                if (response == null)
                {
                    Debug.LogError("Failed to deserialize JSON response: GetGlobalAPR()");
                }
            });
            return response;
        }

        public async Task<RES_GlobalStakedBuds> GetGlobalStakedBuds()
        {
            RES_GlobalStakedBuds response = new RES_GlobalStakedBuds();
            DebugUtils.Log("Getting global buds data");
            await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetLocalBudsCount}", (string res) =>
            {
                response.localBudsCount = JsonUtility.FromJson<LocalBudsCount>(res);
                DebugUtils.Log("Response: " + res);
                if (response == null)
                {
                    Debug.LogError("Failed to deserialize JSON response: GetGlobalAPR()");
                }
            });
            return response;
        }

        public async void GetInfoDialogData()
        {
            RES_GlobalStakedBuds globalBudsCount = await GetGlobalStakedBuds();

            RES_GlobalAPRs globalAprs = await GetGlobalAPR();

            if (globalAprs.apr == null)
            {
                INFO_BeraTestnetAPR.text = UnityEngine.Random.Range(0, 100).ToString() + " %";
                INFO_AmoyAPR.text = UnityEngine.Random.Range(0, 100).ToString() + " %";
                INFO_BaseSepoliaAPR.text = UnityEngine.Random.Range(0, 100).ToString() + "%";
                INFO_AmoyAPR.text = UnityEngine.Random.Range(0, 100).ToString() + " %";
                INFO_BscAPR.text = UnityEngine.Random.Range(0, 100).ToString() + " %";
                INFO_FujiAPR.text = UnityEngine.Random.Range(0, 100).ToString() + " %";
                INFO_ArbitrumAPR.text = UnityEngine.Random.Range(0, 100).ToString() + "%";
            }
            else
            {
                INFO_BeraTestnetAPR.text = (globalAprs.apr.beraTestnet / 100).ToString() + " %";
                INFO_AmoyAPR.text = (globalAprs.apr.amoy / 100).ToString() + " %";
                INFO_BaseSepoliaAPR.text = (globalAprs.apr.baseSepolia / 100).ToString() + "%";
                INFO_AmoyAPR.text = (globalAprs.apr.amoy / 100).ToString() + " %";
                INFO_BscAPR.text = (globalAprs.apr.bscTestnet / 100).ToString() + " %";
                INFO_FujiAPR.text = (globalAprs.apr.fuji / 100).ToString() + " %";
                INFO_ArbitrumAPR.text = (globalAprs.apr.arbSepolia / 100).ToString() + " %";
            }

            if (globalBudsCount.localBudsCount == null)
            {
                INFO_BaseSepoliaStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                INFO_BeraTestnetStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                INFO_AmoyStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                INFO_BscStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                INFO_FujiStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                INFO_ArbitrumStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
            }
            else
            {
                INFO_BaseSepoliaStakedBuds.text = FormatNumber(globalBudsCount.localBudsCount.baseSepolia).ToString();
                INFO_BeraTestnetStakedBuds.text = FormatNumber(globalBudsCount.localBudsCount.beraTestnet).ToString();
                INFO_AmoyStakedBuds.text = FormatNumber(globalBudsCount.localBudsCount.amoy).ToString();
                INFO_BscStakedBuds.text = FormatNumber(globalBudsCount.localBudsCount.bscTestnet).ToString();
                INFO_FujiStakedBuds.text = FormatNumber(globalBudsCount.localBudsCount.fuji).ToString();
                INFO_ArbitrumStakedBuds.text = FormatNumber(globalBudsCount.localBudsCount.arbSepolia).ToString();
            }
        }

        public async Task<RES_GlobalRaidRewards> GetGlobalRaidRewards()
        {
            DebugUtils.Log("Current Chain" + currentChain);
            DebugUtils.Log("Connected Address" + connectedAddress);
            RES_GlobalRaidRewards response = new RES_GlobalRaidRewards();
            DebugUtils.Log(connectedAddress);
            await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetRewards}/{connectedAddress}", (string res) =>
            {
                response.RaidRewards = JsonUtility.FromJson<GlobalRaidRewards>(res);
                DebugUtils.Log("Raid Rewards" + res);
                if (response == null)
                {
                    Debug.LogError("Failed to deserialize JSON response: GetGlobalRaidRewards()");
                }
            });
            return response;
        }

        private async void GetRaidData_INTERNAL()
        {
            Raid.interactable = false;
            DebugUtils.Log("Getting raid data");
            RES_GlobalStakedBuds globalStakedBuds = await GetGlobalStakedBuds();
            DebugUtils.Log("1/2 Getting raid data");
            RES_GlobalRaidRewards globalRaidRewards = await GetGlobalRaidRewards();
            DebugUtils.Log("DONE Getting raid data");

            if (globalStakedBuds.localBudsCount == null)
            {
                RAID_BaseSepoliaStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                RAID_BeraTestnetStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                RAID_AmoyStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                RAID_BscStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                RAID_FujiStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
                RAID_ArbitrumStakedBuds.text = UnityEngine.Random.Range(10000, 30000).ToString();
            }
            else
            {

                RAID_BaseSepoliaStakedBuds.text = FormatNumber(globalStakedBuds.localBudsCount.baseSepolia).ToString();
                RAID_BeraTestnetStakedBuds.text = FormatNumber(globalStakedBuds.localBudsCount.beraTestnet).ToString();
                RAID_AmoyStakedBuds.text = FormatNumber(globalStakedBuds.localBudsCount.amoy).ToString();
                RAID_BscStakedBuds.text = FormatNumber(globalStakedBuds.localBudsCount.bscTestnet).ToString();
                RAID_FujiStakedBuds.text = FormatNumber(globalStakedBuds.localBudsCount.fuji).ToString();
                RAID_ArbitrumStakedBuds.text = FormatNumber(globalStakedBuds.localBudsCount.arbSepolia).ToString();
            }

            if (globalRaidRewards.RaidRewards == null)
            {
                RAID_BaseSepoliaRaidRewards.text = (UnityEngine.Random.Range(10000, 30000) * 0.001f).ToString();
                RAID_BeraTestnetRaidRewards.text = (UnityEngine.Random.Range(10000, 30000) * 0.001f).ToString();
                RAID_AmoyRaidRewards.text = (UnityEngine.Random.Range(10000, 30000) * 0.001f).ToString();
                RAID_BscRaidRewards.text = (UnityEngine.Random.Range(10000, 30000) * 0.001f).ToString();
                RAID_FujiRaidRewards.text = (UnityEngine.Random.Range(10000, 30000) * 0.001f).ToString();
                RAID_ArbitrumRaidRewards.text = (UnityEngine.Random.Range(10000, 30000) * 0.001f).ToString();

            }
            else
            {
                RAID_BaseSepoliaRaidRewards.text = (globalStakedBuds.localBudsCount.baseSepolia * (1 / 100.0f)).ToString();//FormatNumber(globalRaidRewards.RaidRewards.baseSepolia).ToString();
                RAID_BeraTestnetRaidRewards.text = (globalStakedBuds.localBudsCount.beraTestnet * (1 / 100.0f)).ToString();//FormatNumber(globalRaidRewards.RaidRewards.beraTestnet).ToString();
                RAID_AmoyRaidRewards.text = (globalStakedBuds.localBudsCount.amoy * (1 / 100.0f)).ToString();//FormatNumber(globalRaidRewards.RaidRewards.amoy).ToString();
                RAID_BscRaidRewards.text = (globalStakedBuds.localBudsCount.bscTestnet * (1 / 100.0f)).ToString();//FormatNumber(globalRaidRewards.RaidRewards.bscTestnet).ToString();
                RAID_FujiRaidRewards.text = (globalStakedBuds.localBudsCount.fuji * (1 / 100.0f)).ToString();//FormatNumber(globalRaidRewards.RaidRewards.fuji).ToString();
                RAID_ArbitrumRaidRewards.text = (globalStakedBuds.localBudsCount.arbSepolia * (1 / 100.0f)).ToString();//FormatNumber(globalRaidRewards.RaidRewards.arbSepolia).ToString();
            }

            BigInteger informantTokens = await INFORMANT.Read<BigInteger>("balanceOf", connectedAddress);
            BigInteger narcTokens = await NARC.Read<BigInteger>("balanceOf", connectedAddress);

            if (informantTokens != 0)
            {
                BoostRaid.interactable = true;
            }

            //No Conition on Raid 
            Raid.interactable = true;
            //if (narcTokens != 0)
            //{
            //    Raid.interactable = true;
            //}
        }

        public async void GetRaidData()
        {
            interactManager.PoliceIn();
            BoostRaid.interactable = false;
            BoostRaid.isOn = false;

            GetRaidData_INTERNAL();
        }

        public ulong GetRaidChainCCIPId()
        {
            int selectedOptionIndex = RaidingChain.value;
            string selectedOption = RaidingChain.options[selectedOptionIndex].text;

            return GetChainIdFromFormalChainName(selectedOption);
        }

        public async Task<ulong> GetRaidFee()
        {
            //Here there will be an Endpoint for API

            DebugUtils.Log("Trying Send Request GetRaid Fees");
            ulong currentRaidFee = 0;
            try
            {
                DebugUtils.Log(GetParameterChainNameFromChainNameId(currentChain));
                await SendRequestAsync($"{ReadApi_BaseURL}/getRaidFees/{GetParameterChainNameFromChainNameId(currentChain)}", (string res) =>
                {
                    RES_RaidFee response = JsonUtility.FromJson<RES_RaidFee>(res);
                    DebugUtils.Log("The unparsed json value is:" + res);
                    DebugUtils.Log("The response is:" + response.raid_fees);

                    if (response != null)
                    {
                        currentRaidFee = response.raid_fees;
                    }
                    else
                    {
                        Debug.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception e)
            {
                DebugUtils.Log("Failed to get Response from function GerRaidFee" + e);
            }
            return currentRaidFee;
        }

        public async Task<string> ApproveInformant()
        {
            string informantTokenId = await INFORMANT.Read<string>("tokenOfOwnerByIndex", connectedAddress, 0);
            DebugUtils.Log("informantTokenId: " + informantTokenId);
            RaidStatus.text = "Activating informant to acquire intel for raid";
            //var InformantApprovalResult = await INFORMANT.Write("approve", new TransactionRequest { gasLimit = "1500000" }, STAKE_ADDRESS, informantTokenId);
            var InformantApprovalResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, INFORMANT, "approve", 0, STAKE_ADDRESS, informantTokenId);
            DebugUtils.Log("farmerApprovalResult: " + InformantApprovalResult);
            if (InformantApprovalResult.TransactionHash.Length > 0)
            {
                RaidStatus.text = "ERROR: Informant approval";
                return "0";
            }
            return informantTokenId.ToString();
        }

        #region crossChainRaidTransactionMonitoring
        public async Task<ulong> MonitorCrossChainRaid(ulong _desiredChainSelector, string TransactionHash)
        {
            int num_of_loops = 0;
            string desiredChainName = GetParameterChainNameFromChainNameId(_desiredChainSelector);
            bool shouldBreak = false;
            try
            {
                while (!shouldBreak)
                {
                    try
                    { 
                        DebugUtils.Log("Waiting for 30 seconds");
                        await delayOneMin();
                        DebugUtils.Log("30 seconds done, next call!");
                        DebugUtils.Log("Started Listening");
                        num_of_loops++;
                        if (num_of_loops >= 6)
                        {
                            RaidStatus.text = "Deal unsuccessful";
                            shouldBreak = true;
                            throw new CustomException("Breaking loop Transaction monitoring done but Deal Unsuccessful");
                        }

                        RES_CrossChainRaiding response = new RES_CrossChainRaiding();
                        await SendRequestAsync($"{TransactionConfirmation_BaseURL}/getStatus/{desiredChainName}/{TransactionHash}", (string res) =>
                        {
                            response = JsonUtility.FromJson<RES_CrossChainRaiding>(res);
                            DebugUtils.Log("Raid result: " + response);
                        });
                        DebugUtils.Log("Raid resultisSuccess: " + response.tx_completed);
                        if (response?.tx_completed == true)
                        {
                            RaidStatus.text = "Raid successful! The task force sezied " + GetRaidRewards(GetFormalNameFromChainName(desiredChainName)) + " BUDS from an illicit farm on " + GetFormalNameFromChainName(desiredChainName);
                            DebugUtils.Log("breaking loop");
                            shouldBreak = true;
                            TransactionTracker.Instance.CallingEndTransaction("crossChainRaid", true);
                            throw new CustomException("Transaction monitoring done");

                        }
                    }
                    catch (CustomException ex)
                    {
                        DebugUtils.Log("TX MONITORING DONE with exception" + ex.Message);
                    }
                }
            }
            catch (CustomException ex)
            {
                RaidStatus.text = "Error 420: The game logic be tripping. \n Please try again or open a support ticket on our Discord";
                DebugUtils.Log($"Error in ccip raid listening: {ex.Message}");
                TransactionTracker.Instance.CallingEndTransaction("crossChainRaid", false);
                return 0;
            }
            return 1;
        }
        #endregion

        #region SameChainRaid
        public async Task<ulong> MonitorRaid(ulong _desiredChainSelector, BigInteger blockNumber)
        {
            string desiredChainName = GetParameterChainNameFromChainNameId(_desiredChainSelector);

            bool shouldBreak = false;
            int num_of_loops = 0;
            try
            {
                while (!shouldBreak)
                {
                    try
                    { 
                        DebugUtils.Log("Waiting for 30 seconds");
                        await delayOneMin();
                        DebugUtils.Log("30s done, next call!");
                        num_of_loops++;
                        //if (num_of_loops % 2 == 0)
                        //{
                        //    blockNumber += 50;
                        //    DebugUtils.Log("The block number now is:" + blockNumber);
                        //}
                        if (num_of_loops >= 6)
                        {
                            shouldBreak = true;
                            RaidStatus.text = "Deal Unsuccessful";
                            DebugUtils.Log("breaking loop");
                            throw new CustomException("Transaction done but Raid Unsuccessful");
                        }
                        else if (num_of_loops == 2)
                        {
                            blockNumber += 10;
                            DebugUtils.Log("The block number now is:" + blockNumber);
                        }

                        DebugUtils.Log("Started Listening");

                        RES_CCIPRaiding response = new RES_CCIPRaiding();
                        await SendRequestAsync($"{TransactionConfirmation_BaseURL}/getStatus/raid/{desiredChainName}/{blockNumber}/{connectedAddress}", (string res) =>
                        {
                            response = JsonUtility.FromJson<RES_CCIPRaiding>(res);
                        });
                        DebugUtils.Log("Raid resultisSuccess: " + response.isSuccess);
                        DebugUtils.Log("Raid resulteventOccurred: " + response.eventOccurred);
                        if (response?.eventOccurred == true)
                        {
                            if (response?.isSuccess == true)
                            {
                                RaidStatus.text = "Raid successful! The task force sezied " + GetRaidRewards(GetFormalNameFromChainName(desiredChainName)) + " BUDS from an illicit farm on " + GetFormalNameFromChainName(desiredChainName);
                                DebugUtils.Log("breaking loop");
                                TransactionTracker.Instance.CallingEndTransaction("raid", true);
                                throw new CustomException("Transaction done and Raid Successful");
                            }
                            else
                            {
                                RaidStatus.text = "Raid unsuccessful. It seems our intel was sound but the farm was cleared before the task force arrived.";
                                shouldBreak = true;
                                throw new CustomException("Transaction done but Raid Unsuccessful");
                            }
                        }
                    }
                    catch (CustomException ex)
                    {
                        DebugUtils.Log("TX MONITORING DONE with exception" + ex.Message);
                    }
                }
            }
            catch (CustomException ex)
            {
                RaidStatus.text = "Error 420: The game logic be tripping. \n Please try again or open a support ticket on our Discord";
                DebugUtils.Log($"Error in ccip raid listening: {ex.Message}");
                TransactionTracker.Instance.CallingEndTransaction("raid", false);
                return 0;
            }
            return 1;
        }
        #endregion

        public async Task<ulong> MonitorCCIPRaid(ulong _desiredChainSelector)
        {
            int num_of_loops = 0;
            string desiredChainName = GetParameterChainNameFromChainNameId(_desiredChainSelector);
            BigInteger currentBlock = await GetCurrentBlockNumber(desiredChainName);
            currentBlock -= 10;

            if (currentBlock == 0) return 0;

            bool shouldBreak = false;
            try
            {
                while (!shouldBreak)
                {
                    try
                    { 
                        DebugUtils.Log("Waiting for 30 seconds");
                        await delayOneMin();
                        num_of_loops++;
                        if (num_of_loops >= 6)
                        {
                            shouldBreak = true;
                            RaidStatus.text = "Deal Unsuccessful";
                            throw new CustomException("TX DONE");
                        }
                        DebugUtils.Log("30 secondsdone, next call!");
                        DebugUtils.Log("Started Listening");

                        RES_CCIPRaiding response = new RES_CCIPRaiding();
                        await SendRequestAsync($"{TransactionConfirmation_BaseURL}/getStatus/{desiredChainName}/{currentBlock}/{connectedAddress}", (string res) =>
                        {
                            response = JsonUtility.FromJson<RES_CCIPRaiding>(res);
                            DebugUtils.Log("Raid result: " + response);
                        });
                        DebugUtils.Log("Raid resultisSuccess: " + response.isSuccess);
                        DebugUtils.Log("Raid resulteventOccurred: " + response.eventOccurred);
                        if (response?.eventOccurred == true)
                        {
                            if (response?.isSuccess == true)
                            {
                                RaidStatus.text = "Raid successful! The task force sezied " + GetRaidRewards(GetFormalNameFromChainName(desiredChainName)) + " BUDS from an illicit farm on " + GetFormalNameFromChainName(desiredChainName);
                                DebugUtils.Log("breaking loop");
                                shouldBreak = true;
                                throw new CustomException("TX DONE");
                                break;
                            }
                            else
                            {
                                RaidStatus.text = "Raid unsuccessful. It seems our intel was sound but the farm was cleared before the task force arrived.";
                                shouldBreak = true;
                                throw new CustomException("TX DONE");
                                break;
                            }
                        }
                    }
                    catch (CustomException ex)
                    {
                        DebugUtils.Log("TX MONITORING DONE with exception" + ex.Message);
                        throw new CustomException("TX DONE");
                    }
                    catch (Exception ex)
                    {
                        RaidStatus.text = "Error 420: The game logic be tripping. \n Please try again or open a support ticket on our Discord";
                        DebugUtils.Log($"Error in ccip raid listening: {ex.Message}");
                        return 0;
                    }
                }
            }
            catch (CustomException ex)
            {
                DebugUtils.Log("TX MONITORING DONE 2");
            }
            return 1;
        }

        public async void ClearCCIPText()
        {
            CCIP_txn.text = "";
        }

        public async void RaidFarms()
        {
            interactManager.PoliceRaidStart();
            try
            {
                BoostRaid.interactable = false;
                Raid.interactable = false;
                RaidStatus.text = "LOADING...";
                DebugUtils.Log(RaidStatus.text);
                ulong desiredChain = GetRaidChainCCIPId();
                DebugUtils.Log("The desired chain is" + desiredChain);
                bool boostRaid = BoostRaid.isOn;
                ulong raidingFee = await GetRaidFee();
                DebugUtils.Log("Raid fees parsed: " + raidingFee);
                string informantTokenId = "0";

                if (boostRaid)
                {
                    informantTokenId = await ApproveInformant();
                    if (informantTokenId == "0") return;
                }
                RaidStatus.text = "Assembling task force to initiate raid";
                DebugUtils.Log(RaidStatus.text);
                DebugUtils.Log("The current chain is:" + currentChain + "The desired chain" + desiredChain);
                DebugUtils.Log(currentChain == desiredChain);
                if (currentChain == desiredChain)
                {
                    DebugUtils.Log("Inside Same Chain raid");
                    RES_globalBudsCcq claimRewardsResult = await ClaimRewardsWormholeAPI();
                    DebugUtils.Log("The bytes parameter passed is:" + claimRewardsResult.bytes);
                    // Same chain Raid

                    // Remove the "0x" prefix
                    claimRewardsResult.bytes = claimRewardsResult.bytes.Substring(2);

                    // Convert the hexadecimal string to a byte array
                    byte[] byteArray = Enumerable.Range(0, claimRewardsResult.bytes.Length / 2)
                        .Select(x => Convert.ToByte(claimRewardsResult.bytes.Substring(x * 2, 2), 16))
                        .ToArray();

                    DebugUtils.Log(byteArray);

                    TransactionTracker.Instance.AddTransaction("raid", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                    object[] TokenIdParam = { informantTokenId };
                    object[] OtherChainParams = { informantTokenId, byteArray, claimRewardsResult.sig };
                    var normalRaidResult = GetParameterChainNameFromChainNameId(currentChain) == GetParameterChainNameFromChainNameId(beraTestnet_chain) ?
                        await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, BERA_RAID, "raid", raidingFee, TokenIdParam) :
                        await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "raid", raidingFee, OtherChainParams);

                    DebugUtils.Log("The raid result" + normalRaidResult);
                    if (normalRaidResult.TransactionHash.Length > 0)
                    {
                        BigInteger BlockNumber = normalRaidResult.BlockNumber;
                        DebugUtils.Log("The block number is: " + BlockNumber + "and the desired chain is : ");
                        RaidStatus.text = "Task force is enroute to raid " + GetFormalNameFromChainNameId(desiredChain) + "'s farm!";
                        DebugUtils.Log(RaidStatus.text);
                        ulong result = await MonitorRaid(desiredChain, BlockNumber);
                        if (result == 0)
                        {
                            TransactionTracker.Instance.CallingEndTransaction("raid", false);
                            return;
                        }
                        TransactionTracker.Instance.CallingEndTransaction("raid", true);
                    }
                    else
                    {
                        RaidStatus.text = "Raid unsuccessful. It seems our intel was sound but the farm was cleared before the task force arrived.";
                        DebugUtils.Log(RaidStatus.text);
                        TransactionTracker.Instance.CallingEndTransaction("raid", false);
                        return;
                    }
                }
                //Cross Chain Raid
                else
                {
                    DebugUtils.Log("Inside Cross Chain Raid");
                    string currentChainName = GetParameterChainNameFromChainNameId(currentChain);
                    ulong ccipFees = 0;
                    ccipFees = await GetCCIPFees(currentChainName, "0", informantTokenId, desiredChain);
                    DebugUtils.Log("The ccip fees is:" + ccipFees);
                    ulong addedFee = ccipFees + raidingFee;
                    DebugUtils.Log("The Added value is:" + addedFee);
                    DebugUtils.Log("The Chain name is:" + GetParameterChainNameFromChainNameId(desiredChain));
                    //var crossChainRaid = await STAKE.Write("crossChainRaid", new TransactionRequest { value = (addedFee).ToString(), gasLimit = "2500000" }, desiredChain.ToString(), informantTokenId);
                    var crossChainRaid = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "crossChainRaid", addedFee, desiredChain.ToString(), informantTokenId);
                    if (!string.IsNullOrEmpty(crossChainRaid.TransactionHash))
                    {
                        //Tracking Transaction
                        TransactionTracker.Instance.AddTransaction("crossChainRaid", GetFormalNameFromChainNameId(currentChain), GetFormalNameFromChainNameId(desiredChain), 20f);

                        RES_updateChainState crossChainResult = await CrossChainWormholeAPI(desiredChain);
                        if (crossChainResult.status)
                        {
                            DebugUtils.Log("The query to update  was successful");
                        }

                        RaidStatus.text = "Task force is enroute to raid " + GetFormalNameFromChainNameId(desiredChain) + "'s farm!";
                        DebugUtils.Log(RaidStatus.text);
                        //ulong result = await MonitorCCIPRaid(desiredChain);
                        ulong result = await MonitorCrossChainRaid(desiredChain, crossChainRaid.TransactionHash);
                        if (result == 0)
                        {
                            TransactionTracker.Instance.CallingEndTransaction("crossChainRaid", false);
                            return;
                        }
                        TransactionTracker.Instance.CallingEndTransaction("crossChainRaid", true);
                    }
                    else
                    {
                        RaidStatus.text = "Raid unsuccessful. It seems our intel was sound but the farm was cleared before the task force arrived.";
                        TransactionTracker.Instance.CallingEndTransaction("crossChainRaid", false);
                        DebugUtils.Log(RaidStatus.text);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("crossChainRaid", false);
                TransactionTracker.Instance.CallingEndTransaction("raid", false);
                ClearCCIPText();
                RaidStatus.text = "Error 420: The game logic be tripping. \n Please try again or open a support ticket on our Discord ";
                DebugUtils.Log("Raid error occured: " + e.ToString());
            }
            CCIP_txn.text = "";
            GetRaidData_INTERNAL();
        }

        //API call for Cross Chain Raid
        public async Task<RES_updateChainState> CrossChainWormholeAPI(ulong destinationChain)
        {
            RES_updateChainState response = new RES_updateChainState();
            try
            {
                await SendRequestAsync($"{Wormhole_BaseUrl}/{PATH_updateChainState}/{GetParameterChainNameFromChainNameId(destinationChain)}", (string res) =>
                {
                    response = JsonUtility.FromJson<RES_updateChainState>(res);
                    DebugUtils.Log("The response is :" + res);
                    DebugUtils.Log("The response data is:" + response);
                }, true);
            }
            catch (Exception e)
            {
                DebugUtils.Log("The error is:" + e);
            }
            return response;
        }

        //API call for Cross Chain Raid
        public async Task<RES_InventoryData> GetUserInventory()
        {
            RES_InventoryData response = new RES_InventoryData();
            try
            {
                await SendRequestAsync($"{InventoryServiceAPIBase}/{PATH_GetUserInventory}/{connectedAddress}", (string res) =>
                {
                    response = JsonUtility.FromJson<RES_InventoryData>(res);
                    DebugUtils.Log("The response is :" + res);
                    DebugUtils.Log("The response data is:" + response);
                }, true);
            }
            catch (Exception e)
            {
                DebugUtils.Log("The error is:" + e);
            }
            return response;
        }

        public async void GetInventory()
        {
            // Call Save Function
            // Call the function to Mint
            // If required to call this function
            RES_InventoryData InventoryData = await GetUserInventory();
        }

        //Mint the ChainSaw
        public async void MintNFT(string itemDifferentiator)
        {
            ThirdwebContract contract = itemDifferentiator == "mintAmulate" ? InventoryItemAmulate : InventoryItemChainsaw;
            try
            {
                TransactionTracker.Instance.AddTransaction(itemDifferentiator, GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                byte[] mintByteArray = { };
                object[] mintParameters = { GetConnectedAddress(), 1,"", mintByteArray  };
                var MintResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, contract, "mint", 0,mintParameters);
                if (MintResult.TransactionHash.Length > 0)
                {
                    TransactionTracker.Instance.CallingEndTransaction(itemDifferentiator, true);
                    DebugUtils.Log("Minting is successful");
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction(itemDifferentiator, false);
                    DebugUtils.Log("Mint is unsuccessful");
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction(itemDifferentiator, false);
                DebugUtils.Log("Mint Unsuccessful: " + e.Message);
            }
        }

        #region Yeet Variables and functions
        public void OpenYeetPanel()
        {
            YeetAmtBera.interactable = true;
            YeetButton.interactable = false;
            interactManager.YeetPanelIn();
            try
            {
                GetAndSetYeetPanelData();
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Get Yeet Panel Information error: " + ex.Message);
            }
            YeetButton.interactable = true;
            bYeetRewardsPanelOpen = false;
        }

        public void GetAndSetYeetPanelData()
        {
            //Time Remaining
            TimeRemaining();
            //Current Round, Amount Yeet, Pooled Yeets
            CurrentRound();
            //Get Amount yeeted
            GetAmountYeet();
            //Live Prize Pool
            GetLivePrizePool();
            //Get pooled Yeets
            GetPooledYeets();
        }

        private async void TimeRemaining()
        {

            try
            {
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetTimeRemaining}", (string res) =>
                {
                    YeetInfo time = JsonUtility.FromJson<YeetInfo>(res);
                    if (time != null)
                    {
                        timeRemaining = BigInteger.Parse(time.response);
                        UpdateTimerDisplay();
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Getting time remaining error: " + ex.Message);
            }
        }

        private void UpdateTimerDisplay()
        {
            // Convert seconds to days, hours, minutes, seconds
            BigInteger seconds = timeRemaining % 60;
            BigInteger minutes = (timeRemaining / 60) % 60;
            BigInteger hours = (timeRemaining / 3600) % 24;

            // Format the display based on the magnitude of time
            string timerString;

            if (hours > 0)
            {
                timerString = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            }
            else
            {
                timerString = $"{minutes:D2}:{seconds:D2}";
            }

            timerText.text = timerString;
        }

        private async void CurrentRound()
        {
            try
            {
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetCurrentRound}", (string res) =>
                {
                    YeetInfo round = JsonUtility.FromJson<YeetInfo>(res);
                    if (round != null)
                    {
                        BigInteger CurrentRoundInteger = BigInteger.Parse(round.response);
                        Yeet_CurrentRound.text = CurrentRoundInteger.ToString();
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Getting time remaining error: " + ex.Message);
            }
        }

        private async void GetAmountYeet()
        {
            try
            {
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetAmountYeeted}/{connectedAddress}", (string res) =>
                {
                    YeetInfo amount = JsonUtility.FromJson<YeetInfo>(res);
                    if (amount != null)
                    {
                        BigInteger AmountYeet = BigInteger.Parse(amount.response);
                        AmountYeeted.text = AmountYeet.ToString();
                        DebugUtils.Log("Amount Yeeted: " + AmountYeet);
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Getting Amount Yeet Error: " + ex.Message);
            }
        }

        private async void GetLivePrizePool()
        {
            try
            {
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetLivePrizePool}", (string res) =>
                {
                    YeetInfo pool = JsonUtility.FromJson<YeetInfo>(res);

                    DebugUtils.Log("The response is: " + res);
                    if (pool != null)
                    {
                        BigInteger wei = BigInteger.Parse(pool.response);
                        BigInteger divisor = BigInteger.Pow(10, 18);
                        decimal beraAmount = (decimal)wei / (decimal)divisor;
                        beraAmount = Math.Round(beraAmount, 2);
                        Yeet_LivePrizePool.text = beraAmount.ToString() + " BERA";
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception e)
            {
                DebugUtils.LogError("Failed to get:" + e.Message);
            }
        }

        private async void GetPooledYeets()
        {
            try
            {
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetPooledYeets}", (string res) =>
                {
                    YeetInfo pooledY = JsonUtility.FromJson<YeetInfo>(res);
                    if (pooledY != null)
                    {
                        BigInteger pYeets = BigInteger.Parse(pooledY.response);
                        BigInteger divisor = BigInteger.Pow(10, 18);
                        decimal pooledAmount = (decimal)pYeets / (decimal)divisor;
                        pooledAmount = Math.Round(pooledAmount, 2);
                        PooledYeet.text = pooledAmount.ToString();
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });

            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Getting Pooled Yeets Error: " + ex.Message);
            }
        }

        public async void YeetTransaction()
        {
            ulong YeetInputAmount = string.IsNullOrEmpty(YeetAmtBera.text)? GetStakeAmountFromInput("0.69"): GetStakeAmountFromInput(YeetAmtBera.text);
            try
            {
                TransactionTracker.Instance.AddTransaction("yeet", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                var MintResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, Yeet_Contract, "yeet", UnitConversion.Convert.ToWei(YeetInputAmount.ToString(), UnitConversion.EthUnit.Ether));
                if (MintResult.TransactionHash.Length > 0)
                {
                    TransactionTracker.Instance.CallingEndTransaction("yeet", true);
                    DebugUtils.Log("Yeeting is successful");
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("yeet", false);
                    DebugUtils.Log("Yeeting is unsuccessful");
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("yeet", false);
                DebugUtils.Log("Yeeting Unsuccessful: " + e.Message);
            }
        }

        public void OpenYeetRewardsPanel()
        {
            YeetRewardsClaim.interactable = true;
            try
            {
                GetAndSetYeetRewardsPanelData();

            }
            catch (Exception ex)
            {
                DebugUtils.LogError("Yeet Claim Rewards open error " + ex.Message);

            }
        }

        private void GetAndSetYeetRewardsPanelData()
        {
            GetYeetRewardsAndRound();
            bYeetRewardsPanelOpen = true;
        }

        private void ValidateInput(string input)
        {
            // Remove any non-numeric characters
            string numbers = System.Text.RegularExpressions.Regex.Replace(input, "[^0-9]", "");

            // Update input field if the value changed
            if (input != numbers)
            {
                RoundNumberInput.text = numbers;
            }
        }

        public int GetInputAsInteger()
        {
            if (int.TryParse(RoundNumberInput.text, out int number))
            {
                return number;
            }
            return 1; 
        }

        public async void YeetClaimReward()
        {
            int roundInput = GetInputAsInteger();
            try
            {
                //Get Round Number to pass as a parameter
                TransactionTracker.Instance.AddTransaction("claimReward", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                var YeetClaimRewardTransaction  = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, Yeet_Contract, "claimReward",0, roundInput);
                if (YeetClaimRewardTransaction.TransactionHash.Length > 0)
                {
                    TransactionTracker.Instance.CallingEndTransaction("claimReward", true);
                    DebugUtils.Log("Yeet Claim Reward is successful");
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("claimReward", false);
                    DebugUtils.Log("Yeet Claim Reward is unsuccessful");
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("claimReward", false);
                DebugUtils.Log("Yeet Claim Reward: " + e.Message);
            }
        }

        private async void GetYeetRewardsAndRound()
        {
            try
            {
                await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetYeetRewards}/{connectedAddress}", (string res) =>
                {
                    YeetRewardsContent data = JsonUtility.FromJson<YeetRewardsContent>(res);
                    if (data != null)
                    {
                        DebugUtils.Log(data.response.Length);
                        PopulateYeetRewardsDropdown(data.response);
                    }
                    else
                    {
                        DebugUtils.LogError("Failed to deserialize JSON response.");
                    }
                });
            }
            catch (Exception e)
            {
                DebugUtils.LogError("Failed to get:" + e.Message);
            }
        }

        private void PopulateYeetRewardsDropdown(YeetRewardsField[] wrapper)
        {
            for (int i = 0; i < wrapper.Length; i++)
            {
                GameObject gameObject = Instantiate(YeetRoundContent, ScrollContentHolder.transform);
                YeetRoundContentManager manager = gameObject.GetComponent<YeetRoundContentManager>();
                if (manager != null)
                {
                    manager.SetText(wrapper[i].round, wrapper[i].amountWon);
                }
            }
        }

        public void SetboolYeetRewards()
        {
            bYeetRewardsPanelOpen = false;
        }

        #endregion

        //Burn
        public async void ClaimTheFarmer()
        {
            DebugUtils.Log("CLAIM FARMER CALLED");
            DebugUtils.Log("Calling Game Object: " + gameObject.name);
            try
            {
                TransactionTracker.Instance.AddTransaction("ClaimFarmer", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var farmerClaimResult = await NFT_FAUCET.Write("claimFarmer", new TransactionRequest { gasLimit = "1500000" });
                var farmerClaimResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, NFT_FAUCET, "claimFarmer", 0);
                if (!string.IsNullOrEmpty(farmerClaimResult.TransactionHash))
                {
                    TransactionTracker.Instance.CallingEndTransaction("ClaimFarmer", true);
                    DebugUtils.Log("Farmer claim successful");
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("ClaimFarmer", false);
                    DebugUtils.Log("Farmer claim error");
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("ClaimFarmer", false);
                DebugUtils.Log("Claim Farmer error: " + e.Message);
            }
        }

        public async void ClaimTheNarc()
        {
            DebugUtils.Log("CLAIM NARC CALLED");
            try
            {
                TransactionTracker.Instance.AddTransaction("ClaimNarc", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var narcClaimResult = await NFT_FAUCET.Write("claimNarc");
                var narcClaimResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, NFT_FAUCET, "claimNarc", 0);
                if (!string.IsNullOrEmpty(narcClaimResult.TransactionHash))
                {
                    TransactionTracker.Instance.CallingEndTransaction("ClaimNarc", true);
                    DebugUtils.Log("narc claim successful");
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("ClaimNarc", false);
                    DebugUtils.Log("narc claim error");
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("ClaimNarc", false);
                DebugUtils.Log("Claim narc error: " + e.Message);
            }
        }

        public async void ClaimBuds()
        {
            try
            {
                DebugUtils.Log("The current chain is:" + GetFormalNameFromChainNameId(currentChain));
                TransactionTracker.Instance.AddTransaction("Claim", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var budsClaimResult = await BUDS_FAUCET.Write("claim", new TransactionRequest { gasLimit = "1500000" }, connectedAddress);
                var budsClaimResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, BUDS_FAUCET, "claim", 0);
                if (!string.IsNullOrEmpty(budsClaimResult.TransactionHash))
                {
                    DebugUtils.Log("buds claim successful");
                    TransactionTracker.Instance.CallingEndTransaction("Claim", true);
                }
                else
                {
                    DebugUtils.Log("buds claim error");
                    TransactionTracker.Instance.CallingEndTransaction("Claim", false);
                }
            }
            catch (Exception e)
            {
                DebugUtils.Log("Claim buds error: " + e.Message);
                TransactionTracker.Instance.CallingEndTransaction("Claim", false);
            }
            GetBudsBalance();
        }

        public async void openBurnDialogForStoner()
        {
            BurnForStoner_B.interactable = true;
            try
            {
                interactManager.PlaygroundIn();
                BurnStatus.text = "LOADING...";
                BigInteger balance = await BUDS.Read<BigInteger>("balanceOf", connectedAddress);
                GetBudsBalance();
                if (balance == 0)
                {
                    BurnForStoner_B.interactable = false;
                }
            }
            catch (Exception e)
            {
                DebugUtils.Log("openBun dialog error: " + e.Message);
            }
        }
        public async void openBurnDialogForInformant()
        {
            BurnForInformant_B.interactable = true;
            try
            {
                interactManager.BlackMarketIn();
                BurnStatus.text = "LOADING...";
                BigInteger balance = await BUDS.Read<BigInteger>("balanceOf", connectedAddress);
                GetBudsBalance();
                if (balance == 0)
                {
                    BurnForInformant_B.interactable = false;
                }
            }
            catch (Exception e)
            {
                DebugUtils.Log("openBun dialog error: " + e.Message);
            }
        }

        public async Task<ulong> MonitorBurnEvent(BigInteger blockNumber)
        {
            int num_Of_runs = 0;
            ulong result = 0;
            bool shouldBreak = false;
            try
            {
                while (!shouldBreak)
                {
                    try
                    { 
                        DebugUtils.Log("Waiting for 30 seconds");
                        await delayOneMin();
                        DebugUtils.Log("30 seconds done, next call!");
                        num_Of_runs++;
                        if (num_Of_runs >= 6)
                        {
                            RaidStatus.text = "Deal unsuccessful. It seems our intel was sound but the farm was cleared before the task force arrived.";
                            shouldBreak = true;
                            throw new CustomException("TX DONE");
                            break;
                        }
                        DebugUtils.Log("Started Listening");

                        DebugUtils.Log($"{TransactionConfirmation_BaseURL}/ getStatus / burn /{ GetParameterChainNameFromChainNameId(currentChain)}/{ blockNumber}/{ connectedAddress}");
                        Burn response = new Burn();
                        await SendRequestAsync($"{TransactionConfirmation_BaseURL}/getStatus/burn/{GetParameterChainNameFromChainNameId(currentChain)}/{blockNumber}/{connectedAddress}", (string res) =>
                        {
                            response = JsonUtility.FromJson<Burn>(res);
                            DebugUtils.Log("Raid result: " + res);
                        });
                        DebugUtils.Log("Raid resultisSuccess: " + response.status.boosterMinted);
                        DebugUtils.Log("Raid resulteventOccurred: " + response.status.tokenId);
                        if (response.status.boosterMinted.Length > 0)
                        {
                            if (response.status.tokenId != "0")
                            {
                                if (response.status.boosterMinted == "Informant")
                                {
                                    RaidStatus.text = "Bribe successful! The informant has agreed to share intel with you.";
                                    result = 1;
                                    TransactionTracker.Instance.CallingEndTransaction("Burn", true);
                                }
                                else if (response.status.boosterMinted == "Stoner")
                                {
                                    RaidStatus.text = "Deal accepted! The stoner has agreed to purchase more of your supply.";
                                    result = 1;
                                    TransactionTracker.Instance.CallingEndTransaction("BurnStoner", true);
                                }
                                else
                                {
                                    RaidStatus.text = "Deal unsuccessful!";
                                    result = 1;
                                }
                                DebugUtils.Log("breaking loop");
                                shouldBreak = true;
                                throw new CustomException("TX DONE");
                                break;
                            }
                            else
                            {
                                RaidStatus.text = "Deal unsuccessful.";
                                shouldBreak = true;
                                throw new CustomException("TX DONE");
                                break;
                            }
                        }
                    }
                    catch (CustomException ex)
                    {
                        DebugUtils.Log("TX MONITORING DONE");
                        throw new CustomException("TX DONE");
                    }
                    catch (Exception ex)
                    {
                        RaidStatus.text = "Error 420: The game logic be tripping. \n Please try again or open a support ticket on our Discord";
                        DebugUtils.Log($"Error in ccip raid listening: {ex.Message}");
                        TransactionTracker.Instance.CallingEndTransaction("raid", false);
                        return 0;
                    }
                }
            }
            catch (CustomException ex)
            {
                DebugUtils.Log("TX MONITORING DONE 2");
            }
            return result;
        }

        public async void BurnForStoner()
        {
            interactManager.txAnimator.SetTrigger("in");
            try
            {
                BurnStatus.text = "Please release the $BUDS to attempt recruiting a stoner";
                TransactionTracker.Instance.AddTransaction("BurnStoner", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var burnResult = await STAKE.Write("burnForStoner", new TransactionRequest { gasLimit = "1500000" });
                var burnResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "burnForStoner", 0);
                DebugUtils.Log("The burnResult transaction Hash is:" + burnResult.TransactionHash);
                string txnHash = burnResult.TransactionHash;
                DebugUtils.Log(txnHash.Length > 0);
                if (txnHash.Length > 0)
                {
                    BigInteger BlockNumber = burnResult.BlockNumber;
                    DebugUtils.Log(BlockNumber);
                    DebugUtils.Log("TX hash: " + burnResult.TransactionHash);
                    BurnStatus.text = "Negotiating a deal with potential stoner...";
                    // listen for Burned events
                    ulong result = await MonitorBurnEvent(BlockNumber);
                    if (result == 0)
                    {
                        TransactionTracker.Instance.CallingEndTransaction("BurnStoner", false);
                        return;
                    }
                    TransactionTracker.Instance.CallingEndTransaction("BurnStoner", true);
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("BurnStoner", false);
                    BurnStatus.text = "ERROR: burn approval";
                    return;
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("BurnStoner", false);
                DebugUtils.Log("Burn for stoner error: " + e.Message);
            }
        }

        public async void BurnForInformant()
        {
            interactManager.txAnimator.SetTrigger("in");
            try
            {
                BurnStatus.text = "Release the $BUDS to bribe a bystander";
                TransactionTracker.Instance.AddTransaction("Burn", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                var burnResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "burnForInformant", 0);
                if (burnResult.TransactionHash.Length > 0)
                {
                    BigInteger BlockNumber = burnResult.BlockNumber;
                    DebugUtils.Log(BlockNumber);
                    DebugUtils.Log("TX HASH: " + burnResult.TransactionHash);
                    BurnStatus.text = "Negotiating deal with potential informant...";
                    // listen for Burned events
                    ulong result = await MonitorBurnEvent(BlockNumber);
                    if (result == 0)
                    {
                        TransactionTracker.Instance.CallingEndTransaction("Burn", false);
                        return;
                    }
                    TransactionTracker.Instance.CallingEndTransaction("Burn", true);
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("Burn", false);
                    BurnStatus.text = "Error 420: Either you're tripping or there's a bug here! Please try again or open a support ticket on our Discord if the issue persists.";
                    return;
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("Burn", false);
                DebugUtils.Log("Burn for informant error: " + e.Message);
            }
        }

        //Get FromalName of the chains when ChainId is given
        public void openBudsFaucet()
        {
            interactManager.FaucetIn();

            BudsFaucetChainName.text = GetFormalNameFromChainNameId(currentChain);
        }

        //Here _chainname is the Formal Chain Name
        public string GetRaidRewards(string _chainname)
        {
            if (_chainname == "BaseSepolia")
            {
                return RAID_BaseSepoliaRaidRewards.text;
            }
            else if (_chainname == "Bera")
            {
                return RAID_BeraTestnetRaidRewards.text;
            }
            else if (_chainname == "Binance")
            {
                return RAID_BscRaidRewards.text;
            }
            else if (_chainname == "Avalanche")
            {
                return RAID_FujiRaidRewards.text;
            }
            else if (_chainname == "Amoy")
            {
                return RAID_AmoyRaidRewards.text;
            }
            else
            {
                return RAID_ArbitrumRaidRewards.text;
            }
        }

        #region GetFormalChainName
        //This two functions are getting Fromal Chain Name from chainName in string and chainName used in
        //ulong chain Name ids so in both the codition name will be same.
        //Add "_chain" while using ulong
        private string GetFormalNameFromChainName(string chainName)
        {
            if (chainName == "baseSepolia")
            {
                return "BaseSepolia";
            }
            else if (chainName == "beraTestnet")
            {
                return "Bera";
            }
            else if (chainName == "bscTestnet")
            {
                return "Binance";
            }
            else if (chainName == "fuji")
            {
                return "Avalanche";
            }
            else if (chainName == "amoy")
            {
                return "Amoy";
            }
            else
            {
                return "Arbitrum";
            }
        }

        private string GetFormalNameFromChainNameId(ulong chainName)
        {
            if (chainName == 0)
            {
                return "";
            }
            else if (chainName == baseSepolia_chain)
            {
                return "BaseSepolia";
            }
            else if (chainName == beraTestnet_chain)
            {
                return "Bera";
            }
            else if (chainName == bscTestnet_chain)
            {
                return "Binance";
            }
            else if (chainName == fuji_chain)
            {
                return "Avalanche";
            }
            else if (chainName == amoy_chain)
            {
                return "Amoy";
            }
            else
            {
                return "Arbitrum";
            }
        }
        #endregion

        #region GetParameterChainName
        //This function provides with the chain Name used for API calls

        public string GetParameterChainNameFromChainNameId(ulong chain)
        {
            if (chain == 0)
            {
                return "";
            }
            if (chain == baseSepolia_chain)
            {
                return "baseSepolia";
            }
            else if (chain == beraTestnet_chain)
            {
                return "beraTestnet";
            }
            else if (chain == bscTestnet_chain)
            {
                return "bscTestnet";
            }
            else if (chain == fuji_chain)
            {
                return "fuji";
            }
            else if (chain == amoy_chain)
            {
                return "amoy";
            }
            else
            {
                return "arbSepolia";
            }
        }
        #endregion

        #region GetChainIdFromFromalName
        //Function Name is Self Explinatory
        public ulong GetChainIdFromFormalChainName(string chainName)
        {
            if (chainName == "BaseSepolia") return baseSepolia_chain;

            else if (chainName == "Bera") return beraTestnet_chain;

            else if (chainName == "Binance") return bscTestnet_chain;

            else if (chainName == "Avalanche") return fuji_chain;

            else if (chainName == "Amoy") return amoy_chain;

            else return arbitrum_chain;
        }
        #endregion

        #region GetTimePeriodForSingleChain
        //Function Name is Self Explinatory
        public float GetTime(ulong chainName)
        {
            if (chainName == 0)
            {
                return 0.0f;
            }
            else if (chainName == baseSepolia_chain)
            {
                return 6.0f;
            }
            else if (chainName == beraTestnet_chain)
            {
                return 7.0f;
            }
            else if (chainName == bscTestnet_chain)
            {
                return 5.0f;
            }
            else if (chainName == fuji_chain)
            {
                return 3.0f;
            }
            else if (chainName == amoy_chain)
            {
                return 12.0f;
            }
            else
            {
                return 5.0f;
            }
        }
        #endregion

        #region convert byte array to Hex
        public string ByteArrayToHex(byte[] byteArray)
        {
            return "0x" + BitConverter.ToString(byteArray).Replace("-", string.Empty).ToLower();
        }
        #endregion

        public string GetConnectedAddress()
        {
            return connectedAddress;
        }

        #region NotUsedFunctions
        public async void ClaimAndUnstakeRewards()
        {
            UnStakeFarmer.interactable = false;
            UnStakeAmt.interactable = false;
            UnStakeNClaim.interactable = false;

            UnStakeStatus.text = "WAITING: Txn approval";

            try
            {
                RES_globalBudsCcq result = await ClaimRewardsWormholeAPI();
                DebugUtils.Log("The bytes Parameter passed is:" + result.bytes);

                // Remove the "0x" prefix
                result.bytes = result.bytes.Substring(2);

                // Convert the hexadecimal string to a byte array
                byte[] byteArray = Enumerable.Range(0, result.bytes.Length / 2)
                    .Select(x => Convert.ToByte(result.bytes.Substring(x * 2, 2), 16))
                    .ToArray();

                DebugUtils.Log(byteArray.ToString());

                TransactionTracker.Instance.AddTransaction("claimAndUnstake", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));

                var claimResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, STAKE, "claimAndUnstake", 0, byteArray, result.sig);

                if (!string.IsNullOrEmpty(claimResult.TransactionHash))
                {
                    TransactionTracker.Instance.CallingEndTransaction("claimAndUnstake", true);
                    UnStakeStatus.text = "Claiming buds rewards";
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("claimAndUnstake", false);
                    UnStakeStatus.text = "Unsuccessful Claim";
                    return;
                }
            }
            catch (Exception ex)
            {
                TransactionTracker.Instance.CallingEndTransaction("claimAndUnstake", false);
                DebugUtils.LogError(" Claim rewards error: " + ex.Message);
            }

            UnStakeFarmer.interactable = true;
            UnStakeAmt.interactable = true;
            UnStakeNClaim.interactable = true;
            OpenUnStakeDialog();
        }

        public async void GetRektChadBakedData()
        {
            try
            {
                //await SendRequestAsync($"{ReadApi_BaseURL}/{PATH_GetDashboardData}/REKT", (string res) =>
                await SendRequestAsync($"{ReadApi_BaseURL}/mostRekt", (string res) =>
                {
                    DebugUtils.Log("REKT res: " + res);
                    RES_RektChain response = new RES_RektChain();
                    response.rektChain = JsonUtility.FromJson<RektChain>(res);

                    INFO_RektChain.text = "24H <shake a=0.5><wiggle a=0.2><color=red>REKT</color></wiggle></shake>: " + GetFormalNameFromChainName(response.rektChain.chain) +
                    "\n <color=#4B4B4B>(" + (response.rektChain.raided < 0 ? "-" : "") + response.rektChain.raided + " BUDS lost to raids)</color>";
                });
                await SendRequestAsync($"{ReadApi_BaseURL}/mostBaked", (string res) =>
                {
                    DebugUtils.Log("BAKED res: " + res);
                    RES_BakedChain response = new RES_BakedChain();
                    response.bakedChain = JsonUtility.FromJson<BakedChain>(res);

                    INFO_BakedChain.text = "24H <incr a=0.8><wave a=0.2><color=#158E15>BAKED</color></wave></incr>: " + GetFormalNameFromChainName(response.bakedChain.chain) +
                    "\n <color=#4B4B4B>(Additional " + (response.bakedChain.staked > 0 ? "+" : "") + response.bakedChain.staked + " BUDS staked)</color>";
                });
            }
            catch (Exception e)
            {
                DebugUtils.LogError("getting dashboard data error: " + e.Message);
            }
        }

        //Mint amulate
        public async void MintAmulate()
        {
            try
            {
                TransactionTracker.Instance.AddTransaction("mintAmulate", GetFormalNameFromChainNameId(currentChain), GetTime(currentChain));
                //var MintResult = await InventoryItemContract.Write("mintAmulate", new TransactionRequest { gasLimit = "1500000" });
                var MintResult = await ThirdwebContract.Write(ThirdwebManager.Instance.ActiveWallet, InventoryItemAmulate, "mintAmulate", 0);
                if (!string.IsNullOrEmpty(MintResult.TransactionHash))
                {
                    TransactionTracker.Instance.CallingEndTransaction("mintAmulate", true);
                    DebugUtils.Log("Minting is successful");
                }
                else
                {
                    TransactionTracker.Instance.CallingEndTransaction("mintAmulate", false);
                    DebugUtils.Log("Mint is unsuccessful");
                }
            }
            catch (Exception e)
            {
                TransactionTracker.Instance.CallingEndTransaction("mintAmulate", false);
                DebugUtils.Log("Mint Unsuccessful: " + e.Message);
            }
        }
        #endregion

    }
}
