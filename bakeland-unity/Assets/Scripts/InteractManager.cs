using System.Collections;
using System.Collections.Generic;
//using Fusion;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class InteractManager : MonoBehaviour
{
    public static InteractManager instance;

    [HideInInspector]
    //public NetworkRunner Runner;

    public FastTravelPanel fastTravelPanel;
    public Animator mapAnimator;
    public Animator handbookAnimator;
    public Animator settingsAnimator;
    public Animator highlightHandbookAnimator;
    public Animator highlightNetworkAnimator;
    public Animator faucetAnimator;
    public Animator bulletinBoardAnimator;
    public Animator farmAnimator;
    public Animator yeetAnimator;
    public Animator policeAnimator;
    public Animator blackMarketAnimator;
    public Animator playgroundAnimator;
    public Animator txAnimator;
    public Animator yeetBoothNoticeAnimator;
    public Animator PvPPanelAnimator;
    public TextMeshProUGUI txFirstText;

    [Header("Stake&Unstake")]
    public CanvasGroup stakeHeader;
    public CanvasGroup unstakeHeader;
    public GameObject stakeContent;
    public GameObject unstakeContent;

    [Header("Yeet&Rewards")]
    public CanvasGroup YeetHeader;
    public CanvasGroup YeetRewardsHeader;
    public GameObject YeetContent;
    public GameObject YeetRewardsContent;

    [Header("Canvas Group")]
    public CanvasGroup PvPPanelCanvasGroup;

    [Header("References")]
    public GameObject chainsawMintPanel;
    public CanvasGroup chainsawMintPanelCanvasGroup;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OpenMap()
    {
        mapAnimator.SetTrigger("in");
    }

    public void CloseMap()
    {
        mapAnimator.SetTrigger("out");
    }

    public void SettingsIn()
    {
        //PlayerController.canMove = false;
        settingsAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.handbookIn);
    }

    public void SettingsOut()
    {
        //PlayerController.canMove = true;
        settingsAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.handbookOut);
    }

    public void HandbookIn()
    {
        //PlayerController.canMove = false;
        handbookAnimator.SetTrigger("in");
        highlightHandbookAnimator.SetTrigger("out");
        highlightNetworkAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.handbookIn);
    }

    public void HandbookOut()
    {
        //PlayerController.canMove = true;
        handbookAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.handbookOut);
    }

    public void FaucetIn()
    {
        //PlayerController.canMove = false;
        faucetAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.faucetIn);
    }

    public void FaucetOut()
    {
        //PlayerController.canMove = true;
        faucetAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.faucetOut);
    }

    public void BulletinBoardIn()
    {
        //PlayerController.canMove = false;
        bulletinBoardAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.bulletinBoardIn);
    }

    public void BulletinBoardOut()
    {
        //PlayerController.canMove = true;
        bulletinBoardAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.bulletinBoardOut);
    }

    public void YeetBoothNoticeIn()
    {
        yeetBoothNoticeAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.bulletinBoardIn);
    }

    public void YeetBoothNoticeOut()
    {
        yeetBoothNoticeAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.bulletinBoardOut);
    }

    public void YeetPanelIn()
    {
        PlayerController.canMove = false;
        yeetAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.farmIn);
        YeetClicked();
    }

    public void YeetPanelOut()
    {
        PlayerController.canMove = true;
        yeetAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.farmOut);
    }

    public void FarmIn()
    {
        //PlayerController.canMove = false;
        farmAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.farmIn);
        StakeClicked();
    }

    public void FarmOut()
    {
        //PlayerController.canMove = true;
        farmAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.farmOut);
    }

    public void FarmStakeStart()
    {
        // STAKE CODE

        //PlayerController.canMove = false;
        SoundManager.instance.PlaySfx(SoundManager.instance.txIn);
        txAnimator.SetTrigger("in");
        // TMP_Dropdown dropdown = farmAnimator.gameObject.GetComponentInChildren<TMP_Dropdown>();
        // string selectedChain = dropdown.options[dropdown.value].text;
        // txFirstText.text = "Sending across your BUDS to " + selectedChain;
    }

    public void PoliceIn()
    {
        //PlayerController.canMove = false;
        policeAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.policeIn);
    }

    public void PoliceOut()
    {
        //PlayerController.canMove = true;
        policeAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.policeOut);
    }

    public void PoliceRaidStart()
    {
        // RAID CODE
        //PlayerController.canMove = false;
        SoundManager.instance.PlaySfx(SoundManager.instance.txIn);
        // TMP_Dropdown dropdown = policeAnimator.gameObject.GetComponentInChildren<TMP_Dropdown>();
        // string selectedChain = dropdown.options[dropdown.value].text;
        // txFirstText.text = "Initiating raid on " + selectedChain + ". Please wait 420 seconds.";
        txAnimator.SetTrigger("in");
    }

    public void TXClose()
    {
        //PlayerController.canMove = true;
        SoundManager.instance.PlaySfx(SoundManager.instance.txOut);
        txAnimator.SetTrigger("out");
    }

    public void BlackMarketIn()
    {
        //PlayerController.canMove = false;
        blackMarketAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.blackMarketIn);
    }

    public void BlackMarketOut()
    {
        //PlayerController.canMove = true;
        blackMarketAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.blackMarketOut);
    }

    public void PlaygroundIn()
    {
        //PlayerController.canMove = false;
        playgroundAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.playgroundIn);
    }

    public void PlaygroundOut()
    {
        //PlayerController.canMove = true;
        playgroundAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.playgroundOut);
    }

    public void StakeClicked()
    {
        stakeHeader.alpha = 1;
        unstakeHeader.alpha = 0.5f;

        stakeContent.SetActive(true);
        unstakeContent.SetActive(false);

        SoundManager.instance.PlayRandomFromList(SoundManager.instance.clickSounds);
    }

    public void UnstakeClicked()
    {
        stakeHeader.alpha = 0.5f;
        unstakeHeader.alpha = 1f;

        stakeContent.SetActive(false);
        unstakeContent.SetActive(true);

        SoundManager.instance.PlayRandomFromList(SoundManager.instance.clickSounds);
    }

    public void YeetClicked()
    {
        DebugUtils.Log("The command is here");
        YeetHeader.alpha = 1;
        YeetRewardsHeader.alpha = 0.5f;

        YeetContent.SetActive(true);
        YeetRewardsContent.SetActive(false);

        SoundManager.instance.PlayRandomFromList(SoundManager.instance.clickSounds);
    }

    public void YeetRewardsClicked()
    {
        YeetHeader.alpha = 0.5f;
        YeetRewardsHeader.alpha = 1f;

        YeetContent.SetActive(false);
        YeetRewardsContent.SetActive(true);

        SoundManager.instance.PlayRandomFromList(SoundManager.instance.clickSounds);
    }

    public void PvPPanelIn()
    {
        PvPPanelAnimator.SetTrigger("in");
        SoundManager.instance.PlaySfx(SoundManager.instance.playgroundIn);
    }

    public void PvPPanelOut()
    {
        PvPPanelAnimator.SetTrigger("out");
        SoundManager.instance.PlaySfx(SoundManager.instance.playgroundOut);
    }

    public void ChainsawMintIn()
    {
        chainsawMintPanel.SetActive(true);
        chainsawMintPanelCanvasGroup.DOFade(1, 0.25f);
        chainsawMintPanelCanvasGroup.interactable = true;
        chainsawMintPanelCanvasGroup.blocksRaycasts = true;
    }

    public void ChainsawMintOut()
    {
        chainsawMintPanelCanvasGroup.alpha = 0;
        chainsawMintPanelCanvasGroup.interactable = false;
        chainsawMintPanelCanvasGroup.blocksRaycasts = false;
        chainsawMintPanel.SetActive(false);
    }

    public void WebsiteButton()
    {
        Application.OpenURL("https://docs.stakenbake.xyz/how-to-play/");
    }

    public void YeetHandbookButton()
    {
        Application.OpenURL("https://mirror.xyz/0xEF031D9a242a08EaF300f93Df2Dc23Cb59A4c555/bAcqqJbdz24XrhdffvkqjvdUZPQZqjPQmLE6O_vRdWI");
    }
}
