using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Speed Elixir")]
public class UseSpeedElixir : ItemAction
{
    public Buff speedBuff;
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        SoundManager.Instance.PlaySfx(SoundManager.Instance.potion);
        BuffManager.instance.AddBuff(speedBuff);
        inventory.Remove(usedItem);
    }
}