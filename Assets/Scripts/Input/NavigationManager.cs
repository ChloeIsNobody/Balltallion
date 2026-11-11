using System;
using System.Collections.Generic;
using Balltallion.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Balltallion.Input
{
    public class NavigationManager : MonoBehaviour
    {
        public event Action<LocalPlayerSO> OnPlayerTurnStarted; 
        
        public static NavigationManager Instance { get; private set; }

        [SerializeField] private InputManager inputManager;
        [SerializeField] private InputSystemUIInputModule inputModule;
        
        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private LocalPlayerSO firstPlayer;
        [SerializeField] private NavigationPointer navigationPointer;

        [SerializeField] private List<CanvasGroup> player1CanvasGroups;
        [SerializeField] private List<CanvasGroup> player2CanvasGroups;

        [SerializeField] private Selectable player1DefaultSelection;
        [SerializeField] private Selectable player2DefaultSelection;
        
        private LocalPlayerSO currentPlayer;
        private bool inputManagerSetupComplete;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                Debug.LogError("There can't be multiple instances of NavigationManager (singleton)!", this);
            }

            inputModule.enabled = false;
            inputManager.OnSetupCompleted += () => SetPlayerTurn(firstPlayer);
        }
        
        public void SetFirstPlayer(LocalPlayerSO player) => firstPlayer = player;
        
        public void EndPlayerTurn()
        {
            SetPlayerTurn(currentPlayer == localPlayers[0] ? localPlayers[1] : localPlayers[0]);
        }

        public void SetPlayerTurn(LocalPlayerSO player)
        {
            currentPlayer = player;
            inputModule.enabled = currentPlayer;
            
            Selectable defaultSelection = player == localPlayers[0] ? player1DefaultSelection : player2DefaultSelection;
            defaultSelection.Select();
            SetActionsAsset(player.GetInputRelay().GetActionAsset());
            
            SetCanvasGroupInteractable(player1CanvasGroups, player == localPlayers[0]);
            SetCanvasGroupInteractable(player2CanvasGroups, player == localPlayers[1]);
            
            navigationPointer.SetColor(player.GetColorFromPalette(0));
            OnPlayerTurnStarted?.Invoke(player);
        }
        
        private void SetActionsAsset(InputActionAsset newActionAsset)
        {
            inputModule.UnassignActions();
            inputModule.actionsAsset = newActionAsset;
            inputModule.point = InputActionReference.Create(newActionAsset.FindAction("Navigation/Point"));
            inputModule.leftClick = InputActionReference.Create(newActionAsset.FindAction("Navigation/Click"));
            inputModule.middleClick = InputActionReference.Create(newActionAsset.FindAction("Navigation/MiddleClick"));
            inputModule.rightClick = InputActionReference.Create(newActionAsset.FindAction("Navigation/RightClick"));
            inputModule.scrollWheel = InputActionReference.Create(newActionAsset.FindAction("Navigation/ScrollWheel"));
            inputModule.move = InputActionReference.Create(newActionAsset.FindAction("Navigation/Navigate"));
            inputModule.submit = InputActionReference.Create(newActionAsset.FindAction("Navigation/Select"));
            inputModule.cancel = InputActionReference.Create(newActionAsset.FindAction("Navigation/Cancel"));
            inputModule.trackedDevicePosition = null;
            inputModule.trackedDeviceOrientation = null;
        }

        private void SetCanvasGroupInteractable(List<CanvasGroup> canvasGroups, bool interactable)
        {
            foreach (CanvasGroup canvasGroup in canvasGroups) canvasGroup.interactable = interactable;
        }
        
        public LocalPlayerSO GetCurrentPlayer() => currentPlayer;
    }
}