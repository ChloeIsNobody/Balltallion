using Balltallion.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.DevicePairing
{
    public class PlayerPanelUI : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO trackedPlayer;
        private PlayerInputRelaySO inputRelay;
        
        [Header("References")]
        [SerializeField] private TextMeshProUGUI controlsText;
        [SerializeField] private Image panelImage;
        [SerializeField] private Image deviceIcon;
        
        [SerializeField] private GameObject buttonObjects;
        [SerializeField] private Image readyIcon;
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private Image leaveIcon;
        [SerializeField] private GameObject joinPrompt;
        [SerializeField] private GameObject disconnectedFlair;

        [Header("Style")]
        [SerializeField] private Sprite activatedSprite;
        [SerializeField] private Color activatedColor;
        [SerializeField] private Color deactivatedColor;
        [SerializeField] private Color readyColor;

        private void Awake()
        {
            inputRelay = trackedPlayer.GetInputRelay();
        }
        
        private void Start()
        {
            SetActive(false);
        }

        private void OnEnable()
        {
            trackedPlayer.OnConnected += OnPlayerConnected;
            trackedPlayer.OnDisconnected += OnPlayerDisconnected;
        }

        private void OnDisable()
        {
            trackedPlayer.OnConnected -= OnPlayerConnected;
            trackedPlayer.OnDisconnected -= OnPlayerDisconnected;
        }

        private void OnPlayerConnected()
        {
            SetActive(true);
            inputRelay = trackedPlayer.GetInputRelay();
            deviceIcon.sprite = inputRelay.GetDeviceIcon();
            controlsText.text = $"Controls: {inputRelay.GetDeviceData().displayName}";
            readyIcon.sprite = inputRelay.GetIconForAction("Ready");
            leaveIcon.sprite = inputRelay.GetIconForAction("Leave");
        }
        
        private void OnPlayerDisconnected() => SetActive(false);

        private void Update()
        {
            readyIcon.color = trackedPlayer.IsReady() ? readyColor : Color.white;
            readyText.color = trackedPlayer.IsReady() ? readyColor : Color.white;
            disconnectedFlair.SetActive(inputRelay.IsDeviceLost());
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