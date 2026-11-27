using Balltallion.Players;
using Balltallion.Simulation.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class EquipableInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        
        private LocalPlayerSO owningPlayer;
        private EquipableDataSO equipableData;

        public void LoadData(EquipableDataSO equipableData, LocalPlayerSO player)
        {
            this.equipableData = equipableData;
            owningPlayer = player;
            backgroundImage.color = owningPlayer.GetColorFromPalette(2);
            icon.sprite = equipableData?.icon;
            icon.enabled = equipableData != null;
        }

        private void OnEnable()
        {
            if (equipableData == null) return;
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            Debug.Log($"Equipping {equipableData.displayName}...");
        }
    }
}