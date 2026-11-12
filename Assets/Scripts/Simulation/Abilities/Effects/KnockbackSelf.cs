using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Physics;
using Balltallion.Simulation.Teams;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class KnockbackSelf : AbilityEffect
    {
        [SerializeField] private float knockbackStrength;
        [SerializeField] private KnockbackOptions knockbackOption;
        [SerializeField, Range(0.0f, 360.0f)] private float direction;
        [SerializeField, Range(0.0f, 360.0f)] private float randomSpread;
        [SerializeField] private bool resetVelocityBeforeKnockback;

        private enum KnockbackOptions
        {
            Fixed,
            Random,
            TowardsOpponent,
            AwayFromOpponent,
        }


        public override void Activate()
        {
            if (resetVelocityBeforeKnockback) parentBall.ResetVelocity();
            
            Vector2 knockbackDirection = Vector2.zero;
            
            if (knockbackOption == KnockbackOptions.Fixed)
            {
                knockbackDirection = Quaternion.Euler(0.0f, 0.0f, direction) * Vector2.right;
            }
            else if (knockbackOption == KnockbackOptions.Random)
            {
                float randAngle = direction + Random.Range(-randomSpread/2.0f, randomSpread/2.0f);
                knockbackDirection = Quaternion.Euler(0.0f, 0.0f, randAngle) * Vector2.right;
            }
            else if (knockbackOption == KnockbackOptions.TowardsOpponent)
            {
                BattleBall opp = TeamUtilities.GetNearestOpposingBallTo(parentBall);
                if (opp) knockbackDirection = (opp.transform.position - parentBall.transform.position).normalized;
            }
            else if (knockbackOption == KnockbackOptions.AwayFromOpponent)
            {
                BattleBall opp = TeamUtilities.GetNearestOpposingBallTo(parentBall);
                if (opp) knockbackDirection = (parentBall.transform.position - opp.transform.position).normalized;
            }
            
            AttackData attackData = new AttackData()
            {
                source = parentBall,
                knockback = knockbackStrength * knockbackDirection,
            };
            parentBall.ReceiveAttack(attackData);
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
            clone.resetVelocityBeforeKnockback = resetVelocityBeforeKnockback;
            return clone;
        }
    }
}