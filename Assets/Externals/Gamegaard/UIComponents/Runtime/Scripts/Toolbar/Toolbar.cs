using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour, IToolbar
{
    [SerializeField] private Button toolbarButtonPrefab;
    [SerializeField] private Transform contentArea;

    private RectTransform rectTransform;
    private List<Button> allOptions = new List<Button>();
    private bool hasOptions;

    public void OpenToolbar(IEnumerable<ToolbarButtonValue> options, Vector2 position, Vector2 pivot, bool selectFirstElement)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        rectTransform.anchorMax = pivot;
        rectTransform.anchorMin = pivot;
        rectTransform.pivot = pivot;
        rectTransform.position = position;
        OpenToolbar(options, selectFirstElement);
    }

    public void OpenToolbar(IEnumerable<ToolbarButtonValue> options, bool selectFirstElement)
    {
        ClearOptions();
        foreach (ToolbarButtonValue option in options)
        {
            AddOption(option);
        }

        hasOptions = allOptions.Count > 0;
        if (hasOptions && selectFirstElement)
        {
            allOptions[0].Select();
        }
        gameObject.SetActive(hasOptions);
    }

    public void CloseToolbar()
    {
        gameObject.SetActive(false);
    }

    public void AddOption(ToolbarButtonValue option)
    {
        Button newButton = Instantiate(toolbarButtonPrefab, contentArea);
        newButton.interactable = option.IsEnabled;
        newButton.onClick.AddListener(option.OnClick);
        allOptions.Add(newButton);

        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.SetText(option.Text);
    }

    public void ClearOptions()
    {
        foreach (Button option in allOptions)
        {
            Destroy(option.gameObject);
        }
        allOptions.Clear();
        hasOptions = false;
    }
}