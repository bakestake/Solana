using DG.Tweening;
using Gamegaard.Commons;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : ButtonBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected bool isHand;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite defaultButtonSprite;
    [SerializeField] private Image icon;
    [SerializeField] private Image highlight;

    public TextMeshProUGUI count;
    private int myIndex;
    protected bool isHighlighted;

    protected ItemSlot slot;
    public Item ItemSO { get; private set; }

    public void SetIndex(int index)
    {
        myIndex = index;
    }

    public void TrySet(ItemSlot slot)
    {
        if (slot.item != null && slot.item == ItemSO) TryUpdateCount(slot);

        if (this.slot == slot && slot.item == ItemSO) return;
        this.slot = slot;

        if (slot.IsEmpty)
        {
            Clear();
            return;
        }

        Set(slot);
    }

    private void Set(ItemSlot slot)
    {
        TriggerSelectionBehaviours(false);

        ItemSO = slot.item;
        icon.gameObject.SetActive(true);
        icon.sprite = slot.item.Icon;
        name = $"InventorySlot [{slot.item.ItemName}]";
        TryUpdateCount(slot);

        if (isHighlighted)
        {
            TriggerSelectionBehaviours(true);
        }
    }

    private void TryUpdateCount(ItemSlot slot)
    {
        if (slot.item == null) return;

        if (slot.item.Stackable)
        {
            count.gameObject.SetActive(true);
            count.text = slot.count.ToString();
        }
        else
        {
            count.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        TriggerSelectionBehaviours(false);
        name = "InventorySlot [Empty]";
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        count.gameObject.SetActive(false);
        ItemSO = null;
    }

    public override void OnClick()
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

    public void Highlight(bool isHighlighted)
    {
        if (this.isHighlighted == isHighlighted) return;
        this.isHighlighted = isHighlighted;
        buttonImage.sprite = isHighlighted ? highlight.sprite : defaultButtonSprite;
        TriggerSelectionBehaviours(isHighlighted);
    }

    protected void TriggerSelectionBehaviours(bool isActive)
    {
        if (isHand && ItemSO != null && ItemSO.OnHold != null)
        {
            if (isActive)
            {
                ItemSO.OnHold.OnItemSelected(ItemSO);
            }
            else
            {
                ItemSO.OnHold.OnItemDeselected(ItemSO);
            }
        }
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