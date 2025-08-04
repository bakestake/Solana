using TMPro;
using UnityEngine;

namespace Bakeland
{
    public class QuestPopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text questTitleText;
        [SerializeField] private AudioClip finishAudioClip;

        private bool playAudio = true;
        private TypewriterWrapper typewriter;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            typewriter = GetComponent<TypewriterWrapper>();
        }

        private void OnEnable()
        {
            GameEventsManager.NotNullInstance.questEvents.onFinishQuest += PlayAnimation;
        }

        private void OnDisable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.questEvents.onFinishQuest -= PlayAnimation;
        }

        public void PlayAnimation(string questId)
        {
            Quest quest = QuestManager.Instance.GetQuestById(questId);
            typewriter.text = $"{quest.info.displayName}";
            playAudio = true;
            animator.SetTrigger("play");
        }

        public void PlayCustomAnimation(string text, bool showHeader, bool playAudio)
        {
            typewriter.text = text;
            this.playAudio = playAudio;
            if (showHeader) animator.SetTrigger("play");
            else animator.SetTrigger("playNoHeader");
        }

        public void TryPlayAudio()
        {
            if (playAudio) SoundManager.Instance.PlaySfx(finishAudioClip);
        }
    }
}