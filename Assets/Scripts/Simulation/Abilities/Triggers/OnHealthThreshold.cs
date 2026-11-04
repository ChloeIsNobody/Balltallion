using System;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnHealthThreshold : AbilityTrigger
    {
        [InfoBox("Trigger activates when health is...")]
        [SerializeField] private ComparisonType comparisonType;
        [SerializeField] private float percentValue;
        [SerializeField] private bool triggersOnce;

        private bool triggerLock;

        private enum ComparisonType
        {
            LessThan,
            LessThanOrEqualTo,
            GreaterThan,
            GreaterThanOrEqualTo,
        }
        
        public override AbilityTrigger Clone()
        {
            OnHealthThreshold clone = new OnHealthThreshold();
            clone.comparisonType = comparisonType;
            clone.percentValue = percentValue;
            clone.triggersOnce = triggersOnce;
            return clone;
        }
    }
}