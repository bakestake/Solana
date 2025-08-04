namespace Gamegaard
{
    public class VfxSettingsToggle : SettingsToggle
    {
        public override bool SettingsValue { get => persistentSettings.vfxState.Value; protected set => persistentSettings.vfxState.AutoSaveableValue = value; }

        public override void OnValueChange(bool isEnabled)
        {
        }
    }
}