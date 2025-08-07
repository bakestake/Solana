using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarController : MonoBehaviour
{
    [SerializeField] private int toolbarSize = 8;
    int selectedTool;

    public Action<int> onChange;

    public Item GetItem
    {
        get
        {
            return LocalGameManager.Instance.inventoryContainer.slots[selectedTool].item;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float delta = Input.mouseScrollDelta.y;
        if (delta != 0)
        {
            if (delta < 0)
            {
                selectedTool += 1;
                selectedTool = (selectedTool >= toolbarSize ? 0 : selectedTool);
            }
            else
            {
                selectedTool -= 1;
                selectedTool = (selectedTool < 0 ? toolbarSize - 1 : selectedTool);
            }

            onChange?.Invoke(selectedTool);
            SoundManager.instance.PlayRandomFromList(SoundManager.instance.clickSounds);
        }
    }

    internal void Set(int id)
    {
        selectedTool = id;
        onChange?.Invoke(selectedTool);
    }
}
