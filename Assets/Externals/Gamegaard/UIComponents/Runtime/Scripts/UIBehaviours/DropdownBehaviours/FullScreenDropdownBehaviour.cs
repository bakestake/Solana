using System;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenDropdownBehaviour : DropdownBehaviour
{
    private int currentResolutionIndex;

    private void Start()
    {
        targetComponent.ClearOptions();

        List<string> options = new List<string>();
        var fullScreenModesArray = Enum.GetValues(typeof(FullScreenMode));

        System.Collections.IList list = fullScreenModesArray;
        for (int i = 0; i < list.Count; i++)
        {
            object option = list[i];
            options.Add(option.ToString());

            if (Screen.fullScreenMode == (FullScreenMode)option)
            {
                currentResolutionIndex = i;
            }
        }

        targetComponent.AddOptions(options);
        targetComponent.value = currentResolutionIndex;
        targetComponent.RefreshShownValue();
    }

    public override void OnValueChange(int optionIndex)
    {
        Screen.fullScreenMode = (FullScreenMode)optionIndex;
    }
}