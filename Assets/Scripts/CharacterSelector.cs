using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Bakeland;
using System.Threading.Tasks;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;

    private int currentIndex = 0;
    private int currentRotation = 0; // 0: Front, 1: Right, 2: Back, 3: Left

    [Header("Settings")]
    [SerializeField] private int minNameChar = 3;
    [SerializeField] private int maxNameChar = 12;
    public bool forceChooseUsername = false;

    [Header("Character References")]
    public CharacterSelectorChar[] characters;
    public Image characterImage;

    [Header("References")]
    public CanvasGroup selectorScreenCanvasGroup;
    public TextMeshProUGUI characterNameText;
    public TMP_InputField usernameInput;
    public Button confirmButton;

    public static event Action<CharacterSelectorChar> OnCharacterSelected;
    public event Action OnConfirm;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        OnCharacterSelected?.Invoke(characters[0]);
        if (selectorScreenCanvasGroup != null)
        {
            selectorScreenCanvasGroup.alpha = 0;
            selectorScreenCanvasGroup.interactable = false;
            selectorScreenCanvasGroup.blocksRaycasts = false;
        }

        usernameInput.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
        usernameInput.onValueChanged.AddListener(OnTypingName);
        // confirmButton.interactable = false;

        UpdateCharacterDisplay();
    }

    private void OnTypingName(string value)
    {
        confirmButton.interactable = value.Length == 0 || CheckNameChar(value);
    }

    public void OpenCharacterSelection()
    {
        PlayerController.canMove = false;
        PlayerController.canInteract = false;

        selectorScreenCanvasGroup.DOFade(1, 1);
        selectorScreenCanvasGroup.interactable = true;
        selectorScreenCanvasGroup.blocksRaycasts = true;
        confirmButton.interactable = UserRegistration.username != null;
        usernameInput.text = "";

        // nameInput.interactable = string.IsNullOrEmpty(PlayerController.Instance.username);
    }

    public void CloseCharacterSelection()
    {
        PlayerController.canMove = true;
        PlayerController.canInteract = true;
        selectorScreenCanvasGroup.DOFade(0, 1);
        selectorScreenCanvasGroup.interactable = false;
        selectorScreenCanvasGroup.blocksRaycasts = false;
    }

    public bool CheckNameChar(string name)
    {
        return name.Length >= minNameChar && name.Length <= maxNameChar;
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
        Animator playerAnimator = LocalGameManager.Instance.PlayerController.GetComponent<Animator>();

        foreach (CharacterSelectorChar character in characters)
        {
            if (character.characterName == characterName)
            {
                CharacterAvatarImage[] avatarImages = FindObjectsOfType<CharacterAvatarImage>(true);
                playerAnimator.runtimeAnimatorController = character.animatorOverrideController;
                OnCharacterSelected?.Invoke(character);
                break;
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
        CharacterSelectorChar character = GetSelectedCharacter();
        LocalGameManager.Instance.PlayerController.GetComponent<Animator>().runtimeAnimatorController = character.animatorOverrideController;
        CloseCharacterSelection();
        PlayerPrefs.SetString("character", character.characterName);
        // if (string.IsNullOrEmpty(PlayerController.Instance.username)) PlayerController.Instance.username = nameInput.text;
        OnCharacterSelected?.Invoke(character);

        yield return new WaitForSeconds(1);

        if (String.IsNullOrEmpty(usernameInput.text) == false)
        {
            if (String.IsNullOrEmpty(UserRegistration.username))
            {
                Task task = UserAndReferralApi.CreateUser(WalletConnectScript.connectedWalletAddress, usernameInput.text, ("0x" + usernameInput.text), "", WalletConnectScript.connectedWalletAddress);
                yield return new WaitUntil(() => task.IsCompleted);
                PlayerController.Instance.SetUsername(usernameInput.text);
                UserRegistration.SetUserInfo(usernameInput.text, ("0x" + usernameInput.text));
                FindObjectOfType<TutorialManager>(true).gameObject.SetActive(true);
            }
            else
            {
                Task task = UserAndReferralApi.UpdateUsername(WalletConnectScript.connectedWalletAddress, usernameInput.text);
                yield return new WaitUntil(() => task.IsCompleted);
                PlayerController.Instance.SetUsername(usernameInput.text);
                UserRegistration.SetUserInfo(usernameInput.text, UserRegistration.referralCode);
            }
        }

        PlayerController.canMove = true;
        PlayerController.canInteract = true;
        LoadingWheel.instance.DisableLoading();
        OnConfirm?.Invoke();
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }
}
