using System;
using System.Collections;
using System.Collections.Generic;
using Thirdweb.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class UserRegistration : MonoBehaviour
    {
        [SerializeField] private GameObject userRegistrationPanel;

        [SerializeField] private TMP_InputField usernameInputField;
        [SerializeField] private TMP_InputField referrerCodeInputField;
        [SerializeField] private TMP_InputField createReferralCodeInputField;

        [SerializeField] private Button submitButton;

        [SerializeField] public static string username;
        [SerializeField] public static string referralCode;

        private void Start()
        {
            if (submitButton != null)
            {
                submitButton.onClick.RemoveAllListeners();
                submitButton.onClick.AddListener(() => RegisterUser());
            }
        }

        private async void RegisterUser()
        {
            if (usernameInputField == null || string.IsNullOrEmpty(usernameInputField.text))
            {
                usernameInputField.text = "Enter a username";
                return;
            }

            if (createReferralCodeInputField == null || string.IsNullOrEmpty(createReferralCodeInputField.text))
            {
                createReferralCodeInputField.text = "create a referral code";
                return;
            }

            try
            {
                string usernameFromInput = usernameInputField.text;
                string referrerCodeFromInput = referrerCodeInputField.text;
                string referralCodeFromInput = createReferralCodeInputField.text;

                await UserAndReferralApi.CreateUser(WalletConnectScript.connectedWalletAddress, usernameFromInput, referralCodeFromInput, referrerCodeFromInput, "");

                username = usernameFromInput;
                referralCode = referralCodeFromInput;
                PlayerController.Instance.SetUsername(UserRegistration.username);

                userRegistrationPanel.gameObject.SetActive(false);

            }
            catch (Exception e) { 
                Debug.Log("Failed to register user :" + e);
                throw new Exception("Failed to register user :" + e);
            }
        }

        public static void SetUserInfo(string _username, string _refCode) {
            if (string.IsNullOrEmpty(_username)) {
                Debug.LogError("Can not set null username");
            }

            if (string.IsNullOrEmpty(_refCode))
            {
                Debug.LogError("Can not set null referral code");
            }

            username = _username;
            referralCode = _refCode;

            PlayerController.Instance.SetUsername(UserRegistration.username);
        }
    }
}
