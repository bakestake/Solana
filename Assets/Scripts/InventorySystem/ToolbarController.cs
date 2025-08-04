using System;
using UnityEngine;

public class ToolbarController : MonoBehaviour
{
    [SerializeField] private int toolbarSize = 10;

    public event Action<int> OnChange;

    public Item SelectedItem => LocalGameManager.Instance.inventoryContainer.slots[SelectedTool].item;
    public int SelectedTool { get; private set; }
    public bool HasSelectedItem => SelectedItem != null;

    private void Update()
    {
        float delta = Input.mouseScrollDelta.y;
        if (delta != 0)
        {
            if (delta < 0)
            {
                SelectedTool += 1;
                SelectedTool = (SelectedTool >= toolbarSize ? 0 : SelectedTool);
            }
            else
            {
                SelectedTool -= 1;
                SelectedTool = (SelectedTool < 0 ? toolbarSize - 1 : SelectedTool);
            }

            OnChange?.Invoke(SelectedTool);
            SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.clickSounds);
        }
    }

    internal void Set(int id)
    {
        SelectedTool = id;
        OnChange?.Invoke(SelectedTool);
    }
}
