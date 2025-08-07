using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Weed")]
public class UseWeed : ItemAction
{
    public Buff drunkBuff;
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        // LocalGameManager.Instance.EnableWeedHighEffect(5);
        SoundManager.instance.PlaySfx(SoundManager.instance.weedUsed);
        BuffManager.instance.AddBuff(drunkBuff);
        inventory.Remove(usedItem);
        Debug.Log(usedItem.Name);
    }

}
