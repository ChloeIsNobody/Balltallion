using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Input
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputManager playerInputManager;

        private void OnEnable()
        {
            playerInputManager.onPlayerJoined += OnPlayerJoined;
            playerInputManager.onPlayerLeft += OnPlayerLeft;
        }

        private void OnDisable()
        {
            playerInputManager.onPlayerJoined -= OnPlayerJoined;
            playerInputManager.onPlayerLeft -= OnPlayerLeft;
        }

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            string deviceName = playerInput.devices[0].displayName;
            Debug.Log($"Player joined the game with {deviceName}!");
        }

        private void OnPlayerLeft(PlayerInput playerInput)
        {
            string deviceName = playerInput.devices[0].displayName;
            Debug.Log($"Player left the game with {deviceName}!");
        }
    }
}