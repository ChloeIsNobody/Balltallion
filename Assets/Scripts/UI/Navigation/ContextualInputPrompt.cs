using Balltallion.Input;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Navigation
{
    public class ContextualInputPrompt : MonoBehaviour
    {
        [SerializeField] private string actionPath;
        [SerializeField] private string displayName;
        [SerializeField] private PromptModes mode;
        [SerializeField, ShowIf("IsManualMode")] private LocalPlayerSO manualSetPlayer;
        
        [Header("References")]
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image icon;

        private LocalPlayerSO targetPlayer;

        private enum PromptModes
        {
            Manual,
            Automatic
        }

        private void OnValidate()
        {
            if (label) label.text = displayName;
        }

        private void Awake()
        {
            if (mode == PromptModes.Manual) SetTargetPlayer(manualSetPlayer);
        }

        private void OnEnable()
        {
            if (mode == PromptModes.Automatic) NavigationManager.Instance.OnPlayerTurnStarted += SetTargetPlayer;
        }

        private void OnDisable()
        {
            if (mode == PromptModes.Automatic) NavigationManager.Instance.OnPlayerTurnStarted -= SetTargetPlayer;
        }

        public void SetTargetPlayer(LocalPlayerSO newTargetPlayer)
        {
            if (targetPlayer) targetPlayer.OnConnected -= OnTargetPlayerConnected;
            targetPlayer = newTargetPlayer;
            if (targetPlayer.IsConnected()) UpdatePromptForPlayer(targetPlayer);
            targetPlayer.OnConnected += OnTargetPlayerConnected;
        }

        private void OnTargetPlayerConnected()
        {
            UpdatePromptForPlayer(targetPlayer);
        }
        
        private void UpdatePromptForPlayer(LocalPlayerSO player)
        {
            targetPlayer = player;
            if (label) label.text = displayName;
            if (icon) icon.sprite = player.GetInputRelay().GetIconForAction(actionPath);
        }

        public void SetActionPath(string newActionPath) => actionPath = newActionPath;
        
        private bool IsManualMode() => mode == PromptModes.Manual;
    }
}