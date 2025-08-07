using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;

    private int currentIndex = 0;
    private int currentRotation = 0; // 0: Front, 1: Right, 2: Back, 3: Left

    [Header("Settings")]
    [SerializeField] private int minNameChar;

    [Header("Character References")]
    public CharacterSelectorChar[] characters;
    public Image characterImage;

    [Header("References")]
    public CanvasGroup selectorScreenCanvasGroup;
    public TextMeshProUGUI characterNameText;
    public TMP_InputField nameInput;
    public Button confirmButton;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if (selectorScreenCanvasGroup != null)
        {
            selectorScreenCanvasGroup.alpha = 0;
            selectorScreenCanvasGroup.interactable = false;
            selectorScreenCanvasGroup.blocksRaycasts = false;
        }

        // confirmButton.interactable = false;
        UpdateCharacterDisplay();
    }

    public void OpenCharacterSelection()
    {
        PlayerController.canMove = false;

        selectorScreenCanvasGroup.DOFade(1, 1);
        selectorScreenCanvasGroup.interactable = true;
        selectorScreenCanvasGroup.blocksRaycasts = true;
    }

    public void CloseCharacterSelection()
    {
        PlayerController.canMove = true;
        Debug.Log("Can Move true");

        selectorScreenCanvasGroup.DOFade(0, 1);
        selectorScreenCanvasGroup.interactable = false;
        selectorScreenCanvasGroup.blocksRaycasts = false;
    }

    public void CheckNameChar()
    {
        confirmButton.interactable = nameInput.text.Length >= minNameChar;
    }

    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void RotateLeft()
    {
        currentRotation = (currentRotation - 1 + 4) % 4;
        UpdateCharacterDisplay();
    }

    public void RotateRight()
    {
        currentRotation = (currentRotation + 1) % 4;
        UpdateCharacterDisplay();
    }

    private void UpdateCharacterDisplay()
    {
        CharacterSelectorChar currentCharacter = characters[currentIndex];
        characterNameText.text = currentCharacter.characterName;

        // Update the character image based on the current rotation
        switch (currentRotation)
        {
            case 0: // Front
                characterImage.sprite = currentCharacter.front;
                break;
            case 1: // Right
                characterImage.sprite = currentCharacter.right;
                break;
            case 2: // Back
                characterImage.sprite = currentCharacter.back;
                break;
            case 3: // Left
                characterImage.sprite = currentCharacter.left;
                break;
        }
    }

    public CharacterSelectorChar GetSelectedCharacter()
    {
        return characters[currentIndex];
    }

    public void SetCharacter(string characterName)
    {
        Animator playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        foreach (CharacterSelectorChar character in characters)
        {
            if (character.characterName == characterName)
            {
                playerAnimator.runtimeAnimatorController = character.animatorOverrideController;
            }
        }
    }

    public void SelectCharacter()
    {
        StartCoroutine(SelectCharacterRoutine());
    }

    private IEnumerator SelectCharacterRoutine()
    {
        LoadingWheel.instance.EnableLoading();

        // LocalGameManager.Instance.SetPlayerName(nameInput.text);
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().runtimeAnimatorController = GetSelectedCharacter().animatorOverrideController;
        CloseCharacterSelection();
        PlayerPrefs.SetString("character", GetSelectedCharacter().characterName);

        yield return new WaitForSeconds(1);

        // gameObject.SetActive(false);
        PlayerController.canMove = true;
        Debug.Log("Can Move true");
        Debug.Log("Character selection is successful!");

        LoadingWheel.instance.DisableLoading();
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }
}
