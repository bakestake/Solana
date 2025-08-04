using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private bool autoStartFirstQuest = true;

        [Header("Debug")]
        [SerializeField] private bool skipTutorial = false;

        [Header("References")]
        [SerializeField] private QuestPopUp questPopUpManager;
        [SerializeField] private HighlightManager highlightManager;
        [SerializeField] private CharacterSelector characterSelector;
        [SerializeField] private Button characterSelectButton, characterConfirmButton;
        [SerializeField] private Button handbookOpenButton, handbookCloseButton;
        [SerializeField] private QuestInfoSO questInfo;

        private void LateUpdate()
        {
            // TO-DO: this is a TERRIBLE TEMPORARY FIX
            // other elements are setting canMove and canInteract to true, so force it to false
            PlayerController.canMove = false;
            PlayerController.canInteract = false;
        }

        private void OnEnable()
        {
            PlayerController.canMove = false;
            PlayerController.canInteract = false;

            if (skipTutorial)
            {
                TryStartQuest();
                highlightManager.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(Coroutine_ManageSteps());
            }
        }

        private void OnDisable()
        {
            PlayerController.canMove = true;
            PlayerController.canInteract = true;
        }

        private IEnumerator Coroutine_ManageSteps()
        {
            Debug.Log($"{name}: start");
            questPopUpManager.PlayCustomAnimation("Welcome to Bakeland", showHeader: false, playAudio: true);
            yield return new WaitForSeconds(8f); // TO-DO: remove hardcoded time

            Debug.Log($"{name}: step one");
            bool isUsernameSet = false;
            characterSelector.OnConfirm += () => isUsernameSet = true;
            characterSelector.forceChooseUsername = true;
            characterSelectButton.onClick.Invoke();
            yield return new WaitUntil(() => isUsernameSet);
            characterSelector.OnConfirm -= () => isUsernameSet = true;
            characterSelector.forceChooseUsername = false;

            Debug.Log($"{name}: step two");
            bool isHighlightFinished = false;
            highlightManager.gameObject.SetActive(true);
            highlightManager.onFinished += () => isHighlightFinished = true;
            yield return new WaitUntil(() => isHighlightFinished);
            highlightManager.onFinished -= () => isHighlightFinished = true;

            Debug.Log($"{name}: step three");
            bool isHandbookFinished = false;
            handbookOpenButton.onClick.Invoke();
            handbookCloseButton.onClick.AddListener(() => isHandbookFinished = true);
            yield return new WaitUntil(() => isHandbookFinished);
            handbookCloseButton.onClick.RemoveListener(() => isHandbookFinished = true);

            Debug.Log($"{name}: finished");
            TryStartQuest();
            highlightManager.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }

        private void TryStartQuest()
        {
            bool isQuestFinished = QuestManager.Instance.GetQuestById(questInfo.id).state == QuestState.FINISHED;
            if (!isQuestFinished)
            {
                if (autoStartFirstQuest) GameEventsManager.Instance.questEvents.StartQuest(questInfo.id);
                else NPCManager.GetNPC("Puff the Bear").SetActive(true);
            }
        }
    }
}