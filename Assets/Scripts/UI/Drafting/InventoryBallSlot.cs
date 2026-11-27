using System;
using Balltallion.Players;
using Balltallion.Simulation.Teams;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class InventoryBallSlot : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        
        private LocalPlayerSO owningPlayer;
        private BallDataSO ballData;

        public void LoadData(BallDataSO ballData, LocalPlayerSO player)
        {
            this.ballData = ballData;
            owningPlayer = player;
            backgroundImage.color = owningPlayer.GetColorFromPalette(2);
            icon.sprite = ballData?.ballSprite;
            icon.enabled = ballData != null;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            if (ballData == null) return;
            Debug.Log($"Drafting {ballData.displayName}...");
        }
    }
}