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
                SoundManager.Instance.PlaySfx(SoundManager.Instance.chainsawEnd);
            }
            else
            {
                PlayerController.isWeaponActive = true;
                SoundManager.Instance.PlaySfx(SoundManager.Instance.chainsawStart);
            }
        }
    }

    //public override void OnItemSelected(Item item)
    //{
    //    base.OnItemSelected(item);
    //    GameObject weapon = GameObject.FindWithTag("Player").transform.Find("Weapon").gameObject;

    //    if (weapon != null)
    //    {
    //        if (PlayerController.isRiding) { return; }
    //        weapon.SetActive(!weapon.activeInHierarchy);

    //        PlayerController.isWeaponActive = true;
    //        SoundManager.Instance.PlaySfx(SoundManager.Instance.chainsawStart);
    //    }
    //}

    //public override void OnItemDeselected(Item item)
    //{
    //    base.OnItemDeselected(item);
    //    GameObject weapon = GameObject.FindWithTag("Player").transform.Find("Weapon").gameObject;

    //    if (weapon != null)
    //    {
    //        if (PlayerController.isRiding) { return; }
    //        weapon.SetActive(!weapon.activeInHierarchy);

    //        PlayerController.isWeaponActive = false;
    //        SoundManager.Instance.PlaySfx(SoundManager.Instance.chainsawEnd);
    //    }
    //}
}