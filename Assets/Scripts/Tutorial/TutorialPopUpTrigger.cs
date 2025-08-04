using UnityEngine;

namespace Bakeland
{
    public class TutorialPopUpTrigger : MonoBehaviour
    {
        [SerializeField] private TutorialPanel.TutorialPanelParams[] tutorial;

        public void Trigger()
        {
            TutorialPopUp.Instance.GeneratePanels(tutorial);
        }
    }
}