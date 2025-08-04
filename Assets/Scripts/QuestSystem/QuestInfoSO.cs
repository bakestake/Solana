using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }
    [field: SerializeField] public int uid { get; private set; }

    [Header("General")]
    public string displayName;
    public Dialogue questStartDialogue;
    public Dialogue questProgressDialogue;
    public Dialogue questFinishDialogue;

    [Header("Requirements")]
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int goldReward;
    public Item itemReward;
    public int itemRewardAmount;

    [Header("Delete on completion")]
    public Item itemDelete;
    public int itemDeleteAmount;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
