using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragAndDropController : MonoBehaviour
{
    [SerializeField] ItemSlot itemSlot;
    [SerializeField] ItemSlot lastItemSlot;
    public GameObject itemIcon;
    RectTransform iconTransform;
    Image itemIconImage;

    // Start is called before the first frame update
    void Start()
    {
        itemSlot = new ItemSlot();
        iconTransform = itemIcon.GetComponent<RectTransform>();
        itemIconImage = itemIcon.GetComponent<Image>();
    }

    private void Update()
    {
        if (itemIcon.activeInHierarchy)
        {
            iconTransform.position = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject() == false)
                {

                    // Debug.Log("tesdasd");
                }
            }
        }
    }

    public void PutItemBack()
    {
        if (itemSlot.item != null)
        {
            if (lastItemSlot.item == null)
            {
                lastItemSlot.Set(itemSlot.item, itemSlot.count);
            }
            else
            {
                LocalGameManager.Instance.inventoryContainer.Add(itemSlot.item, itemSlot.count);
            }
            itemSlot.Clear();
            UpdateIcon();
        }
    }

    internal void OnClick(ItemSlot itemSlot)
    {
        lastItemSlot = itemSlot;
        if (this.itemSlot.item == null)
        {
            this.itemSlot.Copy(itemSlot);
            itemSlot.Clear();
            SoundManager.Instance.PlaySfx(SoundManager.Instance.itemPickup);

        }
        else
        {
            Item item = itemSlot.item;
            int count = itemSlot.count;

            itemSlot.Copy(this.itemSlot);
            this.itemSlot.Set(item, count);
            SoundManager.Instance.PlaySfx(SoundManager.Instance.itemDrop);
        }
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (itemSlot.item == null)
        {
            itemIcon.SetActive(false);
        }
        else
        {
            itemIcon.SetActive(true);
            itemIconImage.sprite = itemSlot.item.Icon;
        }
    }
}
