using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class TabMenu : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private float tabSpacing;

    [Header("References")]
    [SerializeField] private List<TabReference> tabs;
    [SerializeField] private Transform buttonContent;
    [SerializeField] private Transform tabContent;

    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject menuPrefab;

    [Header("Events")]
    [SerializeField] private UnityEvent<int> onTabChanged;

    private TabReference currentTab;

    private void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            TabReference tab = tabs[i];
            tab.Button.onClick.AddListener(() => SelectedTab(index));
            tab.SetEnabled(i == 0);
        }
        currentTab = tabs[0];
    }

    public void SelectedTab(int index)
    {
        TabReference tab = tabs[index];
        if (tab == currentTab) return;

        onTabChanged?.Invoke(index);
        currentTab.SetEnabled(false);
        currentTab = tab;
        currentTab.SetEnabled(true);
    }
}

public partial class TabMenu : MonoBehaviour
{
#if UNITY_EDITOR
    private LayoutGroup layoutGroup;

    public void OnValidate()
    {
        if (layoutGroup == null)
        {
            layoutGroup = GetComponentInChildren<LayoutGroup>();
        }

        if (layoutGroup is HorizontalLayoutGroup horizontalLayout)
        {
            horizontalLayout.spacing = tabSpacing;
        }
        else if (layoutGroup is VerticalLayoutGroup verticalLayout)
        {
            verticalLayout.spacing = tabSpacing;
        }
    }

    public void GenerateTabs()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            TabReference tab = tabs[i];

            GameObject buttonObj;
            Button button;
            if (tab.Button == null)
            {
                buttonObj = CreatePrefabInstance(buttonPrefab, buttonContent);
                button = buttonObj.GetComponent<Button>();
            }
            else
            {
                buttonObj = tab.Button.gameObject;
                button = tab.Button;
            }

            GameObject menuObj;
            if (tab.Content == null)
            {
                menuObj = CreatePrefabInstance(menuPrefab, tabContent);
                menuObj.SetActive(false);
            }
            else
            {
                menuObj = tab.Content;
            }

            tab.SetElements(button, menuObj, string.IsNullOrWhiteSpace(tab.Title) ? $"Tab {i}" : tab.Title);
            menuObj.name = $"Tab [{tab.Title}]";
            button.name = $"Button [{tab.Title}]";

            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = tab.Title;
            }

            tabs[i] = tab;
        }

        OnValidate();
    }

    private GameObject CreatePrefabInstance(GameObject prefab, Transform parent)
    {
        GameObject instance;

        if (PrefabUtility.IsPartOfPrefabAsset(prefab))
        {
            instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        }
        else
        {
            instance = Instantiate(prefab, parent);
        }

        return instance;
    }
#endif
}