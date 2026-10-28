using Balltallion.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.DevicePairing
{
    public class PlayerPanelUI : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO trackedPlayer;
        
        [Header("References")]
        [SerializeField] private TextMeshProUGUI controlsText;
        [SerializeField] private Image panelImage;
        [SerializeField] private Image deviceIcon;
        
        [SerializeField] private GameObject buttonObjects;
        [SerializeField] private Image readyIcon;
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private Image leaveIcon;
        [SerializeField] private GameObject joinPrompt;

        [Header("Style")]
        [SerializeField] private Sprite activatedSprite;
        [SerializeField] private Color activatedColor;
        [SerializeField] private Color deactivatedColor;
        [SerializeField] private Color readyColor;

        private void Start()
        {
            SetActive(false);
        }

        private void OnEnable()
        {
            trackedPlayer.OnConnected += OnPlayerConnected;
            trackedPlayer.OnDisconnected += OnPlayerDisconnected;
            trackedPlayer.OnReadied += OnPlayerReadied;
            trackedPlayer.OnUnreadied += OnPlayerUnreadied;
        }

        private void OnDisable()
        {
            trackedPlayer.OnConnected -= OnPlayerConnected;
            trackedPlayer.OnDisconnected -= OnPlayerDisconnected;
            trackedPlayer.OnReadied -= OnPlayerReadied;
            trackedPlayer.OnUnreadied -= OnPlayerUnreadied;
        }

        private void OnPlayerConnected()
        {
            SetActive(true);
            PlayerInputRelaySO inputRelay = trackedPlayer.GetInputRelay();
            deviceIcon.sprite = inputRelay.GetDeviceIcon();
            controlsText.text = $"Controls: {inputRelay.GetControlScheme()}";
            readyIcon.sprite = inputRelay.GetIconForAction("Ready");
            leaveIcon.sprite = inputRelay.GetIconForAction("Leave");
        }
        
        private void OnPlayerDisconnected() => SetActive(false);

        private void OnPlayerReadied()
        {
            readyIcon.color = readyColor;
            readyText.color = readyColor;
        }

        private void OnPlayerUnreadied()
        {
            readyIcon.color = Color.white;
            readyText.color = Color.white;
        }
        
        
        private void SetActive(bool active)
        {
            panelImage.color = active ? activatedColor : deactivatedColor;
            deviceIcon.enabled = active;
            controlsText.gameObject.SetActive(active);
            buttonObjects.SetActive(active);
            joinPrompt.SetActive(!active);
        }
    }
}