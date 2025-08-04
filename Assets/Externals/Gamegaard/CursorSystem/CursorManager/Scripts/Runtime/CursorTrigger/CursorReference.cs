namespace Gamegaard.CursorSystem
{
    [System.Serializable]
    public struct CursorReference
    {
        public BaseCursorData cursor;
        public string stateName;

        public CursorReference(BaseCursorData cursor, string stateName)
        {
            this.cursor = cursor;
            this.stateName = stateName;
        }
    }
}