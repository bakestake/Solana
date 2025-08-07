using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanel : ItemPanel
{
    public override void OnClick(int id)
    {
        LocalGameManager.Instance.dragAndDropController.OnClick(inventory.slots[id]);
        // SoundManager.instance.PlayRandomFromList(SoundManager.instance.clickSounds);
        Show();
    }
}
