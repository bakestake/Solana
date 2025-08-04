using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    public Button ExitButton;

    public Animator ButtonAnimator;

    private bool SettingsPressed = false;

    public void ToggleExitButton()
    {
        if (!SettingsPressed)
        {
            SettingsPressed = true;
            ButtonAnimator.SetTrigger("in");
        }
        else
        {
            SettingsPressed = false;
            ButtonAnimator.SetTrigger("out");
        }
    }


}
