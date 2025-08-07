using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToolbarPanel : ItemPanel
{
    [SerializeField] ToolbarController toolbarController;

    private void Start()
    {
        Init();
        toolbarController.onChange += Highlight;
        Highlight(0);

        Show();
    }

    private void LateUpdate()
    {
        Show();
    }

    private void OnEnable()
    {
        toolbarController.onChange += Highlight;
        Show();
    }

    private void OnDisable()
    {
        toolbarController.onChange -= Highlight;
        Show();
    }

    public override void OnClick(int id)
    {
        toolbarController.Set(id);
        Highlight(id);
    }

    int currentSelectedTool;

    public void Highlight(int id)
    {
        buttons[currentSelectedTool].Highlight(false);
        currentSelectedTool = id;
        buttons[currentSelectedTool].Highlight(true);
        Show();
    }

    public GameObject SelectedToolButton()
    {
        return buttons[currentSelectedTool].gameObject;
    }
}
