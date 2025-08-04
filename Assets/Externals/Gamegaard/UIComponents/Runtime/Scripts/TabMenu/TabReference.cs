using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TabReference
{
    [SerializeField] private string title;
    [SerializeField] private Button button;
    [SerializeField] private GameObject content;

    private Color selectedColor = new Color(0.75f, 0.75f, 0.75f);
    public string Title => title;
    public Button Button => button;
    public GameObject Content => content;

    public TabReference() { }

    public TabReference(Button button, GameObject content, string title = "")
    {
        SetElements(button, content, title);
    }

    public void SetElements(Button button, GameObject content, string title = "")
    {
        this.title = title;
        this.button = button;
        this.content = content;
    }

    public void SetEnabled(bool isActive)
    {
        content.SetActive(isActive);
        button.targetGraphic.color = isActive ? selectedColor : Color.gray;
    }
}