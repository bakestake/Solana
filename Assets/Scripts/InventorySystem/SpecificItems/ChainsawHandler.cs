using System.Collections;
using System.Collections.Generic;
using BakelandWalletInteraction;
using DG.Tweening;
using UnityEngine;

public class ChainsawHandler : MonoBehaviour
{
    private UaserWalletInteractions userWalletInteractions = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject userWalletObject = GameObject.FindGameObjectWithTag("UserWallet");
        if (userWalletObject != null)
        {
            userWalletInteractions = userWalletObject.GetComponent<UaserWalletInteractions>();
        }
    }

    public void ShowMintPanel()
    {
        PlayerController.canMove = false;
        InteractManager.Instance.ChainsawMintIn();
    }

    public void HideMintPanel()
    {
        InteractManager.Instance.ChainsawMintOut();
        PlayerController.canMove = true;
    }

    public void ConfirmMintButton()
    {
        if (PickUpItemFunctionName.mintFunctionName != null)
        {
            userWalletInteractions.MintNFT(PickUpItemFunctionName.mintFunctionName);
            PickUpItemFunctionName.mintFunctionName = null;
        }
        
        HideMintPanel();
    }
}
