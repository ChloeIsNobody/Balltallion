using System;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class ModifyStatsTemporary : AbilityEffect
    {
        [InfoBox("Applies modifiers on trigger activate. Removes after a duration.")]
        [SerializeField] private string modifierLabel;
        [SerializeField] private float duration;

        [Header("Modifier Settings")]
        [SerializeField] private List<StatModifierData> statModifiers = new();

        private float timer;

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
            timer = duration;
        }

        public override void Deactivate()
        {
            
        }
        
        public override void Update()
        {
            if (timer > 0) timer -= Time.deltaTime;
            else RemoveModifiers();
        }

        private void RemoveModifiers()
        {
            for (int i = 0; i < statModifiersList.Count; i++)
            {
                StatModifier statModifier = statModifiersList[i];
                BattleBall.StatNames stat = statModifiers[i].stat;
                parentBall.GetStat(stat)?.RemoveModifier(statModifier);
            }
        }

        public override AbilityEffect Clone()
        {
            ModifyStatsTemporary clone = new ModifyStatsTemporary();
            clone.modifierLabel = modifierLabel;
            clone.duration = duration;
            clone.statModifiers = new List<StatModifierData>(statModifiers);
            return clone;
        }
    }
}