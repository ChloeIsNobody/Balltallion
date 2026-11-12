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
        [SerializeField, Range(0, 100)] private int percentValue;
        [SerializeField] private bool triggersOnce;
        
        private bool isActive = false;

        private enum ComparisonType
        {
            LessThan,
            GreaterThan,
        }

        public override void Update()
        {
            bool conditionMet = comparisonType switch
            {
                ComparisonType.GreaterThan => parentBall.GetRelativeHealth() >= percentValue/100.0f,
                ComparisonType.LessThan => parentBall.GetRelativeHealth() <= percentValue/100.0f,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (!isActive && conditionMet)
            {
                isActive = true;
                Trigger();
            }

            if (isActive && !conditionMet && !triggersOnce)
            {
                isActive = false;
                Untrigger();
            }
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