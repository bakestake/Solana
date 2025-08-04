using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Bakeland
{
    public class CutsceneBehaviours : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private UnityEvent OnStart;
        [SerializeField] private UnityEvent OnEnd;

        private GameObject player;
        private bool isQuitting;

        private void Reset()
        {
            director = GetComponent<PlayableDirector>();
        }

        private void Awake()
        {
            player = LocalGameManager.Instance.PlayerController.gameObject;

            if (director != null)
            {
                director.played += OnDirectorPlayed;
                director.stopped += OnDirectorStopped;
            }

            OnDirectorPlayed(director);
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        private void OnDestroy()
        {
            if (isQuitting) return;
            if (director != null)
            {
                director.played -= OnDirectorPlayed;
                director.stopped -= OnDirectorStopped;
            }
        }

        private void OnDirectorPlayed(PlayableDirector obj)
        {
            LocalGameManager.Instance.Hud.SetActive(false);
            OnStart?.Invoke();
            HidePlayer();
        }

        private void OnDirectorStopped(PlayableDirector obj)
        {
            LocalGameManager.Instance.Hud.SetActive(true);
            OnEnd?.Invoke();
            ShowPlayer();
        }

        public void ShowPlayer()
        {
            player.SetActive(true);
        }

        public void HidePlayer()
        {
            player.SetActive(false);
        }

        public void SetPlayerToTargetPosition(Transform target)
        {
            player.transform.position = target.position;
        }
    }
}
