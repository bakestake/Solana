using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Chainsaw")]
public class UseChainsaw : ItemAction
{
    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        GameObject weapon = GameObject.FindWithTag("Player").transform.Find("Weapon").gameObject;

        if (weapon != null)
        {
            if (PlayerController.isRiding) { return; }
            weapon.SetActive(!weapon.activeInHierarchy);

            if (PlayerController.isWeaponActive)
            {
                PlayerController.isWeaponActive = false;
                SoundManager.instance.PlaySfx(SoundManager.instance.chainsawEnd);
            }
            else
            {
                PlayerController.isWeaponActive = true;
                SoundManager.instance.PlaySfx(SoundManager.instance.chainsawStart);
            }
            Debug.Log("used chainsaw");
        }
    }

}
