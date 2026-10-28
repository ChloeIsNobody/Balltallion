using System;
using System.Collections;
using System.Collections.Generic;
using Balltallion.DebugTools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace Balltallion.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInputPrefab;
        [SerializeField] private InputActionAsset playerActionAsset;

        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private GameObject splitKeyboardPrompt;
        
        private const int playerLimit = 2;
        private bool canActivateSplitKeyboard;

        private void Awake()
        {
            if (localPlayers.Count != playerLimit) Debug.LogError("Local player count doesn't match player limit");
            
            // Ensure local players start disconnected
            foreach(LocalPlayerSO localPlayer in localPlayers) localPlayer.Disconnect();
            splitKeyboardPrompt.SetActive(false);
        }

        private void OnDestroy()
        {
            foreach(LocalPlayerSO localPlayer in localPlayers) localPlayer.Disconnect();
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
            canActivateSplitKeyboard = GetNumConnectedPlayers() == 0;
            splitKeyboardPrompt.gameObject.SetActive(canActivateSplitKeyboard);
        }
        
        /*
         * Taken from PlayerInputManager code
         */
        private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
        {
            if (!(control is ButtonControl)) return;
            if (!IsDeviceUsableWithPlayerActions(control.device)) return;

            if (canActivateSplitKeyboard && control.device is Keyboard keyboard && control.displayName == "Space")
            {
                ActivateSplitKeyboard(keyboard);
                return;
            }
            
            InputControlScheme? potentialControlScheme =
                InputControlScheme.FindControlSchemeForDevice(control.device, playerActionAsset.controlSchemes);

            if (potentialControlScheme.HasValue)
            {
                InputControlScheme controlScheme = potentialControlScheme.Value;
                JoinPlayer(controlScheme.name, control.device);
            }
        }

        private void ActivateSplitKeyboard(Keyboard keyboardDevice)
        {
            JoinPlayer("KeyboardLeft", keyboardDevice);
            JoinPlayer("KeyboardRight", keyboardDevice);
        }
        
        private void JoinPlayer(string controlScheme = null, InputDevice pairWithDevice = null)
        {
            if (GetNumConnectedPlayers() >= playerLimit) return;
            
            PlayerInput newPlayerInput =  PlayerInput.Instantiate(playerInputPrefab.gameObject, -1,
                controlScheme, -1, pairWithDevice);
            newPlayerInput.transform.SetParent(transform);

            int openSlotIdx = FindNextOpenPlayerSlot();
            if (openSlotIdx < 0) return;
            
            LocalPlayerSO localPlayer = localPlayers[openSlotIdx];
            localPlayer.Connect(newPlayerInput);
            newPlayerInput.name = $"{localPlayer.GetDisplayName()} PlayerInput";
            StartCoroutine(ActivateAfterDelay(newPlayerInput));
        }

        /*
         * It's necessary to deactivate player input on the frame the player connects.
         * This prevents scenarios where the player joins by pressing the leave button, and then the input is processed,
         * causing the player to leave in the same frame.
         */
        private IEnumerator ActivateAfterDelay(PlayerInput playerInput)
        {
            playerInput.DeactivateInput();
            yield return null;
            playerInput.ActivateInput();
        }

        private int FindNextOpenPlayerSlot()
        {
            for (int i = 0; i < localPlayers.Count; i++)
            {
                LocalPlayerSO localPlayer = localPlayers[i];
                if (!localPlayer.IsConnected()) return i;
            }
            return -1;
        }

        private int GetNumConnectedPlayers()
        {
            int connectedCount = 0;
            for (int i = 0; i < localPlayers.Count; i++)
            {
                LocalPlayerSO localPlayer = localPlayers[i];
                connectedCount += localPlayer.IsConnected() ? 1 : 0;
            }
            return connectedCount;
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