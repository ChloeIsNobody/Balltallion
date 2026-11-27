using System;
using System.Collections.Generic;
using Balltallion.Input;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Balltallion.Players
{
    [CreateAssetMenu(menuName = "Balltallion/Input/LocalPlayer", fileName = "LocalPlayer")]
    public class LocalPlayerSO : ScriptableObject
    {
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action OnReadied;
        public event Action OnUnreadied;
        
        [Header("Debugging")]
        [SerializeField] private bool logToConsole;
        
        [Header("Data")]
        [SerializeField] private string displayName;
        [SerializeField] private int playerNumber;
        [SerializeField] private int inventoryBallCapacity = 8;
        [SerializeField] private int inventoryEquipableCapacity = 16;
        
        [Space]
        [InfoBox("Read Only. Do not modify this mid-game!", EInfoBoxType.Warning)]
        [SerializeField] public PlayerInventory inventory;
        [InfoBox("Read Only. Do not modify this mid-game!", EInfoBoxType.Warning)]
        [SerializeField] public PlayerTeamLoadout teamLoadout;
        
        [Header("Style")]
        [SerializeField] private Color teamColor;
        [SerializeField] private List<Color> colorPalette;
        [SerializeField] private Material backgroundMaterial;
        
        [Header("References")]
        [SerializeField] private PlayerInputRelaySO inputRelay;
        
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

            inventory = new PlayerInventory(inventoryBallCapacity, inventoryEquipableCapacity);
            teamLoadout = new PlayerTeamLoadout();
            
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

            inventory = null;
            teamLoadout = null;
            
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
        public int GetPlayerID() => playerNumber;
        public string GetDisplayName() => displayName;
        public Color GetColorFromPalette(int colorIdx) => colorPalette[colorIdx];
        public Material GetBackgroundMaterial() => backgroundMaterial;
        public bool IsConnected() => isConnected;
        public bool IsReady() => isReady;
    }
}