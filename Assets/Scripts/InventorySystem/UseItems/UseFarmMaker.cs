using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/FarmMaker")]
public class UseFarmMaker : ItemAction
{
    [SerializeField] private AudioClip[] sounds;

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        inventory.Remove(usedItem);
        LocalGameManager localGameManager = LocalGameManager.Instance;
        SoundManager.Instance.PlayRandomFromList(sounds);

        localGameManager.PlayerController.InteractionAnimator.PlayPopup();
        localGameManager.AddFarmLand();
    }

    public override void OnItemSelected(Item item)
    {
        base.OnItemSelected(item);
        LocalGameManager localGameManager = LocalGameManager.Instance;
        localGameManager.AddFarmLandsHighlight();
    }

    public override void OnItemDeselected(Item item)
    {
        base.OnItemDeselected(item);
        LocalGameManager localGameManager = LocalGameManager.Instance;
        localGameManager.RemoveFarmLandsHighlight();
    }
}

