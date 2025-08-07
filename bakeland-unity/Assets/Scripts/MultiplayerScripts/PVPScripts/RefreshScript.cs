using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshScript : MonoBehaviour
{
    private Button RefreshButton;

    private void Start()
    {
        if (RefreshButton == null)
        {
            RefreshButton = GetComponent<Button>();
        }
        RefreshButton.onClick.AddListener(Refresh);
    }

    private void Refresh()
    {
        StartCoroutine(RefreshList());
    }

    IEnumerator RefreshList()
    {
        RefreshButton.interactable = false;
        ServerStartMenu.Instance.RefreshSessionListUI();
        yield return new WaitForSeconds(5.0f);
        RefreshButton.interactable = true;
    }
}
