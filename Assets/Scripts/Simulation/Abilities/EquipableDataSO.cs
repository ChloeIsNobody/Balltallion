using System.Collections.Generic;
using Balltallion.Simulation.Teams;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    [CreateAssetMenu(menuName = "Balltallion/EquipableData", fileName = "New Equipable")]
    public class EquipableDataSO : ScriptableObject
    {
        [InfoBox( "This is where you can put any notes about the item that you want to display in the editor.")]
        [SerializeField, TextArea(1, 12)] private string designerNotes;
        
        [Header("Display Info")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] public string displayName = "Equipable";
        [SerializeField, TextArea(1, 5)] private string tooltipDescription = "TOOLTIP DESCRIPTION.";
        [SerializeField, ShowAssetPreview, ScriptableObjectIcon] public Sprite icon;
        
        [Header("Data")]
        [SerializeField, Expandable] private List<AbilitySO> abilities;
        
        public IReadOnlyList<AbilitySO> GetAbilities() => abilities;
    }
}