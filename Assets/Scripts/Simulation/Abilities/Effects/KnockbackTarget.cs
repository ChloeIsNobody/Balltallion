using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class KnockbackTarget : AbilityEffect
    {
        [SerializeField] private TargetTypes target = TargetTypes.Self;
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

        private enum TargetTypes
        {
            Self,
            Opponent,
        }

        public override void Activate()
        {
            BattleBall targetBall = target == TargetTypes.Self ? parentBall : TeamUtilities.GetNearestOpposingBallTo(parentBall);
            if (resetVelocityBeforeKnockback) targetBall.ResetVelocity();
            
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
                BattleBall opponent = TeamUtilities.GetNearestOpposingBallTo(targetBall);
                if (opponent) knockbackDirection = (opponent.transform.position - targetBall.transform.position).normalized;
            }
            else if (knockbackOption == KnockbackOptions.AwayFromOpponent)
            {
                BattleBall opponent = TeamUtilities.GetNearestOpposingBallTo(targetBall);
                if (opponent) knockbackDirection = (targetBall.transform.position - opponent.transform.position).normalized;
            }
            
            AttackData attackData = new AttackData()
            {
                source = parentBall,
                knockback = knockbackStrength * knockbackDirection,
            };
            targetBall.ReceiveAttack(attackData);
        }

        public override void Deactivate()
        {
            
        }
        
        public override AbilityEffect Clone()
        {
            KnockbackTarget clone = new KnockbackTarget();
            clone.target = target;
            clone.knockbackStrength = knockbackStrength;
            clone.knockbackOption = knockbackOption;
            clone.direction = direction;
            clone.randomSpread = randomSpread;
            clone.resetVelocityBeforeKnockback = resetVelocityBeforeKnockback;
            return clone;
        }
    }
}