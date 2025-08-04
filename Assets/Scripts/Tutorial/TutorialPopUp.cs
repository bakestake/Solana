using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class TutorialPopUp : MonoBehaviour
    {
        public static TutorialPopUp Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private Button nextButton, previousButton, closeButton;
        [SerializeField] private GameObject tutorialPanelPrefab;
        [SerializeField] private Transform contentParent;

        private List<TutorialPanel> tutorialPanels = new();
        private int currentPanel;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);
        }

        private void Start()
        {
            nextButton.onClick.AddListener(() => NextPanel());
            previousButton.onClick.AddListener(() => PreviousPanel());
            closeButton.onClick.AddListener(() => Close());

            Close();
        }

        public void GeneratePanels(TutorialPanel.TutorialPanelParams[] tutorialPanelParams)
        {
            // destroy all older panels
            for (int i = 0; i < contentParent.childCount; i++)
            {
                Destroy(contentParent.GetChild(i).gameObject);
            }

            // clear old panel refs
            tutorialPanels.Clear();

            // instantiate new panels using given parameters
            foreach (TutorialPanel.TutorialPanelParams panelParams in tutorialPanelParams)
            {
                TutorialPanel newPanel = Instantiate(tutorialPanelPrefab, contentParent).GetComponent<TutorialPanel>();
                newPanel.Initialize(panelParams);
                tutorialPanels.Add(newPanel);
            }

            // initialize first panel
            panel.SetActive(true);
            currentPanel = -1;
            NextPanel();

            PlayerController.canInteract = false;
            PlayerController.canMove = false;
        }

        private void NextPanel()
        {
            if (currentPanel + 1 < tutorialPanels.Count)
            {
                foreach (TutorialPanel panel in tutorialPanels)
                {
                    panel.gameObject.SetActive(false);
                }

                currentPanel++;
                tutorialPanels[currentPanel].gameObject.SetActive(true);
            }

            UpdateButtonsVisibility();
        }

        private void PreviousPanel()
        {
            if (currentPanel - 1 < tutorialPanels.Count)
            {
                foreach (TutorialPanel panel in tutorialPanels)
                {
                    panel.gameObject.SetActive(false);
                }

                currentPanel--;
                tutorialPanels[currentPanel].gameObject.SetActive(true);
            }

            UpdateButtonsVisibility();
        }

        private void Close()
        {
            panel.SetActive(false);
            PlayerController.canInteract = true;
            PlayerController.canMove = true;
        }

        private void UpdateButtonsVisibility()
        {
            // handles all neccessary visibility for the previous, next and close buttons as panels switch around

            previousButton.gameObject.SetActive(currentPanel > 0);

            nextButton.gameObject.SetActive(currentPanel < tutorialPanels.Count);

            if (currentPanel == tutorialPanels.Count - 1)
            {
                nextButton.gameObject.SetActive(false);
                closeButton.gameObject.SetActive(true);
            }
        }
    }
}