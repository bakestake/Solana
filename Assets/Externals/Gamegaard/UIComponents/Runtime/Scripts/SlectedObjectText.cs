using Gamegaard;
using TMPro;
using UnityEngine;

public class SlectedObjectText : SingleComponentBehaviour<TextMeshProUGUI>
{
    public void SetSelectedObject(Transform selectedObject)
    {
        targetComponent.text = selectedObject.name;
    }

    public void OnDiselect()
    {
        targetComponent.text = "__________";
    }
}