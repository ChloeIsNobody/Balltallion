using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class HealTarget : AbilityEffect
    {
        [SerializeField] private TargetTypes target = TargetTypes.Self;
        [SerializeField] private float healing;
        
        private enum TargetTypes
        {
            Self,
            Opponent,
        }
        
        public override void Activate()
        {
            BattleBall targetBall = target == TargetTypes.Self ? parentBall : TeamUtilities.GetNearestOpposingBallTo(parentBall);
            targetBall.Heal(healing);
        }

        public override void Deactivate()
        {
            
        }
        
        public override AbilityEffect Clone()
        {
            HealTarget clone = new HealTarget();
            clone.target = target;
            clone.healing = healing;
            return clone;
        }
    }
}