using UnityEngine;

public class ToolbarMenu : MonoBehaviour
{
    [SerializeField] private ToolbarTriggerValue[] toolbarButtonValue;
    [SerializeField] private ToolbarTrigger triggerPrefab;
    [SerializeField] private Toolbar toolbarPrefab;
    [SerializeField] private Vector2 optionsPivot = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 originPivot = new Vector2(0.5f, 0.5f);

    private void Start()
    {
        foreach (ToolbarTriggerValue button in toolbarButtonValue)
        {
            Toolbar toolbar = Instantiate(toolbarPrefab, transform.root);
            ToolbarTrigger trigger = Instantiate(triggerPrefab, transform);

            if (trigger is ToolbarTriggerWithPosition triggerWithPosition)
            {
                RectTransform rect = trigger.transform as RectTransform;
                triggerWithPosition.Initialize(button.OptionName, toolbar, button.Options, optionsPivot, rect, originPivot);
            }
            else
            {
                trigger.Initialize(button.OptionName, toolbar, button.Options);
            }
            trigger.name = $"ToolbarOption [{button.OptionName}]";
        }
    }
}