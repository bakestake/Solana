namespace Gamegaard
{
    public abstract class PlayerFillBarUI : FillBarUI
    {
        protected void SetValue(IBarUser attributeValue)
        {
            base.SetValue(attributeValue.Percentage);
        }
    }
}