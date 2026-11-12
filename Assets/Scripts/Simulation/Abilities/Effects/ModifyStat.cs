using System;
using Balltallion.Simulation.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class ModifyStat : AbilityEffect
    {
        [InfoBox("Applies modifier on trigger activate. Removes on trigger deactivate.")]
        [SerializeField] private BattleBall.StatNames stat;
        
        [Header("Modifier Settings")]
        [SerializeField] private string modifierLabel;
        [SerializeField] private StatModifier.OperationType operation;
        [SerializeField] private float value;

        private StatModifier statModifier;
        
        public override void Activate()
        {
            if (!parentBall) return;
            if (statModifier == null) statModifier = new StatModifier(modifierLabel, value, operation);
            parentBall.GetStat(stat)?.AddModifier(statModifier);
        }

        public override void Deactivate()
        {
            if (!parentBall) return;
            parentBall.GetStat(stat)?.RemoveModifier(statModifier);
        }
        
        public override AbilityEffect Clone()
        {
            ModifyStat clone = new ModifyStat();
            clone.stat = stat;
            clone.modifierLabel = modifierLabel;
            clone.operation = operation;
            clone.value = value;
            return clone;
        }
    }
}