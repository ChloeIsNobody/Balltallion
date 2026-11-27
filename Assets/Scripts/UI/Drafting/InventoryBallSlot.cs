using System;
using System.Runtime.CompilerServices;
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

        private int inventoryIdx;
        private LocalPlayerSO owningPlayer;
        private BallDataSO ballData;

        public void BindToPlayer(LocalPlayerSO player, int inventoryIdx)
        {
            owningPlayer = player;
            this.inventoryIdx = inventoryIdx;
            ballData = owningPlayer.inventory.GetBallAtIdx(inventoryIdx);
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
            BallLoadout ballLoadout = owningPlayer.teamLoadout.GetFirstOpenLoadout();
            ballLoadout.SetBall(ballData);
        }
    }
}