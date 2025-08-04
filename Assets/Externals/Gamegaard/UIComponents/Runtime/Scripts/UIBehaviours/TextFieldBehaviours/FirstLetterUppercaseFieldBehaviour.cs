public class FirstLetterUppercaseFieldBehaviour : TextFieldBehaviour
{
    protected override void ValidateValue(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        if (text.Length == 1)
        {
            targetComponent.SetTextWithoutNotify(text.ToUpper());
            return;
        }

        string formattedText = char.ToUpper(text[0]) + text.Substring(1).ToLower();
        targetComponent.SetTextWithoutNotify(formattedText);
    }
}