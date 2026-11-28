using System;
using System.Runtime.CompilerServices;
using Balltallion.Players;
using Balltallion.Simulation.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class InventoryBallSlot : MonoBehaviour
    {
        [Header("Style")]
        [SerializeField] private int normalPaletteIdx = 0;
        [SerializeField] private int equippedPaletteIdx = 0;
        [SerializeField, Range(1.0f, 2.0f)] private float normalImageScale = 1.0f;
        [SerializeField, Range(1.0f, 2.0f)] private float equippedImageScale = 1.0f;
        
        [Header("References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private GameObject equippedVisual;
        [SerializeField] private TextMeshProUGUI draftIndexLabel;

        private int inventoryIdx;
        private int draftIdx;
        private LocalPlayerSO owningPlayer;
        private PlayerTeamLoadout trackedTeam;
        private BallDataSO ballData; 

        private bool isEquipped;

        private void OnValidate()
        {
            icon.transform.localScale = normalImageScale * Vector3.one;
        }

        public void BindToPlayer(LocalPlayerSO player, int inventoryIdx)
        {
            owningPlayer = player;
            this.inventoryIdx = inventoryIdx;
            
            trackedTeam = owningPlayer.teamLoadout;
            trackedTeam.OnLoadoutUpdated += UpdateState;
            
            backgroundImage.color = owningPlayer.GetColorFromPalette(2);
            ballData = owningPlayer.inventory.GetBallAtIdx(inventoryIdx);
            
            LoadBallData(ballData);
            SetEquippedFlag(false);
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
            
            if (!isEquipped)
            {
                draftIdx = trackedTeam.GetCurrentLoadoutIdx();
                trackedTeam.DraftBall(ballData, inventoryIdx);
            }
            else
            {
                trackedTeam.RemoveBall(inventoryIdx);
            }
        }

        private void LoadBallData(BallDataSO ballData)
        {
            icon.sprite = ballData?.ballSprite;
            icon.enabled = ballData != null;
        }

        private void UpdateState()
        {
            SetEquippedFlag(trackedTeam.IsBallEquipped(inventoryIdx));
        }

        private void SetEquippedFlag(bool value)
        {
            if (isEquipped == value) return;
            isEquipped = value;
            equippedVisual.SetActive(value);
            draftIndexLabel.text = $"{draftIdx+1}";
            
            float scale = value ? equippedImageScale : normalImageScale;
            icon.transform.localScale = scale * Vector3.one;
            
            int paletteIdx = value ? equippedPaletteIdx : normalPaletteIdx;
            backgroundImage.color = owningPlayer.GetColorFromPalette(paletteIdx);
        } 
    }
}