using System;
using Balltallion.Simulation.Teams;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class BallInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        
        private BallDataSO ballData;

        public void LoadData(BallDataSO ballData)
        {
            this.ballData = ballData;
            icon.sprite = ballData?.ballSprite;
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
            Debug.Log($"Drafting {ballData.displayName}...");
        }
    }
}