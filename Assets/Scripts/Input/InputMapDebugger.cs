using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace Balltallion.Input
{
    public class InputMapDebugger : MonoBehaviour
    {
        [SerializeField] private DeviceListSO deviceList;
        [SerializeField] private Image deviceIcon;
        [SerializeField] private Image controlIcon;
        [SerializeField] private TextMeshProUGUI deviceName;
        [SerializeField] private TextMeshProUGUI controlName;
        
        private IDisposable _subscription;

        private void OnEnable()
        {
            _subscription = InputSystem.onAnyButtonPress.Call(OnAnyInputPressed);
        }

        private void OnDisable()
        {
            _subscription.Dispose();
        }

        private void OnAnyInputPressed(InputControl control)
        {
            DeviceDataSO deviceData = deviceList.GetDeviceData(control.device);
            if (!deviceData) return;
            
            deviceName.text = $"Device: {deviceData.displayName}";
            deviceIcon.sprite = deviceData.icon;
            controlName.text = $"Control: {control.displayName}";
            controlIcon.sprite = deviceData.GetIconForControl(control.displayName);
        }
    }
}