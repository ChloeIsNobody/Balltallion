using System;
using Balltallion.Simulation.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class ModifyStatTemporary : AbilityEffect
    {
        [InfoBox("Applies modifier on trigger activate. Removes after duration.")]
        [SerializeField] private BattleBall.StatNames stat;
        [SerializeField] private float duration;
        
        [Header("Modifier Settings")]
        [SerializeField] private string modifierLabel;
        [SerializeField] private StatModifier.OperationType operation;
        [SerializeField] private float value;


        public override void Activate()
        {
            
        }

        public override void Deactivate()
        {
            
        }
        
        public override AbilityEffect Clone()
        {
            ModifyStatTemporary clone = new ModifyStatTemporary();
            clone.stat = stat;
            clone.duration = duration;
            clone.modifierLabel = modifierLabel;
            clone.operation = operation;
            clone.value = value;
            return clone;
        }
    }
}