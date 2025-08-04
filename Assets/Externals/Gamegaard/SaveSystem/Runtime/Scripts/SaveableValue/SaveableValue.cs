namespace Everseeker
{
    public abstract class SaveableValue
    {
        protected readonly string saveKey;
        public abstract void Save();

        public SaveableValue(string saveKey)
        {
            this.saveKey = saveKey;
        }
    }
}