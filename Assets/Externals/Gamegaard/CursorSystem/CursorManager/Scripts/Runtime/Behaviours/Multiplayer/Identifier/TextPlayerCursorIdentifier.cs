using TMPro;
using UnityEngine;

namespace Gamegaard.CursorSystem
{
    public class TextPlayerCursorIdentifier : PlayerCursorIdentifier
    {
        [SerializeField] private TextMeshProUGUI textIdentifier;

        public CursorIdentifierData CursorIdentifier { get; private set; }
        public MultiplayerCursorManager MultiplayerCursorManager { get; private set; }

        private void OnDestroy()
        {
            RemoveEvents();
        }

        public override void SetPlayer(CursorIdentifierData cursorIdentifier, MultiplayerCursorManager multiplayerCursorManager)
        {
            name = $"PlayerIdentifier [{cursorIdentifier.CursorID}]";
            CursorIdentifier = cursorIdentifier;
            MultiplayerCursorManager = multiplayerCursorManager;

            RemoveEvents();
            multiplayerCursorManager.OnPlayerJoinedEvent += UpdateValue;
            multiplayerCursorManager.OnPlayerLeftEvent += UpdateValue;

            UpdateValue(cursorIdentifier, multiplayerCursorManager);
        }

        private void RemoveEvents()
        {
            if (MultiplayerCursorManager != null)
            {
                MultiplayerCursorManager.OnPlayerJoinedEvent -= UpdateValue;
                MultiplayerCursorManager.OnPlayerLeftEvent -= UpdateValue;
            }
        }

        private void UpdateValue(CursorIdentifierData cursorIdentifier, MultiplayerCursorManager multiplayerCursorManager)
        {
            textIdentifier.enabled = multiplayerCursorManager.PlayerCount > 1;
            if (textIdentifier.enabled)
            {
                textIdentifier.text = $"P{CursorIdentifier.CursorID + 1}";
            }
        }
    }
}