using Balltallion.Simulation.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Drafting
{
    public class TeamEquipableSlot : MonoBehaviour
    {
        [SerializeField] private Image equipableIcon;

        private void Awake()
        {
            LoadData(null);
        }
        
        private void LoadData(EquipableDataSO equipableData)
        {
            equipableIcon.sprite = equipableData?.icon;
            equipableIcon.enabled = equipableData;
        }
    }
}