using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Balltallion.Input
{
    public class PlayerPanelUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI controlsText;
        [SerializeField] private Image panelImage;
        [SerializeField] private Image iconImage;

        [Header("Style")]
        [SerializeField] private Sprite activatedSprite;
        [SerializeField] private Color activatedColor;
        [SerializeField] private Color deactivatedColor;
        
        private PlayerInput trackedPlayerInput;

        public void Start()
        {
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            panelImage.color = active ? activatedColor : deactivatedColor;
            iconImage.sprite = active ? activatedSprite : null;
            iconImage.enabled = active;
            controlsText.enabled = active;
        }

        public void SetPlayerInput(PlayerInput playerInput)
        {
            trackedPlayerInput = playerInput;
            string readyControlName = playerInput.actions.FindAction("Ready").controls[0].displayName;
            string disconnectControlName = playerInput.actions.FindAction("Disconnect").controls[0].displayName;
            
            controlsText.text = $"Controls: {playerInput.currentControlScheme}";
            controlsText.text += $"\nReady: {readyControlName}";
            controlsText.text += $"\nDisconnect: {disconnectControlName}";
        }
    }
}