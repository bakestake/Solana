using UnityEngine;

namespace Gamegaard.CursorSystem
{
    [System.Serializable]
    public class PlayerCursorData
    {
        [field: SerializeField] public CursorCanvas Canvas { get; set; }
        [field: SerializeField] public VirtualCursorManager CursorManager { get; set; }
        [field: SerializeField] public VirtualCursor Cursor { get; set; }
        [field: SerializeField] public CursorIdentifierData Identifier { get; set; }

        public PlayerCursorData()
        {

        }

        public PlayerCursorData(CursorCanvas canvas, VirtualCursorManager cursorManager, VirtualCursor cursor, CursorIdentifierData identifierData)
        {
            Canvas = canvas;
            CursorManager = cursorManager;
            Cursor = cursor;
            Identifier = identifierData;
        }
    }
}