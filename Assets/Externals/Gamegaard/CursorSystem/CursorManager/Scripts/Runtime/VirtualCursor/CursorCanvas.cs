using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public class CursorCanvas : MonoBehaviour
    {
        [SerializeField] private VirtualCursor cursor;

        public PlayerCursorIdentifier Identifier { get; private set; }
        public VirtualCursor Cursor => cursor;

        public void SetIdentifier(PlayerCursorIdentifier identifierPrefab)
        {
            if(Identifier != null)
            {
                Destroy(Identifier.gameObject);
            }

            Identifier = Instantiate(identifierPrefab, cursor.transform);
        }
    }
}