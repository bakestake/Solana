using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Gamegaard.CursorSystem
{
    public class PlayerInputInitializer : MonoBehaviour
    {
        private PlayerInput playerInput;

        public PlayerInput PlayerInput => playerInput;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.camera = Camera.main;
            if (playerInput.playerIndex == 0)
            {
                playerInput.uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
            }
        }
    }
}