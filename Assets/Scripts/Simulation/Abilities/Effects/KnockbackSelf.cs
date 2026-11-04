using System;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class KnockbackSelf : AbilityEffect
    {
        [SerializeField] private float knockbackStrength;
        [SerializeField] private KnockbackOptions knockbackOption;
        [SerializeField, Range(0.0f, 360.0f)] private float direction;
        [SerializeField, Range(0.0f, 360.0f)] private float randomSpread;

        private enum KnockbackOptions
        {
            Fixed,
            Random,
            TowardsOpponent,
            AwayFromOpponent,
        }


        public override void Activate()
        {
            
        }

        public override void Deactivate()
        {
            
        }
        
        public override AbilityEffect Clone()
        {
            KnockbackSelf clone = new KnockbackSelf();
            clone.knockbackStrength = knockbackStrength;
            clone.knockbackOption = knockbackOption;
            clone.direction = direction;
            clone.randomSpread = randomSpread;
            return clone;
        }
    }
}