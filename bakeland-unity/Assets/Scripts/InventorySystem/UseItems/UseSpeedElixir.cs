using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Speed Elixir")]
public class UseSpeedElixir : ItemAction
{
    public Buff speedBuff;
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        SoundManager.instance.PlaySfx(SoundManager.instance.potion);
        BuffManager.instance.AddBuff(speedBuff);
        inventory.Remove(usedItem);
        Debug.Log(usedItem.Name);
    }

}
