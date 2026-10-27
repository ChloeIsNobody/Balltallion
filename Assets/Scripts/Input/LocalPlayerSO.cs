using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Input
{
    [CreateAssetMenu(menuName = "Balltallion/LocalPlayerSO", fileName = "LocalPlayerSO")]
    public class LocalPlayerSO : ScriptableObject
    {
        private PlayerInput playerInput;

        public void OnDisable()
        {
            DisconnectPlayerInput();
        }

        public void ConnectPlayerInput(PlayerInput newPlayerInput)
        {
            if (playerInput != null) Debug.LogError($"{name} is already connected to a player input! Disconnect it first before connecting a new one!", this);
            newPlayerInput.onActionTriggered += OnActionTriggered;
            playerInput = newPlayerInput;
        }

        public void DisconnectPlayerInput()
        {
            playerInput.onActionTriggered -= OnActionTriggered;
            playerInput = null;
        }

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Debug.Log($"{name} performed {context.action.name}!");
            }
        }
    }
}