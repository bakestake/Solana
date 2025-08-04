using UnityEngine;
using UnityEngine.UI;

public class FastTravelPanel : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    public string CurrentStation { get; set; }

    public void SetButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            bool isInteractible = CurrentStation != buttons[i].GetComponent<FastTravel>().StationName;
            buttons[i].interactable = isInteractible;
        }
    }
}
