namespace Gamegaard.UI.PopupText
{
    public interface IPopupTextEffect
    {
        void Initialize(PopupTextBase popupText);
        /// <summary>
        /// Apply the effect using the current evaluated time from the initialized popupText.
        /// </summary>
        /// <param name="popupText">The affected text</param>
        void Evaluate(PopupTextBase popupText);
    }
}