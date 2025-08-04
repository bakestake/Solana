using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private TutorialPanel.TutorialPanelParams[] tutorialPanels;

        private void Start()
        {
            TutorialPopUp.Instance.GeneratePanels(tutorialPanels);
        }
    }
}