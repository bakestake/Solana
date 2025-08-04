using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Gamegaard.CursorSystem
{
    public class MultiplayerCursorManager : CursorManagerBase
    {
        [Header("References")]
        [SerializeField] private VirtualCursorManager cursorManagerPrefab;
        [SerializeField] private SpriteCursorData[] playerSkins;

        [Header("Optional References")]
        [SerializeField] private PlayerCursorIdentifier playerIdentifierPrefab;

        private readonly Dictionary<int, PlayerCursorData> playerCursorDataMap = new();

        public event Action<CursorIdentifierData, MultiplayerCursorManager> OnPlayerJoinedEvent;
        public event Action<CursorIdentifierData, MultiplayerCursorManager> OnPlayerLeftEvent;

        public int PlayerCount => playerCursorDataMap.Count;

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            if (playerInput.playerIndex == 0)
            {
                CursorIdentifierData.playerOne.PlayerInput = playerInput;
            }

            playerInput.name += $" [{playerInput.playerIndex}]";
            CursorIdentifierData playerIdentifier = new CursorIdentifierData(playerInput.playerIndex, "", playerInput);
            AddPlayerCursor(playerIdentifier);
            OnPlayerJoinedEvent?.Invoke(playerIdentifier, this);
        }

        public void OnPlayerLeft(PlayerInput playerInput)
        {
            int playerIndex = playerInput.playerIndex;
            if (!playerCursorDataMap.ContainsKey(playerIndex))
            {
                Debug.LogWarning($"Nenhum cursor encontrado para o PlayerInput com índice {playerIndex}. O jogador pode já ter sido removido.");
                return;
            }

            CursorIdentifierData playerIdentifier = new CursorIdentifierData(playerIndex, "", playerInput);
            RemovePlayerCursor(playerIdentifier);

            OnPlayerLeftEvent?.Invoke(playerIdentifier, this);
            Debug.Log($"Jogador com índice {playerIndex} deixou o jogo e seu cursor foi removido.");
        }

        public void AddPlayerCursor(CursorIdentifierData player)
        {
            if (!playerCursorDataMap.ContainsKey(player.CursorID))
            {
                VirtualCursorManager managerInstance = Instantiate(cursorManagerPrefab, transform);
                CursorCanvas canvasInstance = managerInstance.CursorCanvas;

                ConfigurePlayerInput(player, managerInstance);

                if (playerIdentifierPrefab != null)
                {
                    canvasInstance.SetIdentifier(playerIdentifierPrefab);
                    canvasInstance.Identifier.SetPlayer(player, this);
                }

                if (playerSkins.Length > player.CursorID)
                {
                    managerInstance.SetDefaultCursor(playerSkins[player.CursorID]);
                }

                managerInstance.SetVirtualCursor(canvasInstance.Cursor);
                managerInstance.VirtualCursor.SetID(player);

                playerCursorDataMap[player.CursorID] = new PlayerCursorData(canvasInstance, managerInstance, canvasInstance.Cursor, player);
            }
        }

        private void ConfigurePlayerInput(CursorIdentifierData player, VirtualCursorManager manager)
        {
            if (manager.InputHandler.InputModeData.InputMode == InputMode.LegacyInputManager) return;

            InputUser inputUser = InputUser.PerformPairingWithDevice(player.PlayerInput.devices.First(), player.PlayerInput.user);
            inputUser.AssociateActionsWithUser(player.PlayerInput.actions);

            VirtualMouseFix virtualMouse = manager.GetComponentInChildren<VirtualMouseFix>();
            if (virtualMouse != null)
            {
                InputActionAsset clonedAsset = player.PlayerInput.actions;
                string bindingGroup = inputUser.controlScheme.Value.bindingGroup;
                bool isMouse = bindingGroup == "Mouse";

                if (!isMouse && virtualMouse.StickAction.action != null)
                {
                    virtualMouse.PointerAction = default;
                    virtualMouse.StickAction = GetNewAction(virtualMouse.StickAction, clonedAsset);
                }
                else if (isMouse && virtualMouse.PointerAction.action != null)
                {
                    virtualMouse.StickAction = default;
                    virtualMouse.PointerAction = GetNewAction(virtualMouse.PointerAction, clonedAsset);
                }
                virtualMouse.LeftButtonAction = GetNewAction(virtualMouse.LeftButtonAction, clonedAsset);
                virtualMouse.MiddleButtonAction = GetNewAction(virtualMouse.MiddleButtonAction, clonedAsset);
                virtualMouse.RightButtonAction = GetNewAction(virtualMouse.RightButtonAction, clonedAsset);
                virtualMouse.ForwardButtonAction = GetNewAction(virtualMouse.ForwardButtonAction, clonedAsset);
                virtualMouse.BackButtonAction = GetNewAction(virtualMouse.BackButtonAction, clonedAsset);
                virtualMouse.ScrollWheelAction = GetNewAction(virtualMouse.ScrollWheelAction, clonedAsset);
            }

            manager.InputHandler.SetInputMode(InputMode.NewInputSystem, virtualMouse.LeftButtonAction);
        }

        public string GetActionPath(InputActionProperty inputAction)
        {
            InputAction action = inputAction.action;
            if (action != null)
            {
                return $"{action.actionMap.name}/{action.name}";
            }
            return null;
        }

        public InputActionProperty GetNewAction(InputActionProperty inputAction, InputActionAsset clonedAsset)
        {
            if (inputAction.action == null)
            {
                return inputAction;
            }
            return new InputActionProperty(clonedAsset.FindAction(GetActionPath(inputAction), true));
        }

        public void RemovePlayerCursor(CursorIdentifierData player, bool reorganize = true)
        {
            if (playerCursorDataMap.TryGetValue(player.CursorID, out var cursorData))
            {
                Destroy(cursorData.Canvas.gameObject);
                playerCursorDataMap.Remove(player.CursorID);

                if (reorganize)
                {
                    ReorganizeIdentifiers();
                }
            }
        }

        private void ReorganizeIdentifiers()
        {
            List<int> sortedPlayers = playerCursorDataMap.Keys.OrderBy(p => p).ToList();
            Dictionary<int, PlayerCursorData> originalMap = new Dictionary<int, PlayerCursorData>(playerCursorDataMap);
            playerCursorDataMap.Clear();

            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                int oldId = sortedPlayers[i];

                if (!originalMap.TryGetValue(oldId, out var cursorData))
                {
                    Debug.LogWarning($"Cursor data não encontrado para ID: {oldId}");
                    continue;
                }

                CursorIdentifierData oldIdentifier = cursorData.Identifier;
                CursorIdentifierData newIdentifier = new CursorIdentifierData(i + 1, "", oldIdentifier.PlayerInput);

                playerCursorDataMap[newIdentifier.CursorID] = cursorData;

                if (playerIdentifierPrefab != null)
                {
                    cursorData.Canvas?.Identifier?.SetPlayer(newIdentifier, this);
                }
            }
        }

        public void UpdateCursorPosition(CursorIdentifierData player, Vector2 position)
        {
            if (playerCursorDataMap.TryGetValue(player.CursorID, out PlayerCursorData cursorData))
            {
                cursorData.CursorManager.SetCursorToScreenPosition(position);
            }
        }

        public void ApplyCursorState(CursorIdentifierData player, string stateName)
        {
            if (playerCursorDataMap.TryGetValue(player.CursorID, out PlayerCursorData cursorData))
            {
                cursorData.CursorManager.SetCursorState(stateName);
            }
        }

        public override void SetCursorToScreenPosition(Vector2 position, int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData cursorData))
            {
                cursorData.CursorManager.SetCursorToScreenPosition(position);
            }
        }

        public override void HideCursor(int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData cursorData))
            {
                cursorData.CursorManager.HideCursor(cursorID);
            }
        }

        public override void ShowCursor(int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData cursorData))
            {
                cursorData.CursorManager.ShowCursor(cursorID);
            }
        }

        public override void ReleaseAll(int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData cursorData))
            {
                cursorData.CursorManager.ReleaseAll(cursorID);
            }
        }

        public override void SetCursorState(string stateName, int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData cursorData))
            {
                cursorData.CursorManager.SetCursorState(stateName, cursorID);
            }
        }

        public override void RequestCursorAsGeneric(BaseCursorData cursorData, object requester, string stateName = "Default", int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData playerCursorData))
            {
                playerCursorData.CursorManager.RequestCursorAsGeneric(cursorData, requester, stateName, cursorID);
            }
        }

        public override void ReleaseCursorAsGeneric(BaseCursorData cursorData, object requester, int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData playerCursorData))
            {
                playerCursorData.CursorManager.ReleaseCursorAsGeneric(cursorData, requester, cursorID);
            }
        }

        public override void SetDefaultCursorAsGeneric(BaseCursorData cursorData, int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData playerCursorData))
            {
                playerCursorData.CursorManager.SetDefaultCursorAsGeneric(cursorData, cursorID);
            }
        }

        public override BaseCursorData GetActiveCursorData(int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData playerCursorData))
            {
                return playerCursorData.CursorManager.GetActiveCursorData(cursorID);
            }
            return null;
        }

        public override BaseCursorData GetDefaultCursorData(int cursorID = 0)
        {
            if (playerCursorDataMap.TryGetValue(cursorID, out PlayerCursorData playerCursorData))
            {
                return playerCursorData.CursorManager.GetDefaultCursorData(cursorID);
            }
            return null;
        }
    }
}