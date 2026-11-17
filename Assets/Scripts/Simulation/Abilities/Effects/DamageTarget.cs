using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class DamageTarget : AbilityEffect
    {
        [SerializeField] private TargetTypes target = TargetTypes.Self;
        [SerializeField] private float damage;

        private enum TargetTypes
        {
            Self,
            Opponent,
        }
        
        public override void Activate()
        {
            BattleBall targetBall = target == TargetTypes.Self ? parentBall : TeamUtilities.GetNearestOpposingBallTo(parentBall);
            AttackData attackData = new AttackData()
            {
                source = parentBall,
                damage = damage,
            };
            targetBall.ReceiveAttack(attackData);
        }

        public override void Deactivate()
        {
            
        }
        
        public override AbilityEffect Clone()
        {
            DamageTarget clone = new DamageTarget();
            clone.target = target;
            clone.damage = damage;
            return clone;
        }
    }
}