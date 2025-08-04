using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SVImageControl : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    [SerializeField] private Image pickerImage;
    [SerializeField] private ColorPicker colorPicker;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform pickerTransform;

    private float rectSizeX;
    private float rectSizeY;

    private void Awake()
    {
        RefreshSize();
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetColor(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetColor(eventData);
    }

    public void RefreshSize()
    {
        rectSizeX = rectTransform.rect.width;
        rectSizeY = rectTransform.rect.height;
    }

    public void RefreshPickerPosition()
    {
        float x = colorPicker.CurrentSaturation * rectSizeX;
        float y = colorPicker.CurrentValue * rectSizeY;

        SetPickerPosition(new Vector2(x, y), 1 - colorPicker.CurrentValue);
        pickerImage.color = Color.HSVToRGB(0, 0, 1 - colorPicker.CurrentValue);
    }

    public void SetColor(Color color)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        SetColor(s, v);
    }

    public void SetColor(float saturation, float value)
    {
        float x = saturation * rectSizeX;
        float y = value * rectSizeY;

        SetPickerPositionAndColor(new Vector2(x, y), saturation, 1 - value);
    }

    private void SetColor(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPosition);

        localPosition.x = Mathf.Clamp(localPosition.x, 0, rectSizeX);
        localPosition.y = Mathf.Clamp(localPosition.y, 0, rectSizeY);

        float xNorm = localPosition.x / rectSizeX;
        float yNorm = localPosition.y / rectSizeY;

        SetPickerPositionAndColor(localPosition, xNorm, yNorm);
    }

    private void SetPickerPosition(Vector2 position, float y)
    {
        Vector2 pivotOffset = new Vector2(rectSizeX * rectTransform.pivot.x, rectSizeY * rectTransform.pivot.y);
        pickerTransform.localPosition = position - pivotOffset;
        pickerImage.color = Color.HSVToRGB(0, 0, 1 - y);
    }

    private void SetPickerPositionAndColor(Vector2 position, float x, float y)
    {
        SetPickerPosition(position, y);
        colorPicker.SetSV(x, y);
    }
}