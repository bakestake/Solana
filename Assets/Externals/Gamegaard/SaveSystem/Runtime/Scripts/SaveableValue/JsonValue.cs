using Everseeker;
using Gamegaard.SavingSystem;

namespace Gamegaard
{
    public class JsonValue<T> : SaveableValueGeneric<T>
    {
        protected readonly string fileName;

        public JsonValue(string saveKey, T defaultValue = default, string fileName = "") : base(saveKey, defaultValue)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = SaveSystem.defaultSaveName;
            }
            this.fileName = fileName;
        }

        public override void Save()
        {
            SaveSystem.Save(saveKey, value, fileName);
        }

        public override void Load(T defaultValue = default)
        {
            value = SaveSystem.Load(saveKey, defaultValue, fileName);
        }
    }
}