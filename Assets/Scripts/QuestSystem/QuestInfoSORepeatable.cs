using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSORepeatable", menuName = "ScriptableObjects/QuestInfoSORepeatable", order = 1)]
public class QuestInfoSORepeatable : QuestInfoSO
{
    [Header("Repeatable")]
    public Dialogue startDialogueRepeat;
    public Dialogue progressDialogueRepeat;
    public Dialogue finishDialogueRepeat;
}
