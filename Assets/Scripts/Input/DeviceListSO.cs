using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace Balltallion.Input
{
    [CreateAssetMenu(menuName = "Balltallion/Input/DeviceList", fileName = "DeviceList")]
    public class DeviceListSO : ScriptableObject
    {
        [SerializeField] private DeviceDataSO keyboard;
        [SerializeField] private DeviceDataSO xboxController;
        [SerializeField] private DeviceDataSO playstationController;

        public DeviceDataSO GetDeviceData(InputDevice inputDevice)
        {
            if (inputDevice is Keyboard) return keyboard;
            if (inputDevice is XInputController) return xboxController;
            if (inputDevice is DualShockGamepad) return playstationController;
            return null;
        }
    }
}