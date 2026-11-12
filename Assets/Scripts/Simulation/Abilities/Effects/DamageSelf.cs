using System;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class DamageSelf : AbilityEffect
    {
        [SerializeField] private float damage;
        
        public override void Activate()
        {
            AttackData attackData = new AttackData()
            {
                source = parentBall,
                damage = damage,
            };
            parentBall.ReceiveAttack(attackData);
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