using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Input
{
    [CreateAssetMenu(menuName = "Balltallion/Input/PlayerInputRelay", fileName = "PlayerInputRelay")]
    public class PlayerInputRelaySO : ScriptableObject
    {
        public event Action OnDeviceLost;
        public event Action OnDeviceRegained;
        public event Action OnReadyTriggered;
        public event Action OnLeaveTriggered;

        [NonSerialized] public string displayName;
        [SerializeField] private DeviceListSO deviceList;
        
        [Header("Debugging")]
        [SerializeField] private bool logToConsole;
        
        private PlayerInput playerInput;
        private DeviceDataSO deviceData;

        private bool isConnected;
        private bool isDeviceLost;

        public void Connect(PlayerInput newPlayerInput)
        {
            if (playerInput) Debug.LogError($"{name} is already connected to a player input! Disconnect it first before connecting a new one!", this);
            
            newPlayerInput.onActionTriggered += OnActionTriggered;
            newPlayerInput.onDeviceLost += OnDeviceLost_Callback;
            newPlayerInput.onDeviceRegained += OnDeviceRegained_Callback;
            playerInput = newPlayerInput;
            
            InputDevice device = GetDevice();
            deviceData = deviceList.GetDeviceData(device);
            
            isConnected = true;
            isDeviceLost = false;
        }

        public void Disconnect()
        {
            if (playerInput)
            {
                playerInput.onActionTriggered -= OnActionTriggered;
                playerInput.onDeviceLost -= OnDeviceLost_Callback;
                playerInput.onDeviceRegained -= OnDeviceRegained_Callback;
                Destroy(playerInput.gameObject);
            }
            playerInput = null;
            deviceData = null;
            
            isConnected = false;
        }

        private void OnDeviceLost_Callback(PlayerInput context)
        {
            if (logToConsole) Debug.Log($"{displayName} lost their device!");
            isDeviceLost = true;
            OnDeviceLost?.Invoke();
        }
        
        private void OnDeviceRegained_Callback(PlayerInput context)
        {
            if (logToConsole) Debug.Log($"{displayName} regained their device!");
            isDeviceLost = false;
            OnDeviceRegained?.Invoke();
        }
        
        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                if (logToConsole) Debug.Log($"{displayName} performed {context.action.name} with {context.control.displayName}!");
                if (context.action.name == "Ready") OnReadyTriggered?.Invoke();
                else if (context.action.name == "Leave") OnLeaveTriggered?.Invoke();
            }
        }

        public bool PollActionPerformedThisFrame(string actionName)
        {
            return IsConnected() && playerInput.actions[actionName].WasPerformedThisFrame();
        }

        public void SwitchActionMap(ActionMaps actionMap)
        {
            playerInput?.SwitchCurrentActionMap(actionMap.ToString());
            if (playerInput && logToConsole) Debug.Log($"{displayName} switched action map: {actionMap}!");
        }
        
        public string GetControlScheme() => playerInput?.currentControlScheme ?? "None";

        public Sprite GetIconForAction(string action)
        {
            if (!IsConnected()) return null;
            string controlName = GetControlForAction(action);
            return deviceData.GetIconForControl(controlName);
        }
        
        public string GetControlForAction(string action) => playerInput.actions.FindAction(action).controls[0].displayName;

        public Sprite GetDeviceIcon() => deviceData.icon;
        public InputDevice GetDevice() => playerInput.devices[0];
        public DeviceDataSO GetDeviceData() => deviceData;
        public InputActionAsset GetActionAsset() => playerInput.actions;
        public bool IsConnected() => isConnected;
        public bool IsDeviceLost() => isDeviceLost;
    }

    public enum ActionMaps
    {
        Pairing,
        VirtualCursor,
        Battle
    }
}