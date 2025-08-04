using UnityEngine;

public class ItemToolbarPanel : ItemPanel
{
    [SerializeField] private ToolbarController toolbarController;

    private int currentSelectedTool;

    protected override void Start()
    {
        Init();
        toolbarController.OnChange += Highlight;
        Highlight(0);

        Show();
    }

    private void LateUpdate()
    {
        Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        toolbarController.OnChange += Highlight;
        Show();
    }

    private void OnDisable()
    {
        toolbarController.OnChange -= Highlight;
        Show();
    }

    public override void OnClick(int id)
    {
        toolbarController.Set(id);
        Highlight(id);
    }

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
