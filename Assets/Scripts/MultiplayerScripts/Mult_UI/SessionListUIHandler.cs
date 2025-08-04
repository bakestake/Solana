using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System;

public class SessionListUIHandler : MonoBehaviour
{
    //Private Members
    [SerializeField]
    private TextMeshProUGUI statusText;
    [SerializeField]
    private GameObject sessionListItemprefab;
    [SerializeField]
    private VerticalLayoutGroup verticalLayout;

    //Public methods
    public void ClearList()
    {
        //Delete all hildren of the vertical layout group
        foreach (Transform child in verticalLayout.transform)
        {
            Destroy(child.gameObject);
        }

        //Hide the status text
        statusText.gameObject.SetActive(false);

    }

    public void AddToList(SessionInfo sessionInfo)
    {
        //Add a new item to the list
        SessionInfoScriptUIItem sessionInfoScript = Instantiate(sessionListItemprefab, verticalLayout.transform).GetComponent<SessionInfoScriptUIItem>();

        sessionInfoScript.SetInformation(sessionInfo);

        sessionInfoScript.OnJoinSession += AddedSessionInfoListUIItem_OnJoinSession;
    }

    //Private Methods
    private void AddedSessionInfoListUIItem_OnJoinSession(SessionInfo info)
    {
        
    }

    private void OnNoSessionFound()
    {
        statusText.text = "No Game session Found";
        statusText.gameObject.SetActive(true);
    }
}
