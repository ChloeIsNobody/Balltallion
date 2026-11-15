using Balltallion.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Components
{
    public class DevicePairingPanel : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO trackedPlayer;
        private PlayerInputRelaySO inputRelay;
        
        [Header("References")]
        [SerializeField] private TextMeshProUGUI nameText;
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
        [SerializeField] private int activatedColorPaletteIdx;
        [SerializeField] private int deactivatedColorPaletteIdx;
        [SerializeField] private Sprite activatedSprite;
        [SerializeField] private Color readyColor;

        private void OnValidate()
        {
            if (trackedPlayer)
            {
                nameText.text = trackedPlayer.GetDisplayName();
                panelImage.color = GetActivatedColor();
            }
        }

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
            nameText.text = trackedPlayer.GetDisplayName();
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
            panelImage.color = active ? GetActivatedColor() : GetDeactivatedColor();
            deviceIcon.enabled = active;
            controlsText.gameObject.SetActive(active);
            buttonObjects.SetActive(active);
            joinPrompt.SetActive(!active);
        }

        private Color GetActivatedColor() => trackedPlayer.GetColorFromPalette(activatedColorPaletteIdx);
        private Color GetDeactivatedColor() => trackedPlayer.GetColorFromPalette(deactivatedColorPaletteIdx);
    }
}