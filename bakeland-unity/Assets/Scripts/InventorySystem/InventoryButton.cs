using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InventoryButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite defaultButtonSprite;
    [SerializeField] Image icon;
    [SerializeField] public TextMeshProUGUI count;
    [SerializeField] Image highlight;
    public Item itemSO;

    int myIndex;

    public void SetIndex(int index)
    {
        myIndex = index;
    }

    public void Set(ItemSlot slot)
    {
        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.icon;
        // icon.GetComponent<Image>().SetNativeSize();
        itemSO = slot.item;

        if (slot.item.stackable)
        {
            count.gameObject.SetActive(true);
            count.text = slot.count.ToString();
        }
        else
        {
            count.gameObject.SetActive(false);
        }
    }

    public void Clean()
    {
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        count.gameObject.SetActive(false);
        itemSO = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemPanel itemPanel = transform.parent.GetComponent<ItemPanel>();
        itemPanel.OnClick(myIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ZoomInItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ZoomOutItem();
    }

    public void Highlight(bool b)
    {
        if (b)
        {
            buttonImage.sprite = highlight.sprite;
        }
        else
        {
            buttonImage.sprite = defaultButtonSprite;
        }
        // highlight.gameObject.SetActive(b);
    }

    private void ZoomInItem()
    {
        icon.transform.DOKill();
        icon.transform.DOScale(1.1f, 0.2f);

        count.transform.DOKill();
        count.transform.DOScale(1.1f, 0.2f);
    }

    private void ZoomOutItem()
    {
        icon.transform.DOKill();
        icon.transform.DOScale(1f, 0.2f);

        count.transform.DOKill();
        count.transform.DOScale(1f, 0.2f);
    }
}