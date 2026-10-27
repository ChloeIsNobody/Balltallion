using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.Input
{
    public class PlayerPanelUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI controlsText;
        [SerializeField] private Image panelImage;
        [SerializeField] private Image deviceIcon;
        
        [SerializeField] private GameObject buttonObjects;
        [SerializeField] private Image readyIcon;
        [SerializeField] private Image leaveIcon;

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
            deviceIcon.enabled = active;
            controlsText.enabled = active;
            buttonObjects.SetActive(active);
        }

        public void LoadPlayerData(LocalPlayerSO playerData)
        {
            trackedPlayerData = playerData;
            deviceIcon.sprite = playerData.GetDeviceIcon();
            
            controlsText.text = $"Controls: {playerData.GetControlScheme()}";
            readyIcon.sprite = playerData.GetIconForAction("Ready");
            leaveIcon.sprite = playerData.GetIconForAction("Leave");
        }
    }
}