using System.Collections.Generic;
using UnityEngine;

public class ItemPanel : MonoBehaviour
{
    public ItemContainer inventory;
    public List<InventoryButton> buttons;

    protected virtual void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        SetIndex();
        Show();
    }

    protected virtual void OnEnable()
    {
        Show();
    }

    protected virtual void Update()
    {
        if (inventory.isDirty)
        {
            Show();
            inventory.isDirty = false;
        }
    }

    protected virtual void SetIndex()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetIndex(i);
        }
    }

    public virtual void Show()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < inventory.slots.Count)
            {
                buttons[i].TrySet(inventory.slots[i]);
            }
            else
            {
                buttons[i].Clear();
            }
        }
    }

    public virtual void OnClick(int id)
    {
        SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.clickSounds);
    }
}