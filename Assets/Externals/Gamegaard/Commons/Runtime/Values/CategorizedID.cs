namespace Gamegaard.CustomValues
{
    [System.Serializable]
    public struct CategorizedID
    {
        public string category;
        public int id;

        public CategorizedID(string category, int id)
        {
            this.category = category;
            this.id = id;
        }

        public bool Compare(string category, int id)
        {
            return this.category == category && this.id == id;
        }

        public bool Compare(CategorizedID categorizedID)
        {
            return Compare(categorizedID.category, categorizedID.id);
        }

        public static bool operator ==(CategorizedID a, CategorizedID b)
        {
            return a.category == b.category && a.id == b.id;
        }

        public static bool operator !=(CategorizedID a, CategorizedID b)
        {
            return a.category != b.category || a.id != b.id;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}