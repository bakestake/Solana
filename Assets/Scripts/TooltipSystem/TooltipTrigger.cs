using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<InventoryButton>() != null) // Check if this is an inventory item
        {
            Item item = GetComponent<InventoryButton>().ItemSO;
            if (item == null)
            {
                TooltipSystem.instance.HideTooltip();
                return;
            }
            string itemName = item.ItemName;
            string itemDescription = item.Description;
            Color nameColor = Color.white;
            if (itemName.Length > 0 || itemName != null)
            {
                if (item.Stackable)
                {
                    itemName = item.ItemName + "<color=#2c2c2c> x" + GetComponent<InventoryButton>().count.text.ToString() + "</color>";
                }

                if (item.OnAction != null || item.OnItemUsed != null)
                {
                    nameColor = new Color(239f / 255f, 200f / 255f, 85f / 255f);
                }

                TooltipSystem.instance.ShowTooltip(itemName, itemDescription, nameColor);

            }
        }
        else if (GetComponent<BuffUI>())
        {
            if (GetComponent<BuffUI>().buff == null)
            {
                TooltipSystem.instance.HideTooltip();
                return;
            }
            string buffName = GetComponent<BuffUI>().buff.buffName;
            string buffDescription = GetComponent<BuffUI>().buff.description;
            if (buffName.Length > 0 || buffName != null)
            {
                TooltipSystem.instance.ShowTooltip(buffName, buffDescription, Color.white);
            }
        }
        else
        {
            if (header.Length > 0 || description != null)
            {
                TooltipSystem.instance.ShowTooltip(header, description, Color.white);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.instance.HideTooltip();
    }
}
