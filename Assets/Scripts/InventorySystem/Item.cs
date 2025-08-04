using Gamegaard;
using Gamegaard.FarmSystem;
using Gamegaard.SerializableAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    [Header("Infos")]
    public string Name;
    [ContextMenuItem(nameof(GenerateUID), nameof(GenerateUID))]
    [SerializeField] private int uid = -1;
    [SerializeField] private string description;
    [SerializeField, System.Obsolete] private string id;
    [SerializeField] private Sprite icon;

    [Header("Attributes")]
    [SerializeField] private AttributesDictionary attributes;

    [Header("Behaviours")]
    [SerializeField] private ItemAction onHold;
    [SerializeField] private ItemAction onAction;
    [SerializeField] private ItemAction onItemUsed;

    [Header("Settings")]
    [SerializeField] private bool stackable;
    [SerializeField] private int maxStack = 99;
    [SerializeField] private bool unique;
    [SerializeField] private string mintFunction;
    [SerializeField] private bool isMintable;
    [SerializeField] private bool forceMintAfterAdd;

    [Header("Marketing")]
    [SerializeField] private bool marketable;
    [SerializeField] private int buyPrice;
    [SerializeField] private int sellPrice;

    [Header("Farming")]
    [SerializeField] private bool isWaterCan;
    [SerializeField] private PlantData plantData;
    [SerializeField] private FertilizerData fertilizerData;

    public int UniqueID
    {
        get
        {
            if (uid == -1) Debug.LogError($"Item {Name} has unitilized unique ID, please fix this issue before using this item");
            return uid;
        }
    }
    public bool IsWaterCan => isWaterCan;
    public bool IsMintable => isMintable;
    public bool IsPlantable => plantData != null;
    public bool IsFertilizer => fertilizerData != null;
    public string ItemName => Name;
    public string Description => description;
    public Sprite Icon => icon;
    public AttributesDictionary Attributes => attributes;
    public ItemAction OnHold => onHold;
    public ItemAction OnAction => onAction;
    public ItemAction OnItemUsed => onItemUsed;
    public bool Stackable => stackable;
    public int MaxStack => maxStack;
    public bool Unique => unique;
    public string MintFunction => mintFunction;
    public bool ForceMintAfterAdd => forceMintAfterAdd;
    public bool Marketable => marketable;
    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
    public PlantData PlantData => plantData;
    public FertilizerData FertilizerData => fertilizerData;

    private void GenerateUID()
    {
#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(Item)}");
        int maxUID = 0;

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);

            if (asset is Item item && item.uid > maxUID)
            {
                maxUID = item.uid;
            }
        }

        uid = maxUID + 1;
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}