using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public abstract class PlayerCursorIdentifier : MonoBehaviour
    {
        public abstract void SetPlayer(CursorIdentifierData cursorIdentifier, MultiplayerCursorManager multiplayerCursorManager);
    }
}
