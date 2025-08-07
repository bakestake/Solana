using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Bike")]
public class UseBike : ItemAction
{
    public float distortionSpeed = 2f; // Speed of distortion pulsing
    public float distortionIntensity = -20f; // Base intensity
    public float maxDistortion = -30f; // Maximum intensity

    // public GameObject testItem;

    // public override bool OnApply(Vector2 worldPoint)
    // {
    //     Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     if (Vector2.Distance(worldPoint, mousePosition) < 55)
    //     {
    //         Instantiate(testItem, mousePosition, Quaternion.identity);
    //         return true;
    //     }
    //     else return false;
    // }

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        if (PlayerController.isWeaponActive) { return; }
        PlayerController.isRiding = !PlayerController.isRiding;
        SoundManager.instance.PlayRandomFromList(SoundManager.instance.bicycleBellSounds);
        // inventory.Remove(usedItem);
        Debug.Log(usedItem.Name);
    }

}
