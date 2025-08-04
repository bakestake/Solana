using UnityEngine;
using UnityEngine.InputSystem;

namespace Gamegaard.CursorSystem
{
    [System.Serializable]
    public class CursorIdentifierData
    {
        [SerializeField] private int cursorID;
        [SerializeField] string playerName;
        [SerializeField] PlayerInput playerInput;

        public readonly static CursorIdentifierData playerOne = new CursorIdentifierData(0, "Player0", null);

        public int CursorID
        {
            get => cursorID; set
            {
                cursorID = value;
            }
        }
        public string PlayerName { get => playerName; set => playerName = value; }
        public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }


        public CursorIdentifierData()
        {

        }

        public CursorIdentifierData(int id, string playerName, PlayerInput playerInput)
        {
            CursorID = id;
            PlayerName = playerName;
            PlayerInput = playerInput;
        }
    }
}