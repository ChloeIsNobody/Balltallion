using System;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class ModifyStats : AbilityEffect
    {
        [InfoBox("Applies modifiers on trigger activate. Removes on trigger deactivate.")]
        [SerializeField] private string modifierLabel;

        [Header("Modifier Settings")]
        [SerializeField] private List<StatModifierData> statModifiers = new();

        [Serializable]
        private struct StatModifierData
        {
            public BattleBall.StatNames stat;
            public StatModifier.OperationType operation;
            public float value;
        }

        private List<StatModifier> statModifiersList;

        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            statModifiersList = new List<StatModifier>();
            foreach (StatModifierData data in statModifiers)
            {
                StatModifier statModifier = new StatModifier(modifierLabel, data.value, data.operation);
                statModifiersList.Add(statModifier);
            }
        }

        public override void Activate()
        {
            if (!parentBall) return;
            for (int i = 0; i < statModifiersList.Count; i++)
            {
                StatModifier statModifier = statModifiersList[i];
                BattleBall.StatNames stat = statModifiers[i].stat;
                parentBall.GetStat(stat)?.AddModifier(statModifier);
            }
        }

        public override void Deactivate()
        {
            if (!parentBall) return;
            for (int i = 0; i < statModifiersList.Count; i++)
            {
                StatModifier statModifier = statModifiersList[i];
                BattleBall.StatNames stat = statModifiers[i].stat;
                parentBall.GetStat(stat)?.RemoveModifier(statModifier);
            }
        }

        public override AbilityEffect Clone()
        {
            ModifyStats clone = new ModifyStats();
            clone.modifierLabel = modifierLabel;
            clone.statModifiers = new List<StatModifierData>(statModifiers);
            return clone;
        }
    }
}