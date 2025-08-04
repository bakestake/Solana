using Gamegaard.SavingSystem;

namespace Everseeker
{
    public class PlayerPrefValue<T> : SaveableValueGeneric<T>
    {
        public PlayerPrefValue(string saveKey, T defaultValue = default) : base(saveKey, defaultValue)
        {
        }

        public override void Save()
        {
            PrefsUtils.Save(saveKey, value);
        }

        public override void Load(T defaultValue = default)
        {
            value = PrefsUtils.Load(saveKey, defaultValue);
        }
    }
}