using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bakeland
{
    public class LeaderboardManager : MonoBehaviour
    {
        public static LeaderboardManager Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject elementPrefab;
        [SerializeField] private Button closeButton;

        private readonly List<LeaderboardElement> elements = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this.gameObject);

            ClearElements();
            Hide();

            if (closeButton != null) { 
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Hide);
            }
        }

        [ContextMenu("Show")]
        public void Show()
        {
            StopCoroutine(TryPopulateContent());

            panel.SetActive(true);
            PlayerController.canInteract = false;
            PlayerController.canMove = false;
            StartCoroutine(TryPopulateContent());
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            StopCoroutine(TryPopulateContent());

            PlayerController.canInteract = true;
            PlayerController.canMove = true;
            panel.SetActive(false);
        }

        private void ClearElements()
        {
            // clear all previous elements
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }

        private IEnumerator TryPopulateContent()
        {
            ClearElements();

            LoadingWheel.instance.EnableLoading();

            var task = UserAndReferralApi.GetQuestLeaderboard("1");
            yield return new WaitUntil(() => task.IsCompleted);
            var apiResult = task.Result;

            LoadingWheel.instance.DisableLoading();

            if (task.IsCompletedSuccessfully)
            {
                foreach (var entry in apiResult.leaderboard)
                {
                    LeaderboardElement newElement = Instantiate(elementPrefab, content).GetComponent<LeaderboardElement>();
                    newElement.Initialize(entry.username, entry.score);
                    elements.Add(newElement);
                }
            }
            else
            {
                // show error element
            }
        }
    }
}
