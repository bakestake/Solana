using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private void Awake()
    {
       TextMeshProUGUI textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.text = Application.version;
    }
}