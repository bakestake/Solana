using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Bike")]
public class UseBike : ItemAction
{
    public float distortionSpeed = 2f;
    public float distortionIntensity = -20f;
    public float maxDistortion = -30f;

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        if (PlayerController.isWeaponActive) { return; }
        PlayerController.isRiding = !PlayerController.isRiding;
        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.bicycleBellSounds);
    }
}

