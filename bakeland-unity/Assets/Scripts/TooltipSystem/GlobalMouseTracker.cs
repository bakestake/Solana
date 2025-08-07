using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GlobalMouseTracker : MonoBehaviour
{
    public static GlobalMouseTracker Instance { get; private set; }

    private bool isHoveringTooltipTrigger;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Check if the mouse is hovering over a UI element with TooltipTrigger
        CheckHoverStatus();
    }

    private void CheckHoverStatus()
    {
        // Reset the hover state
        isHoveringTooltipTrigger = false;

        // Raycast from the mouse position to check for UI elements
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Create a list to store the results of the raycast
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // Check if any of the results have a TooltipTrigger component
        foreach (var result in results)
        {
            var tooltipTrigger = result.gameObject.GetComponent<TooltipTrigger>();
            if (tooltipTrigger != null)
            {
                isHoveringTooltipTrigger = true;
                break; // No need to check further if we've found one
            }
        }

        // Handle tooltip visibility based on hover status
        if (!isHoveringTooltipTrigger)
        {
            TooltipSystem.instance.HideTooltip();
        }
    }

    // Function to check if the mouse is over a TooltipTrigger
    public bool IsHoveringTooltipTrigger()
    {
        return isHoveringTooltipTrigger;
    }
}
