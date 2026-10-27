using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Input
{
    [CreateAssetMenu(menuName = "Balltallion/Input/LocalPlayer", fileName = "LocalPlayer")]
    public class LocalPlayerSO : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private int playerNumber;

        [SerializeField] private DeviceListSO deviceList;
        
        private PlayerInput playerInput;
        private DeviceDataSO deviceData;

        private bool isConnected;

        public void Connect(PlayerInput newPlayerInput)
        {
            if (playerInput != null) Debug.LogError($"{name} is already connected to a player input! Disconnect it first before connecting a new one!", this);
            
            Debug.Log($"{displayName} connected!");
            
            newPlayerInput.onActionTriggered += OnActionTriggered;
            newPlayerInput.onDeviceLost += OnDeviceLost;
            newPlayerInput.onDeviceRegained += OnDeviceRegained;
            playerInput = newPlayerInput;
            deviceData = deviceList.GetDeviceData(newPlayerInput.devices[0]);
            
            isConnected = true;
        }

        public void Disconnect()
        {
            if (playerInput)
            {
                Debug.Log($"{displayName} disconnected!");
                playerInput.onActionTriggered -= OnActionTriggered;
                playerInput.onDeviceLost -= OnDeviceLost;
                playerInput.onDeviceRegained -= OnDeviceRegained;
            }
            playerInput = null;
            deviceData = null;
            
            isConnected = false;
        }

        private void OnActionTriggered(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Debug.Log($"{displayName} performed {context.action.name}!");
            }
        }

        private void OnDeviceLost(PlayerInput context)
        {
            Debug.Log($"{displayName} lost their device!");
        }
        
        private void OnDeviceRegained(PlayerInput context)
        {
            Debug.Log($"{displayName} regained their device!");
        }
        
        public string GetDisplayName() => displayName;
        public int GetPlayerNumber() => playerNumber;
        public string GetControlScheme() => playerInput?.currentControlScheme ?? "None";
        public string GetControlForAction(string action) => playerInput.actions.FindAction(action).controls[0].name;
        public Sprite GetIconForAction(string action) => deviceData.GetIconForAction(action);
        public Sprite GetDeviceIcon() => deviceData.icon;
        public bool IsConnected() => isConnected;
    }
}