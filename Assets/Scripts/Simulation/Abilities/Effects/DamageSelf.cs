using System;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class DamageSelf : AbilityEffect
    {
        [SerializeField] private float damage;
        
        public override void Activate()
        {
            
        }

        public override void Deactivate()
        {
            
        }
        
        public override AbilityEffect Clone()
        {
            DamageSelf clone = new DamageSelf();
            clone.damage = damage;
            return clone;
        }
    }
}