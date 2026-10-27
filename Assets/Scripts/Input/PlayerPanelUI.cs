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
        
        private LocalPlayerSO trackedPlayerData;

        public void Start()
        {
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            panelImage.color = active ? activatedColor : deactivatedColor;
            iconImage.enabled = active;
            controlsText.enabled = active;
        }

        public void LoadPlayerData(LocalPlayerSO playerData)
        {
            trackedPlayerData = playerData;
            string readyControlName = playerData.GetControlForAction("Ready");
            string disconnectControlName = playerData.GetControlForAction("Disconnect");

            iconImage.sprite = playerData.GetDeviceIcon();
            
            controlsText.text = $"Controls: {playerData.GetControlScheme()}";
            controlsText.text += $"\nReady: {readyControlName}";
            controlsText.text += $"\nDisconnect: {disconnectControlName}";
        }
    }
}