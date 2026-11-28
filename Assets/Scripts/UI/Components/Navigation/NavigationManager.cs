using System;
using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Players;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Balltallion.UI.Components.Navigation
{
    public class NavigationManager : MonoBehaviour
    {
        public event Action<LocalPlayerSO> OnPlayerTurnStarted;
        public event Action<Selectable> OnObjectSelected;
        
        public static NavigationManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private NavigationModes navigationMode = NavigationModes.TurnBased;

        [SerializeField, ShowIf("IsSingleMode")] private Selectable defaultSelectable;
        
        [SerializeField, ShowIf("IsTurnBasedMode")] private LocalPlayerSO firstPlayer;
        [SerializeField, ShowIf("IsTurnBasedMode")] private playerNavData navData1;
        [SerializeField, ShowIf("IsTurnBasedMode")] private playerNavData navData2;

        [Serializable]
        private class playerNavData
        {
            public LocalPlayerSO player;
            public List<CanvasGroup> canvasGroups;
            public Selectable defaultSelection;
        }
        
        [Header("References")]
        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private NavigationPointer navigationPointer;

        private enum NavigationModes
        {
            Single,
            TurnBased,
        }
        
        private InputManager inputManager;
        private LocalPlayerSO currentPlayer;

        private Selectable selectedObject;
        private Vector2 lastSelectedPosition;

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
            
            inputManager = InputManager.Instance;
        }

        private void Start()
        {
            if (navigationMode == NavigationModes.Single)
            {
                inputModule.enabled = true;
                defaultSelectable.Select();
            }
            else if (navigationMode == NavigationModes.TurnBased)
            {
                inputModule.enabled = inputManager.IsSetupCompleted();
                if (inputManager.IsSetupCompleted()) SetPlayerTurn(firstPlayer);
                else inputManager.OnSetupCompleted += InputManager_OnSetupCompleted;
            }
        }

        private void InputManager_OnSetupCompleted()
        {
            inputModule.enabled = true;
            SetPlayerTurn(firstPlayer);
        }

        private void Update()
        {
            if (!inputModule.enabled) return;
            UpdateSelectedObject();
            ChangeSelectionIfLost();
        }
        
        private void ChangeSelectionIfLost()
        {
            if (!selectedObject) return;
            if (!selectedObject.IsInteractable() || !selectedObject.isActiveAndEnabled)
            {
                GameObject nearestSelectable = FindNearestSelectable()?.gameObject;
                EventSystem.current.SetSelectedGameObject(nearestSelectable);
            } 
        }

        private Selectable FindNearestSelectable()
        {
            List<Selectable> selectables = new List<Selectable>(FindObjectsByType<Selectable>());
            Selectable nearestSelectable = null;
            float nearestDistance = Mathf.Infinity;
            foreach (Selectable selectable in selectables)
            {
                if (!selectable.enabled || !selectable.IsInteractable()) continue;
                float distance = Vector2.Distance(lastSelectedPosition, selectable.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSelectable = selectable;
                }
            }

            return nearestSelectable;
        }

        private void UpdateSelectedObject()
        {
            GameObject newSelectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject && newSelectedObject == selectedObject.gameObject) return;
            selectedObject = newSelectedObject?.GetComponent<Selectable>();
            OnObjectSelected?.Invoke(selectedObject);
            
            if (!selectedObject) return;
            lastSelectedPosition = selectedObject.transform.position;
        }

        public void SetFirstPlayer(LocalPlayerSO player) => firstPlayer = player;
        
        public void EndPlayerTurn()
        {
            SetPlayerTurn(currentPlayer == navData1.player ? navData2.player : navData1.player);
        }

        public void SetPlayerTurn(LocalPlayerSO player)
        {
            currentPlayer = player;
            playerNavData navData = GetNavDataForPlayer(player);
            
            inputModule.enabled = currentPlayer;
            SetActionsAsset(player.GetInputRelay().GetActionAsset());
            SetCanvasGroupInteractable(navData1.canvasGroups, player == navData1.player);
            SetCanvasGroupInteractable(navData2.canvasGroups, player == navData2.player);
            navigationPointer.SetColor(player.GetColorFromPalette(0));
            
            OnPlayerTurnStarted?.Invoke(player);
            
            Selectable defaultSelection = navData.defaultSelection;
            defaultSelection?.Select();
        }

        public void SetDefaultSelectionForPlayer(LocalPlayerSO player, Selectable newDefaultSelection)
        {
            playerNavData navData = GetNavDataForPlayer(player);
            navData.defaultSelection = newDefaultSelection;
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

        private playerNavData GetNavDataForPlayer(LocalPlayerSO player)
        {
            if (navData1.player == player) return navData1;
            if (navData2.player == player) return navData2;
            return null;
        }

        private void SetCanvasGroupInteractable(List<CanvasGroup> canvasGroups, bool interactable)
        {
            foreach (CanvasGroup canvasGroup in canvasGroups) canvasGroup.interactable = interactable;
        }

        public Selectable GetCurrentSelectable() => selectedObject;
        public LocalPlayerSO GetCurrentPlayer() => currentPlayer;
        private bool IsSingleMode() => navigationMode == NavigationModes.Single;
        private bool IsTurnBasedMode() => navigationMode == NavigationModes.TurnBased;
    }
}