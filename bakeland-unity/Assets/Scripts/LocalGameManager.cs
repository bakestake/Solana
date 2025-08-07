using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager Instance;

    public Animator transitionAnimator;
    public Animator firstTimeSceneAnimator;
    public bool firstTime;
    public DialogueTrigger welcomeDialogue;
    public CharacterSelector characterSelector;
    public TutorialInitializer tutorialInitializer;
    public ItemDragAndDropController dragAndDropController;
    public ItemContainer inventoryContainer;
    public ItemContainer currentShopContainer;
    public bool canUseKeybinds = false;
    public Item chainsawItem;
    public Transform playerSpawnPoint;

    public string currentPlayerName;

    private void Awake()
    {
        //PlayerController.canMove = false;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void TransitionIn()
    {
        StartCoroutine(TransitionInRoutine());
    }

    private IEnumerator TransitionInRoutine()
    {
        transitionAnimator.SetTrigger("in");
        yield return new WaitForSeconds(0.5f);
    }

    public void TransitionOut()
    {
        StartCoroutine(TransitionOutRoutine());
    }

    private IEnumerator TransitionOutRoutine()
    {
        transitionAnimator.SetTrigger("out");
        yield return new WaitForSeconds(0.5f);
    }

    public void CheckFirstTime()
    {
        if (PlayerPrefs.HasKey("first"))
        {
            LoadCharacter();
            WelcomeDialogue();
        }
        else
        {
            PlayerPrefs.SetString("first", "1");
            // FirstTimeSceneIn();
            characterSelector.OpenCharacterSelection();
            tutorialInitializer.StartQuest();
        }

        canUseKeybinds = true;
    }

    public void WelcomeDialogue()
    {
        welcomeDialogue.StartDialogue();
    }

    public void FirstTimeSceneIn()
    {
        PlayerController.canMove = false;
        firstTimeSceneAnimator.SetTrigger("in");
    }

    public void FirstTimeSceneOut()
    {
        PlayerController.canMove = true;
        Debug.Log("Can Move true");
        firstTimeSceneAnimator.SetTrigger("out");
    }

    public void SetPlayerName(string playerName)
    {
        currentPlayerName = playerName;
    }

    public void LoadCharacter()
    {
        if (PlayerPrefs.HasKey("character"))
        {
            string charName = PlayerPrefs.GetString("character");
            characterSelector.SetCharacter(charName);
        }
    }
    public void SceneLoader(int ScenebuildNumber)
    {
        TransitionOut();
        SceneManager.LoadScene(ScenebuildNumber);
    }

    public void EnableWeedHighEffect()
    {
        GameObject.Find("HighEffectPostProcessing").GetComponent<PostProcessVolume>().enabled = true;
        DrunkEffectScript drunkEffectScript = Camera.main.transform.GetComponent<DrunkEffectScript>();
        drunkEffectScript.enabled = true;
        // drunkEffectScript.effectWeight = Mathf.Lerp(0, 1, 3);
        drunkEffectScript.StartFadeIn(2f);
        // SoundManager.instance.PlaySfx(SoundManager.instance.weedUsed); // added to the useaction
    }

    public void DisableWeedHighEffect()
    {
        StartCoroutine(DisableWeedHighEffectRoutine());
    }

    private IEnumerator DisableWeedHighEffectRoutine()
    {
        DrunkEffectScript drunkEffectScript = Camera.main.transform.GetComponent<DrunkEffectScript>();
        // drunkEffectScript.effectWeight = Mathf.Lerp(1, 0, 3);
        drunkEffectScript.StartFadeOut(0.5f);
        SoundManager.instance.PlaySfx(SoundManager.instance.weedEnded);
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("HighEffectPostProcessing").GetComponent<PostProcessVolume>().enabled = false;
        drunkEffectScript.enabled = false;
    }

    public bool CheckForItem(Item item)
    {
        return inventoryContainer.ContainsItem(item);
    }

    // you can use this to check if the player has the chainsaw item or not.
    // call it after loading the game state 
    public bool CheckChainsaw()
    {
        return CheckForItem(chainsawItem);
    }
}
