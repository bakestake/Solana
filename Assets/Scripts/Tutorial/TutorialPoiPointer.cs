using UnityEngine;

public class TutorialPoiPointer : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public Transform currentPOI;  // Reference to the current Point of Interest
    public RectTransform arrowUI;  // Reference to the arrow UI element
    public RectTransform iconUI;  // Reference to the icon UI element
    public Camera mainCamera;  // Main camera reference

    public float offScreenMargin = 30f;  // Margin to keep the arrow within screen bounds
    public float minDistanceToHideArrow = 5f;  // Minimum distance to hide the arrow
    public float iconDistanceFromArrow = 50f;  // Distance of the icon from the arrow

    private void Start()
    {
        ResetCurrentPointer();
    }

    void Update()
    {
        if (currentPOI != null)
        {
            UpdatePointer();
        }
        else
        {
            arrowUI.gameObject.SetActive(false);
            iconUI.gameObject.SetActive(false);
        }
    }

    void UpdatePointer()
    {
        Vector3 poiScreenPosition = mainCamera.WorldToScreenPoint(currentPOI.position);

        // Check if the POI is off-screen
        bool isOffScreen = poiScreenPosition.x <= 0 || poiScreenPosition.x >= Screen.width || poiScreenPosition.y <= 0 || poiScreenPosition.y >= Screen.height;

        // Calculate distance between player and POI
        float distanceToPOI = Vector3.Distance(player.position, currentPOI.position);

        // Hide arrow and icon if POI is close enough to be visible on-screen
        if (distanceToPOI < minDistanceToHideArrow && !isOffScreen)
        {
            arrowUI.gameObject.SetActive(false);
            iconUI.gameObject.SetActive(false);
            return;
        }
        else
        {
            arrowUI.gameObject.SetActive(true);
            iconUI.gameObject.SetActive(true);
        }

        if (isOffScreen)
        {
            // Calculate the direction from the player to the POI
            Vector3 direction = (currentPOI.position - player.position).normalized;

            // Project the direction to the screen edges
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Vector3 screenDirection = (poiScreenPosition - screenCenter).normalized;

            // Calculate the point where the direction vector intersects the screen bounds
            Vector3 arrowScreenPosition = CalculateEdgePosition(screenDirection);

            // Convert the screen position to UI space and set the arrow position
            arrowUI.position = arrowScreenPosition;

            // Rotate the arrow to point towards the POI
            float angle = Mathf.Atan2(screenDirection.y, screenDirection.x) * Mathf.Rad2Deg;
            arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90f);  // Adjust to align with the arrow graphic

            // Position the icon next to the arrow (without rotating it)
            Vector3 iconPosition = CalculateIconPosition(arrowScreenPosition, screenCenter);
            iconUI.position = iconPosition;
            iconUI.rotation = Quaternion.identity;  // Make sure the icon doesn't rotate
        }
        else
        {
            arrowUI.gameObject.SetActive(false);
            iconUI.gameObject.SetActive(false);
        }
    }

    Vector3 CalculateEdgePosition(Vector3 direction)
    {
        // Determine which edge of the screen the arrow should be on based on the direction vector
        float aspectRatio = Screen.width / (float)Screen.height;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Arrow is on the left or right edge
            float x = direction.x > 0 ? Screen.width - offScreenMargin : offScreenMargin;
            float y = Screen.height / 2f + (x - Screen.width / 2f) * direction.y / direction.x;
            return new Vector3(x, Mathf.Clamp(y, offScreenMargin, Screen.height - offScreenMargin), 0f);
        }
        else
        {
            // Arrow is on the top or bottom edge
            float y = direction.y > 0 ? Screen.height - offScreenMargin : offScreenMargin;
            float x = Screen.width / 2f + (y - Screen.height / 2f) * direction.x / direction.y;
            return new Vector3(Mathf.Clamp(x, offScreenMargin, Screen.width - offScreenMargin), y, 0f);
        }
    }

    Vector3 CalculateIconPosition(Vector3 arrowPosition, Vector3 screenCenter)
    {
        // Calculate the direction from the arrow to the screen center
        Vector3 directionToCenter = (screenCenter - arrowPosition).normalized;

        // Position the icon a certain distance from the arrow towards the screen center
        return arrowPosition + directionToCenter * iconDistanceFromArrow;
    }

    public void ResetCurrentPointer()
    {
        currentPOI = null;
    }
}