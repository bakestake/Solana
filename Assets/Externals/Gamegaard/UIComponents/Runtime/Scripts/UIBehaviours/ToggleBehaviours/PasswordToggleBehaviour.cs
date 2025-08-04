using TMPro;
using UnityEngine;

public class PasswordToggleBehaviour : ToggleBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public override void OnValueChange(bool isEnabled)
    {
        inputField.contentType = isEnabled ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        inputField.ForceLabelUpdate();
    }
}