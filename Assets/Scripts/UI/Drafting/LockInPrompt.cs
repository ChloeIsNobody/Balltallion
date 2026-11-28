using Balltallion.Players;
using Balltallion.UI.Components;
using Balltallion.UI.Components.Navigation;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class LockInPrompt : MonoBehaviour
    {
        [Header("Style")]
        [SerializeField] private int backgroundPaletteIdx = 0;
        [SerializeField] private int progressBarPaletteIdx = 0;

        [Header("References")]
        [SerializeField] private ContextualInputPrompt inputPrompt;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image progressBarImage;
        
        private LocalPlayerSO targetPlayer;
        
        private void OnEnable()
        {
            NavigationManager.Instance.OnPlayerTurnStarted += SetTargetPlayer;
        }

        private void OnDisable()
        {
            NavigationManager.Instance.OnPlayerTurnStarted -= SetTargetPlayer;
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
            backgroundImage.color = player.GetColorFromPalette(backgroundPaletteIdx);
            progressBarImage.color = player.GetColorFromPalette(progressBarPaletteIdx);
        }
    }
}