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
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInputPrefab;
        [SerializeField] private InputActionAsset playerActionAsset;

        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private List<PlayerPanelUI> playerPanels;
        
        private List<PlayerInput> playerInputs;
        private int numPlayersConnected = 0;
        private const int playerLimit = 2;

        private void Awake()
        {
            playerInputs = new List<PlayerInput>(playerLimit);
            for (int i = 0; i < playerLimit; ++i) playerInputs.Add(null);
            if (playerPanels.Count != playerLimit) Debug.LogError("Player limit doesn't match player panel count!");
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
        
        /*
         * Taken from PlayerInputManager code
         */
        private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
        {
            if (!(control is ButtonControl)) return;
            if (!IsDeviceUsableWithPlayerActions(control.device)) return;
            
            Debug.Log($"Unpaired device used: {control.device.displayName}");
            InputControlScheme? potentialControlScheme =
                InputControlScheme.FindControlSchemeForDevice(control.device, playerActionAsset.controlSchemes);

            if (potentialControlScheme.HasValue)
            {
                InputControlScheme controlScheme = potentialControlScheme.Value;
                Debug.Log($"Found control scheme: {controlScheme.name}");
                JoinPlayer(controlScheme.name, control.device);
            }
        }
        
        private void JoinPlayer(string controlScheme = null, InputDevice pairWithDevice = null)
        {
            if (numPlayersConnected >= playerLimit) return;
            
            PlayerInput newPlayer =  PlayerInput.Instantiate(playerInputPrefab.gameObject, -1,
                controlScheme, -1, pairWithDevice);
            newPlayer.transform.SetParent(transform);

            int openSlotIdx = numPlayersConnected;
            localPlayers[openSlotIdx].ConnectPlayerInput(newPlayer);
            playerInputs[openSlotIdx] = newPlayer;
            playerPanels[openSlotIdx].SetActive(true);
            playerPanels[openSlotIdx].SetPlayerInput(newPlayer);
            numPlayersConnected++;
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