using Gamegaard;
using Gamegaard.Singleton;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class LocalGameManager : MonoBehaviourSingleton<LocalGameManager>
{
    [SerializeField] private PlayerController player;
    [SerializeField] private DecreaseClockTimer fracturedRealmsTimer;
    [SerializeField] private GameObject hud;

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
    public int currentFarmLandAmount = 0;

    public string currentPlayerName;

    public event Action<int> OnFarmLandAdded;
    public event Action<bool> OnFarmHighlighted;

    public GameObject Hud => hud;
    public DecreaseClockTimer FracturedRealmsTimer => fracturedRealmsTimer;
    public PlayerController PlayerController
    {
        get
        {
            if (player == null)
            {
                player = FindFirstObjectByType<PlayerController>();
            }
            return player;
        }
    }

    public void AddFarmLandsHighlight()
    {
        OnFarmHighlighted?.Invoke(true);
    }

    public void RemoveFarmLandsHighlight()
    {
        OnFarmHighlighted?.Invoke(false);
    }

    public void AddFarmLand()
    {
        if (currentFarmLandAmount >= 5) return;
        currentFarmLandAmount++;
        OnFarmLandAdded?.Invoke(currentFarmLandAmount);
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

    public void EnableWeedHighEffect(float duration)
    {
        GameObject.Find("HighEffectPostProcessing").GetComponent<PostProcessVolume>().enabled = true;
        // DrunkEffectScript drunkEffectScript = Camera.main.transform.GetComponent<DrunkEffectScript>();
        var drunkEffectScript = FindObjectOfType<DrunkEffectScript>();
        drunkEffectScript.enabled = true;
        drunkEffectScript.effectWeight = Mathf.Lerp(0, 1, 3);
        drunkEffectScript.StartFadeIn(duration);
        // SoundManager.instance.PlaySfx(SoundManager.instance.weedUsed); // added to the useaction
    }

    public void DisableWeedHighEffect()
    {
        StartCoroutine(DisableWeedHighEffectRoutine());
    }

    private IEnumerator DisableWeedHighEffectRoutine()
    {
        // DrunkEffectScript drunkEffectScript = Camera.main.transform.GetComponent<DrunkEffectScript>();
        var drunkEffectScript = FindObjectOfType<DrunkEffectScript>();
        drunkEffectScript.effectWeight = Mathf.Lerp(1, 0, 3);
        drunkEffectScript.StartFadeOut(0.5f);
        // SoundManager.Instance.PlaySfx(SoundManager.Instance.weedEnded);
        yield return new WaitForSeconds(0.5f);
        GameObject.Find("HighEffectPostProcessing").GetComponent<PostProcessVolume>().enabled = false;
        drunkEffectScript.enabled = false;
    }

    public bool CheckForItem(Item item)
    {
        return inventoryContainer.ContainsItem(item);
    }

    public int CheckForItemQuantity(Item item)
    {
        return inventoryContainer.ContainsItemQuantity(item);
    }

    // you can use this to check if the player has the chainsaw item or not.
    // call it after loading the game state 
    public bool CheckChainsaw()
    {
        return CheckForItem(chainsawItem);
    }
}
