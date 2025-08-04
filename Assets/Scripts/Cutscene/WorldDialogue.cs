using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bakeland
{
    public class WorldDialogue : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private RectTransform dialogueBalloon;
        [SerializeField] private float minWidth = 100f;
        [SerializeField] private float maxWidth = 400f;
        [SerializeField] private float minHeight = 50f;
        [SerializeField] private float paddingWidth = 40f;
        [SerializeField] private float paddingHeight = 20f;
        [SerializeField] private float animationDuration = 0.3f;

        [Header("Emoji")]
        [SerializeField] private Image emojiImage;

        private Coroutine dialogueCoroutine;
        private Coroutine emojiCoroutine;

        private GameObject DialogueBoxGameObject => dialogueBalloon.gameObject;
        private GameObject EmojiGameObject => emojiImage.gameObject;
        public bool IsVisible => DialogueBoxGameObject.activeSelf;

        private void Awake()
        {
            DialogueBoxGameObject.SetActive(false);
            EmojiGameObject.SetActive(false);

            dialogueBalloon.pivot = new Vector2(0f, 0f);
        }

        public void ShowDialogue(string text)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                SetDialogueImmediate(text);
                return;
            }
#endif
            if (dialogueCoroutine != null)
                StopCoroutine(dialogueCoroutine);

            dialogueCoroutine = StartCoroutine(ExpandDialogue(text));
        }

        public void HideDialogue()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                HideDialogueImmediate();
                return;
            }
#endif
            if (dialogueCoroutine != null)
                StopCoroutine(dialogueCoroutine);

            dialogueCoroutine = StartCoroutine(CollapseDialogue());
        }

        public void ShowEmoji(Sprite emoji)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                SetEmojiImmediate(emoji);
                return;
            }
#endif
            if (emojiCoroutine != null)
                StopCoroutine(emojiCoroutine);

            emojiCoroutine = StartCoroutine(FadeInEmoji(emoji));
        }

        public void HideEmoji()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                HideEmojiImmediate();
                return;
            }
#endif
            if (emojiCoroutine != null)
                StopCoroutine(emojiCoroutine);

            emojiCoroutine = StartCoroutine(FadeOutEmoji());
        }

        private void SetDialogueImmediate(string text)
        {
            DialogueBoxGameObject.SetActive(true);
            dialogueText.enableWordWrapping = false;
            dialogueText.text = text;

            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.rectTransform);

            float calculatedWidth = dialogueText.preferredWidth + paddingWidth;
            float finalWidth = Mathf.Clamp(calculatedWidth, minWidth, maxWidth);

            bool shouldWrap = calculatedWidth > maxWidth;
            dialogueText.enableWordWrapping = shouldWrap;
            dialogueText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalWidth);

            LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.rectTransform);

            float calculatedHeight = dialogueText.preferredHeight + paddingHeight;
            float finalHeight = Mathf.Max(calculatedHeight, minHeight);

            dialogueBalloon.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalWidth);
            dialogueBalloon.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalHeight);
            dialogueText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalHeight);

            dialogueBalloon.localScale = Vector3.one;
        }

        private void HideDialogueImmediate()
        {
            dialogueBalloon.localScale = Vector3.zero;
            DialogueBoxGameObject.SetActive(false);
        }

        private void SetEmojiImmediate(Sprite emoji)
        {
            emojiImage.sprite = emoji;
            EmojiGameObject.SetActive(true);

            Color color = emojiImage.color;
            color.a = 1f;
            emojiImage.color = color;
        }

        private void HideEmojiImmediate()
        {
            Color color = emojiImage.color;
            color.a = 0f;
            emojiImage.color = color;
            EmojiGameObject.SetActive(false);
        }

        private IEnumerator ExpandDialogue(string text)
        {
            SetDialogueImmediate(text);
            dialogueBalloon.localScale = Vector3.zero;

            float time = 0f;
            while (time < animationDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / animationDuration;
                t = Mathf.SmoothStep(0f, 1f, t);
                dialogueBalloon.localScale = new Vector3(t, t, 1f);
                yield return null;
            }

            dialogueBalloon.localScale = Vector3.one;
        }

        private IEnumerator CollapseDialogue()
        {
            float time = 0f;
            while (time < animationDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / animationDuration;
                t = Mathf.SmoothStep(0f, 1f, t);
                float scale = Mathf.Lerp(1f, 0f, t);
                dialogueBalloon.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            HideDialogueImmediate();
        }

        private IEnumerator FadeInEmoji(Sprite emoji)
        {
            emojiImage.sprite = emoji;
            EmojiGameObject.SetActive(true);
            emojiImage.color = new Color(emojiImage.color.r, emojiImage.color.g, emojiImage.color.b, 0f);

            float time = 0f;
            while (time < animationDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / animationDuration;
                t = Mathf.SmoothStep(0f, 1f, t);
                Color color = emojiImage.color;
                color.a = t;
                emojiImage.color = color;
                yield return null;
            }

            SetEmojiImmediate(emoji);
        }

        private IEnumerator FadeOutEmoji()
        {
            float startAlpha = emojiImage.color.a;
            float time = 0f;
            while (time < animationDuration)
            {
                time += Time.unscaledDeltaTime;
                float t = time / animationDuration;
                t = Mathf.SmoothStep(0f, 1f, t);
                float alpha = Mathf.Lerp(startAlpha, 0f, t);
                Color color = emojiImage.color;
                color.a = alpha;
                emojiImage.color = color;
                yield return null;
            }

            HideEmojiImmediate();
        }
    }
}
