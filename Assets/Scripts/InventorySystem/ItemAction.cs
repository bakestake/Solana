using UnityEngine;

public class ItemAction : ScriptableObject
{
    public virtual bool OnApply(Vector2 worldPoint)
    {
        // ???
        // Debug.LogWarning("OnApply is not implemented!");
        return true;
    }

    // public virtual bool OnApplyToTilemap(Vector3Int gridPosition, TilemapReadController tilemapReadController)
    // {
    //     Debug.LogWarning("OnApplyToTilemap is not implemented!");
    //     return true;
    // }

    public virtual void OnItemSelected(Item item) { }
    public virtual void OnItemDeselected(Item item) { }
    public virtual void OnItemUsed(Item usedItem, ItemContainer inventory)
    {

    }
}
