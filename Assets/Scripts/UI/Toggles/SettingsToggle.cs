using Bakeland;

namespace Gamegaard
{
    public abstract class SettingsToggle : ToggleBehaviour
    {
        protected PersistentSettings persistentSettings;
        public abstract bool SettingsValue { get; protected set; }

        protected virtual void Start()
        {
            persistentSettings = PersistentSettings.Instance;
            targetComponent.SetIsOnWithoutNotify(SettingsValue);
        }
    }
}