using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class HighlightManager : MonoBehaviour
    {
        [SerializeField] private Image blackBackground;

        [SerializeField] private Image settingsHighlight;
        [SerializeField] private Button settingsButton, settingsCloseButton;

        [SerializeField] private Image characterSelectHighlight;
        [SerializeField] private Button characterSelectButton;

        [SerializeField] private Image cloudSaveHighlight;
        [SerializeField] private Button cloudSaveButton;

        [SerializeField] private Image inventoryHighlight;
        [SerializeField] private Button inventoryButton, inventoryCloseButton;

        public Action onFinished;

        private int currentStep;

        private void Start()
        {
            currentStep = 0;
            blackBackground.gameObject.SetActive(true);
            settingsHighlight.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            settingsButton.onClick.AddListener(() => OnClickSettings());
            settingsCloseButton.onClick.AddListener(() => OnCloseSettings());
            inventoryButton.onClick.AddListener(() => OnClickInventory());
            inventoryCloseButton.onClick.AddListener(() => OnCloseInventory());
            // cloudSaveButton.onClick.AddListener(() => OnClickCloudSave());
            // characterSelectButton.onClick.AddListener(() => OnClickCharacterSelect());

            // settingsCloseButton.interactable = false;
            // cloudSaveButton.interactable = false;
        }

        private void OnDisable()
        {
            settingsButton.onClick.RemoveListener(() => OnClickSettings());
            settingsCloseButton.onClick.RemoveListener(() => OnCloseSettings());
            inventoryButton.onClick.RemoveListener(() => OnClickInventory());
            inventoryCloseButton.onClick.RemoveListener(() => OnCloseInventory());
            // cloudSaveButton.onClick.RemoveListener(() => OnClickCloudSave());
            // characterSelectButton.onClick.RemoveListener(() => OnClickCharacterSelect());

            blackBackground.gameObject.SetActive(false);
            settingsHighlight.gameObject.SetActive(false);
            characterSelectHighlight.gameObject.SetActive(false);
            inventoryHighlight.gameObject.SetActive(false);
            // cloudSaveHighlight.gameObject.SetActive(false);

            // settingsCloseButton.interactable = true;
            // cloudSaveButton.interactable = true;
        }

        private void OnClickSettings()
        {
            if (currentStep == 0)
            {
                settingsHighlight.gameObject.SetActive(false);
                currentStep++;
            }
        }

        private void OnCloseSettings()
        {
            if (currentStep == 1)
            {
                inventoryHighlight.gameObject.SetActive(true);
                currentStep++;
            }
        }

        private void OnClickInventory()
        {
            if (currentStep == 2)
            {
                inventoryHighlight.gameObject.SetActive(false);
                currentStep++;
            }
        }

        private void OnCloseInventory()
        {
            if (currentStep == 3)
            {
                currentStep++;
                onFinished.Invoke();

                // calls OnDisable which will hide everything
                this.gameObject.SetActive(false);
            }
        }

        /*
        private void OnClickCloudSave()
        {
            if (currentStep == 4)
            {
                currentStep++;
                onFinished.Invoke();

                // calls OnDisable which will hide everything
                this.gameObject.SetActive(false);
            }
        }

        private void OnClickCharacterSelect()
        {
            if (currentStep == 1)
            {
                characterSelectHighlight.gameObject.SetActive(false);
                inventoryHighlight.gameObject.SetActive(true);
                settingsCloseButton.interactable = true;
                currentStep++;
            }
        }

        private void OnClickInventory()
        {
            if (currentStep == 2)
            {
                inventoryHighlight.gameObject.SetActive(false);
                cloudSaveHighlight.gameObject.SetActive(true);
                cloudSaveButton.interactable = true;
                currentStep++;
            }
        }
        */
    }
}