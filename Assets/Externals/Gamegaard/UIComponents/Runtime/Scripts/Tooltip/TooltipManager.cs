using System;
using Gamegaard.Singleton;

namespace Gamegaard
{
    public class TooltipManager : MonoBehaviourSingleton<TooltipManager>
    {
        public event Action<string, string> OnCallToolTip;
        public event Action OnEndToolTip;

        public void CallTooltip(string title, string description)
        {
            OnCallToolTip?.Invoke(title, description);
        }

        public void EndTooltip()
        {
            OnEndToolTip?.Invoke();
        }
    }
}