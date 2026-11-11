using Balltallion.Input;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI
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

        private void Update()
        {
            if (mode == PromptModes.Manual && !targetPlayer)
            {
                if (manualSetPlayer.IsConnected()) UpdatePromptForPlayer(manualSetPlayer);
            }
        }

        private void OnEnable()
        {
            if (mode == PromptModes.Automatic) NavigationManager.Instance.OnPlayerTurnStarted += UpdatePromptForPlayer;
        }

        private void OnDisable()
        {
            if (mode == PromptModes.Automatic) NavigationManager.Instance.OnPlayerTurnStarted -= UpdatePromptForPlayer;
        }

        private void UpdatePromptForPlayer(LocalPlayerSO player)
        {
            targetPlayer = player;
            if (label) label.text = displayName;
            if (icon) icon.sprite = player.GetInputRelay().GetIconForAction(actionPath);
        }
        
        private bool IsManualMode() => mode == PromptModes.Manual;
    }
}