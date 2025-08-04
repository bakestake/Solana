namespace Gamegaard.CustomValues.Database
{
    public abstract class ListScriptableDatabase<TValue> : ScriptableDatabaseBase<TValue>
    {
        public virtual bool AddData(TValue data)
        {
            if (data == null || Datas.Contains(data)) return false;
            Datas.Add(data);
            return true;
        }

        public virtual bool RemoveData(TValue data)
        {
            return Datas.Remove(data);
        }

        public virtual void Clear()
        {
            Datas.Clear();
        }
    }
}