using UnityEngine;
using UnityEngine.UI;

public class RawImageScroller : MonoBehaviour
{
    [SerializeField] private Vector2 direction = new Vector2(1,-1);
    [SerializeField] private float speed = 1;

    private RawImage rawImage;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        Rect rect = rawImage.uvRect;
        float velocity = speed * Time.unscaledDeltaTime;
        float offsetX = rect.x + direction.x * velocity;
        float offsetY = rect.y + direction.y * velocity;

        Rect newUVRect = new Rect(offsetX, offsetY, rect.width, rect.height);

        rawImage.uvRect = newUVRect;
    }
}