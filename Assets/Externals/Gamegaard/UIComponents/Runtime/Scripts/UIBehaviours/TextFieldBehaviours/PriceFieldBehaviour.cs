public class PriceFieldBehaviour : TextFieldBehaviour
{
    protected override void ValidateValue(string text)
    {
        float value = float.Parse(text);
        targetComponent.SetTextWithoutNotify(value.ToString("0.00"));
    }
}