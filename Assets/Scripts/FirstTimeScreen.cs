using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;
using TMPro;

public class FirstTimeScreen : MonoBehaviour
{
    public TypewriterByWord typewriter;
    public Animator highlightHandbookAnim;
    public Animator highlightNetworkAnim;
    public GameObject questPointerPf;
    public Transform target;
    public TextMeshProUGUI letterText;

    public void PlaySound(AudioClip clip)
    {
        SoundManager.Instance.PlaySfx(clip);
    }

    public void StartTyping()
    {
        typewriter.ShowText(letterText.text);
        typewriter.StartShowingText();
    }

    public void HighlightHandbookIn()
    {
        highlightHandbookAnim.SetTrigger("in");
        highlightNetworkAnim.SetTrigger("in");
    }

    public void SetQuestToFaucet()
    {
        GameObject player = GameObject.Find("Player");
        GameObject questPointer = Instantiate(questPointerPf, player.transform);
        questPointer.GetComponent<QuestPointer>().Target = target;
    }
}
