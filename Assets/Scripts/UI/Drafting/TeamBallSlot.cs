using System.Collections.Generic;
using Balltallion.Players;
using Balltallion.Simulation.Teams;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class TeamBallSlot : MonoBehaviour
    {
        [SerializeField] private LocalPlayerSO player;
        [SerializeField] private int ballSlotIdx;
        
        [Header("References")]
        [SerializeField] private Image ballImage;
        [SerializeField] private CanvasGroup equipablesGroup;
        [SerializeField] private List<TeamEquipableSlot> equipableSlots;

        private BallLoadout trackedBallLoadout;
        
        private void Start()
        {
            BindToPlayer(player, ballSlotIdx);
        }

        private void BindToPlayer(LocalPlayerSO player, int ballSlotIdx)
        {
            this.player = player;
            this.ballSlotIdx = ballSlotIdx;
            
            if (trackedBallLoadout != null) trackedBallLoadout.OnLoadoutUpdated -= UpdateUI;
            trackedBallLoadout = player.teamLoadout.GetBallLoadout(ballSlotIdx);
            trackedBallLoadout.OnLoadoutUpdated += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            BallDataSO ballData = trackedBallLoadout?.GetBallData();
            ballImage.sprite = ballData?.ballSprite;
            ballImage.enabled = ballData;
            CanvasGroupUtilities.SetCanvasGroupEnabled(equipablesGroup, ballData != null);
        }
    }
}