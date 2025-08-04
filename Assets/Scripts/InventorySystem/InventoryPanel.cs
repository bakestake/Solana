public class InventoryPanel : ItemPanel
{
    public override void OnClick(int id)
    {
        LocalGameManager.Instance.dragAndDropController.OnClick(inventory.slots[id]);
        Show();
    }
}
