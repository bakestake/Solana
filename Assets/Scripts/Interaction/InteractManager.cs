using DG.Tweening;
using Gamegaard.Singleton;
using TMPro;
using UnityEngine;

public class InteractManager : MonoBehaviourSingleton<InteractManager>
{
    [SerializeField] private FastTravelPanel fastTravelPanel;
    [SerializeField] private Animator mapAnimator;
    [SerializeField] private Animator handbookAnimator;
    [SerializeField] private Animator settingsAnimator;
    [SerializeField] private Animator highlightHandbookAnimator;
    [SerializeField] private Animator highlightNetworkAnimator;
    [SerializeField] private Animator faucetAnimator;
    [SerializeField] private Animator bulletinBoardAnimator;
    [SerializeField] private Animator farmAnimator;
    [SerializeField] private Animator yeetAnimator;
    [SerializeField] private Animator policeAnimator;
    [SerializeField] private Animator blackMarketAnimator;
    [SerializeField] private Animator playgroundAnimator;
    [SerializeField] private Animator txAnimator;
    [SerializeField] private Animator yeetBoothNoticeAnimator;
    [SerializeField] private Animator PvPPanelAnimator;
    [SerializeField] private TextMeshProUGUI txFirstText;

    [Header("Stake&Unstake")]
    [SerializeField] private CanvasGroup stakeHeader;
    [SerializeField] private CanvasGroup unstakeHeader;
    [SerializeField] private GameObject stakeContent;
    [SerializeField] private GameObject unstakeContent;

    [Header("Yeet&Rewards")]
    [SerializeField] private CanvasGroup YeetHeader;
    [SerializeField] private CanvasGroup YeetRewardsHeader;
    [SerializeField] private GameObject YeetContent;
    [SerializeField] private GameObject YeetRewardsContent;

    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup PvPPanelCanvasGroup;

    [Header("References")]
    [SerializeField] private GameObject chainsawMintPanel;
    [SerializeField] private CanvasGroup chainsawMintPanelCanvasGroup;

    public Animator TxAnimator => txAnimator;

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
        SoundManager.Instance.PlaySfx(SoundManager.Instance.handbookIn);
    }

    public void SettingsOut()
    {
        //PlayerController.canMove = true;
        settingsAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.handbookOut);
    }

    public void HandbookIn()
    {
        //PlayerController.canMove = false;
        handbookAnimator.SetTrigger("in");
        highlightHandbookAnimator.SetTrigger("out");
        highlightNetworkAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.handbookIn);
    }

    public void HandbookOut()
    {
        //PlayerController.canMove = true;
        handbookAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.handbookOut);
    }

    public void FaucetIn()
    {
        //PlayerController.canMove = false;
        faucetAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.faucetIn);
    }

    public void FaucetOut()
    {
        //PlayerController.canMove = true;
        faucetAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.faucetOut);
    }

    public void BulletinBoardIn()
    {
        //PlayerController.canMove = false;
        bulletinBoardAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.bulletinBoardIn);
    }

    public void BulletinBoardOut()
    {
        //PlayerController.canMove = true;
        bulletinBoardAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.bulletinBoardOut);
    }

    public void YeetBoothNoticeIn()
    {
        yeetBoothNoticeAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.bulletinBoardIn);
    }

    public void YeetBoothNoticeOut()
    {
        yeetBoothNoticeAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.bulletinBoardOut);
    }

    public void YeetPanelIn()
    {
        PlayerController.canMove = false;
        yeetAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.farmIn);
        YeetClicked();
    }

    public void YeetPanelOut()
    {
        PlayerController.canMove = true;
        yeetAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.farmOut);
    }

    public void FarmIn()
    {
        //PlayerController.canMove = false;
        farmAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.farmIn);
        StakeClicked();
    }

    public void FarmOut()
    {
        //PlayerController.canMove = true;
        farmAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.farmOut);
    }

    public void FarmStakeStart()
    {
        // STAKE CODE

        //PlayerController.canMove = false;
        SoundManager.Instance.PlaySfx(SoundManager.Instance.txIn);
        txAnimator.SetTrigger("in");
        // TMP_Dropdown dropdown = farmAnimator.gameObject.GetComponentInChildren<TMP_Dropdown>();
        // string selectedChain = dropdown.options[dropdown.value].text;
        // txFirstText.text = "Sending across your BUDS to " + selectedChain;
    }

    public void PoliceIn()
    {
        //PlayerController.canMove = false;
        policeAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.policeIn);
    }

    public void PoliceOut()
    {
        //PlayerController.canMove = true;
        policeAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.policeOut);
    }

    public void PoliceRaidStart()
    {
        // RAID CODE
        //PlayerController.canMove = false;
        SoundManager.Instance.PlaySfx(SoundManager.Instance.txIn);
        // TMP_Dropdown dropdown = policeAnimator.gameObject.GetComponentInChildren<TMP_Dropdown>();
        // string selectedChain = dropdown.options[dropdown.value].text;
        // txFirstText.text = "Initiating raid on " + selectedChain + ". Please wait 420 seconds.";
        txAnimator.SetTrigger("in");
    }

    public void TXClose()
    {
        //PlayerController.canMove = true;
        SoundManager.Instance.PlaySfx(SoundManager.Instance.txOut);
        txAnimator.SetTrigger("out");
    }

    public void BlackMarketIn()
    {
        //PlayerController.canMove = false;
        blackMarketAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.blackMarketIn);
    }

    public void BlackMarketOut()
    {
        //PlayerController.canMove = true;
        blackMarketAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.blackMarketOut);
    }

    public void PlaygroundIn()
    {
        //PlayerController.canMove = false;
        playgroundAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.playgroundIn);
    }

    public void PlaygroundOut()
    {
        //PlayerController.canMove = true;
        playgroundAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.playgroundOut);
    }

    public void StakeClicked()
    {
        stakeHeader.alpha = 1;
        unstakeHeader.alpha = 0.5f;

        stakeContent.SetActive(true);
        unstakeContent.SetActive(false);

        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.clickSounds);
    }

    public void UnstakeClicked()
    {
        stakeHeader.alpha = 0.5f;
        unstakeHeader.alpha = 1f;

        stakeContent.SetActive(false);
        unstakeContent.SetActive(true);

        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.clickSounds);
    }

    public void YeetClicked()
    {
        DebugUtils.Log("The command is here");
        YeetHeader.alpha = 1;
        YeetRewardsHeader.alpha = 0.5f;

        YeetContent.SetActive(true);
        YeetRewardsContent.SetActive(false);

        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.clickSounds);
    }

    public void YeetRewardsClicked()
    {
        YeetHeader.alpha = 0.5f;
        YeetRewardsHeader.alpha = 1f;

        YeetContent.SetActive(false);
        YeetRewardsContent.SetActive(true);

        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.clickSounds);
    }

    public void PvPPanelIn()
    {
        PvPPanelAnimator.SetTrigger("in");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.playgroundIn);
    }

    public void PvPPanelOut()
    {
        PvPPanelAnimator.SetTrigger("out");
        SoundManager.Instance.PlaySfx(SoundManager.Instance.playgroundOut);
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
