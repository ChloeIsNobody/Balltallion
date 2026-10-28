using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Input
{
    [CreateAssetMenu(menuName = "Balltallion/Input/LocalPlayer", fileName = "LocalPlayer")]
    public class LocalPlayerSO : ScriptableObject
    {
        public event Action OnConnected;
        public event Action OnDisconnected;
        
        [SerializeField] private string displayName;
        [SerializeField] private int playerNumber;
        [SerializeField] private PlayerInputRelaySO inputRelay;
        
        [Header("Debugging")]
        [SerializeField] private bool logToConsole;

        private bool isConnected;
        
        public void Connect(PlayerInput newPlayerInput)
        {
            if (isConnected || !newPlayerInput) return;
            if (logToConsole) Debug.Log($"{displayName} connected!");
            
            inputRelay.Connect(newPlayerInput);
            inputRelay.displayName = displayName;
            inputRelay.OnReadyTriggered += ReadyPressed;
            inputRelay.OnLeaveTriggered += LeavePressed;
            
            isConnected = true;
            OnConnected?.Invoke();
        }

        public void Disconnect()
        {
            if (!isConnected) return;
            if (logToConsole) Debug.Log($"{displayName} disconnected!");
            
            inputRelay.Disconnect();
            inputRelay.OnReadyTriggered -= ReadyPressed;
            inputRelay.OnLeaveTriggered -= LeavePressed;
            
            isConnected = false;
            OnDisconnected?.Invoke();
        }

        public void ReadyPressed()
        {
            
        }

        public void LeavePressed()
        {
            Disconnect();
        }
        
        public PlayerInputRelaySO GetInputRelay() => inputRelay;
        public string GetDisplayName() => displayName;
        public bool IsConnected() => isConnected;
    }
}