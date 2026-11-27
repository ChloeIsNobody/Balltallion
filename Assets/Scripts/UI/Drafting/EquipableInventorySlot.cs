using Balltallion.Simulation.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class EquipableInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        
        private EquipableDataSO equipableData;

        public void LoadData(EquipableDataSO equipableData)
        {
            this.equipableData = equipableData;
            icon.sprite = equipableData?.icon;
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
            Debug.Log($"Equipping {equipableData.displayName}...");
        }
    }
}