using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPOI : MonoBehaviour
{
    public string poiName;
    public string questLogText;
    public bool visited = false;

    // Assuming you are using Unity's Trigger system to detect when the player enters the POI
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialQuestManager questManager = FindObjectOfType<TutorialQuestManager>();

            if (!visited)
            {
                questManager.ReachPOI(this);
            }

        }
    }

    public void TriggerEvent()
    {
        GetComponent<DialogueTrigger>().StartDialogue();
    }
}
