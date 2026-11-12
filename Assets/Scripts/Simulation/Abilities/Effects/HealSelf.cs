using System;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class HealSelf : AbilityEffect
    {
        [SerializeField] private float healing;
        
        public override void Activate()
        {
            parentBall.Heal(healing);
        }

        public override void Deactivate()
        {
            
        }
        
        public override AbilityEffect Clone()
        {
            HealSelf clone = new HealSelf();
            clone.healing = healing;
            return clone;
        }
    }
}