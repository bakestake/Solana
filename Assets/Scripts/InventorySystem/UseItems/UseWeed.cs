using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Weed")]
public class UseWeed : ItemAction
{
    [SerializeField] private Buff drunkBuff;

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        LocalGameManager.Instance.EnableWeedHighEffect(drunkBuff.defaultDuration);
        SoundManager.Instance.PlaySfx(SoundManager.Instance.weedUsed);
        BuffManager.instance.AddBuff(drunkBuff);
        inventory.Remove(usedItem);
    }
}