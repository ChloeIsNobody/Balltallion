using System;
using System.Collections;
using Balltallion.DebugTools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace Balltallion.Input
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInputPrefab;
        [SerializeField] private InputActionAsset playerActionAsset;
        
        private PlayerInput playerInput1;
        private PlayerInput playerInput2;

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            ++InputUser.listenForUnpairedDeviceActivity;
            InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
        }

        private void OnDisable()
        {
            --InputUser.listenForUnpairedDeviceActivity;
            InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        }

        private void Update()
        {
            
        }

        private PlayerInput JoinPlayer(string controlScheme = null, InputDevice pairWithDevice = null)
        {
            return PlayerInput.Instantiate(playerInputPrefab.gameObject, -1, controlScheme, -1, pairWithDevice);
        }
        
        /*
         * Taken from PlayerInputManager code
         */
        private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
        {
            if (!(control is ButtonControl)) return;
            if (!IsDeviceUsableWithPlayerActions(control.device)) return;
            Debug.Log($"Unpaired device used: {control.device.displayName}");
            InputControlScheme? controlScheme =
                InputControlScheme.FindControlSchemeForDevice(control.device, playerActionAsset.controlSchemes);
            Debug.Log($"Found control scheme: {controlScheme}");
            //JoinPlayer(pairWithDevice: control.device);
        }
    
        /*
         * Taken from PlayerInputManager code
         */
        private bool IsDeviceUsableWithPlayerActions(InputDevice device)
        {
            Debug.Assert(device != null);

            if (playerInputPrefab == null)
                return true;

            var playerInput = playerInputPrefab.GetComponentInChildren<PlayerInput>();
            if (playerInput == null)
                return true;

            var actions = playerInput.actions;
            if (actions == null)
                return true;

            // If the asset has control schemes, see if there's one that works with the device plus
            // whatever unpaired devices we have left.
            if (actions.controlSchemes.Count > 0)
            {
                using (var unpairedDevices = InputUser.GetUnpairedInputDevices())
                {
                    if (InputControlScheme.FindControlSchemeForDevices(unpairedDevices, actions.controlSchemes,
                            mustIncludeDevice: device) == null)
                        return false;
                }
                return true;
            }

            // Otherwise just check whether any of the maps has bindings usable with the device.
            foreach (var actionMap in actions.actionMaps)
                if (actionMap.IsUsableWithDevice(device))
                    return true;

            return false;
        }
    }
}