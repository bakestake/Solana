using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using BakelandWalletInteraction;

public class AccessCodeChecker : MonoBehaviour
{
    public GameObject accessCodePanel;
    private CanvasGroup accessCodePanelCanvasGroup;

    public TMP_InputField userNameInput;
    public TMP_InputField accessCodeInput;
    public Button accessCodeSubmitButton;
    public CanvasGroup errorTextCanvasGroup;
    public bool hasAccess;
    public bool isAccessCodeValid;
    //Getting the Wallet Interaction
    public UaserWalletInteractions userWalletInteractions;

    private void Start()
    {
        accessCodePanelCanvasGroup = accessCodePanel.GetComponent<CanvasGroup>();

        errorTextCanvasGroup.alpha = 0;
    }

    // CALL THIS 
    // if the entered access code is VALID to save it with the connected wallet.
    public void GrantAccess()
    {
        hasAccess = true;
        LocalSaveManager.instance.SaveAccess();
        Debug.Log("Access code is valid!");
    }

    // CALL THIS 
    // if the entered access code is INVALID to keep the player moving forward.
    public void DenyAccess()
    {
        hasAccess = false;
        LocalSaveManager.instance.SaveAccess();
        Debug.Log("Access code is invalid!");
    }

    // EXAMPLE ACCESS CODE CHECKER 
    // for you to modify to your needs.
    public async void AccessCodeSubmitButton()
    {
        if (accessCodeInput != null && accessCodeInput.text != "" &&
            userNameInput != null && userNameInput.text != "")
        {
            accessCodeSubmitButton.interactable = false;
            LoadingWheel.instance.EnableLoading();
            errorTextCanvasGroup.alpha = 0;
            await ConfirmCreateUser(userNameInput.text, accessCodeInput.text);
            // validity check logic
            // set returning bool to isAccessCodeValid

            if (isAccessCodeValid) // or use your own variable
            {
                GrantAccess();
                LocalGameManager.Instance.CheckFirstTime();
                HidePanel();
                errorTextCanvasGroup.alpha = 0;
            }
            else
            {
                DenyAccess();
                errorTextCanvasGroup.alpha = 1;
                // show an error message
            }
            accessCodeSubmitButton.interactable = true;
            LoadingWheel.instance.DisableLoading();
        }
    }

    public async Task ConfirmCreateUser(string userName, string accessCode)
    {
        string result = await userWalletInteractions.CreateUser(userName, accessCode);
        if (string.Equals(result, "null"))
        {
            Debug.Log("User once already created or invalid user");
            isAccessCodeValid = false;
        }
        else if (string.Equals(result, "User and referral code created successfully!"))
        {
            Debug.Log("User Created");
            isAccessCodeValid = true;
        }
    }

    public void SetAccessCodeUI()
    {
        if (LocalSaveManager.instance.CheckAccessCodeEntry())
        {
            HidePanel();
            LocalGameManager.Instance.CheckFirstTime();
            hasAccess = true;
        }
        else
        {
            ShowPanel();
            hasAccess = false;
        }

        LoadingWheel.instance.DisableLoading();
    }

    public void ShowPanel()
    {
        accessCodePanel.SetActive(true);
        accessCodePanelCanvasGroup.DOFade(1, 0.25f);
        accessCodePanelCanvasGroup.blocksRaycasts = true;
        accessCodePanelCanvasGroup.interactable = true;
        PlayerController.canMove = false;
    }

    public void HidePanel()
    {
        accessCodePanelCanvasGroup.interactable = false;
        accessCodePanelCanvasGroup.blocksRaycasts = false;
        accessCodePanelCanvasGroup.alpha = 0;
        accessCodePanel.SetActive(false);
        PlayerController.canMove = true;
        LocalGameManager.Instance.canUseKeybinds = true;
    }
}
