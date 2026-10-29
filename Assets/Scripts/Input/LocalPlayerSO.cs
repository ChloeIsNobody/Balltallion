using System;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Balltallion.Input
{
    [CreateAssetMenu(menuName = "Balltallion/Input/LocalPlayer", fileName = "LocalPlayer")]
    public class LocalPlayerSO : ScriptableObject
    {
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action OnReadied;
        public event Action OnUnreadied;
        
        [SerializeField] private string displayName;
        [SerializeField] private int playerNumber;
        [SerializeField] private PlayerInputRelaySO inputRelay;
        
        [Header("Debugging")]
        [SerializeField] private bool logToConsole;

        private bool isConnected;
        private bool isReady;
        
        private void OnEnable()
        {
            ResetState();
            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged += LogPlayModeState;
            #endif
        }

        private void OnDisable()
        {
            ResetState();
            #if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= LogPlayModeState;
            #endif
        }

        #if UNITY_EDITOR
        private void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                ResetState();
            }
        }
        #endif

        private void ResetState()
        {
            Disconnect();
            isReady = false;
        }

        public void Connect(PlayerInput newPlayerInput)
        {
            if (isConnected || !newPlayerInput) return;
            if (logToConsole) Debug.Log($"{displayName} connected!");
            
            inputRelay.Connect(newPlayerInput);
            inputRelay.displayName = displayName;
            inputRelay.OnReadyTriggered += ReadyPressed;
            inputRelay.OnLeaveTriggered += LeavePressed;
            
            isConnected = true;
            isReady = false;
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
            isReady = false;
            OnDisconnected?.Invoke();
        }

        public void ReadyPressed()
        {
            isReady = !isReady;
            if (isReady) OnReadied?.Invoke();
            else OnUnreadied?.Invoke();
        }
        public void SetReady(bool ready) => isReady = ready;

        public void LeavePressed()
        {
            Disconnect();
        }
        
        public PlayerInputRelaySO GetInputRelay() => inputRelay;
        public string GetDisplayName() => displayName;
        public bool IsConnected() => isConnected;
        public bool IsReady() => isReady;
    }
}