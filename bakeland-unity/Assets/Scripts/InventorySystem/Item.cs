using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class Item : ScriptableObject
{
    public string Name;
    public string description;
    public bool stackable;
    public bool unique;
    public int maxStack = 999;
    public bool marketable;
    public int buyPrice;
    public int sellPrice;
    public Sprite icon;
    public ItemAction onAction;
    // public ToolAction onTilemapAction;
    public ItemAction onItemUsed;
    public string mintFunction;
}
