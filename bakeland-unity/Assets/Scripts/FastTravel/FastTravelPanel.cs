using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastTravelPanel : MonoBehaviour
{
    public string currentStation;
    [SerializeField] Button[] buttons;

    public void SetButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {

            if (currentStation == buttons[i].GetComponent<FastTravel>().stationName)
            {
                buttons[i].interactable = false;
                Debug.Log("This is false");
            }
            else
            {
                buttons[i].interactable = true;
                Debug.Log("This is true");
            }

        }
    }
}
