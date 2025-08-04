using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
    public GameObject tooltipPrefab;         // Reference to the tooltip prefab
    private GameObject tooltipInstance;      // Instance of the tooltip
    private TextMeshProUGUI headerText;      // Reference to the header text component
    private TextMeshProUGUI descriptionText; // Reference to the description text component

    // Static reference to track the current tooltip instance
    public static TooltipSystem instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // Create the tooltip instance and hide it initially
        tooltipInstance = Instantiate(tooltipPrefab, transform);
        tooltipInstance.SetActive(false);

        // Get the references to the text components
        headerText = tooltipInstance.transform.Find("HeaderText").GetComponent<TextMeshProUGUI>();
        descriptionText = tooltipInstance.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Position the tooltip near the mouse cursor
        if (tooltipInstance.activeSelf)
        {
            PositionTooltip();
        }
    }

    public void ShowTooltip(string header, string description, Color color)
    {
        headerText.text = header;
        headerText.color = color;
        descriptionText.text = description;
        tooltipInstance.SetActive(true);
        PositionTooltip();
    }

    public void HideTooltip()
    {
        tooltipInstance.SetActive(false);
    }

    public void HideTooltipIfActive(GameObject uiElement)
    {
        if (tooltipInstance.activeSelf && uiElement == null)
        {
            HideTooltip();
        }
    }

    private void PositionTooltip()
    {
        Vector2 mousePos = Input.mousePosition;

        // Set the position of the tooltip so that its top-left corner is at the cursor
        tooltipInstance.transform.position = mousePos;
    }
}
