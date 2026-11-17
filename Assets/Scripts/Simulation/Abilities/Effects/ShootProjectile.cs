using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Projectiles;
using Balltallion.Simulation.Teams;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class ShootProjectile : AbilityEffect
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float damage;
        [SerializeField] private float recoil;
        [SerializeField] private int count = 1;
        
        [Header("Aiming")]
        [SerializeField] private AimOptions aimOption;
        [SerializeField, Range(0.0f, 360.0f)] private float direction;
        [SerializeField, Range(0.0f, 360.0f)] private float randomSpread;
        private enum AimOptions
        {
            Fixed,
            Random,
            TowardsOpponent,
            AwayFromOpponent,
        }
        
        public override void Activate()
        {
            for (int i = 0; i < count; i++)
            {
                Shoot(projectilePrefab, damage, recoil);
            }
        }
        
        public void Shoot(Projectile projectilePrefab, float damage, float recoil)
        {
            Projectile newProjectile = Object.Instantiate(projectilePrefab);
            newProjectile.transform.position = parentBall.transform.position;
            Vector2 shootDirection = GetShootDirection();
            newProjectile.Initialize(parentBall, damage, shootDirection, parentBall.GetTeam());
            newProjectile.SetDeflectionCooldownTimer(0.2f);

            AttackData recoilKnockback = new AttackData()
            {
                source = parentBall,
                damage = 0,
                knockback = -shootDirection * recoil
            };
            parentBall.ReceiveAttack(recoilKnockback);
        }
        
        private Vector2 GetShootDirection()
        {
            if (aimOption == AimOptions.Fixed)
            {
                return Quaternion.Euler(0.0f, 0.0f, direction) * Vector2.right;
            }
            if (aimOption == AimOptions.Random)
            {
                float randAngle = direction + Random.Range(-randomSpread/2.0f, randomSpread/2.0f);
                return Quaternion.Euler(0.0f, 0.0f, randAngle) * Vector2.right;
            }
            if (aimOption == AimOptions.TowardsOpponent)
            {
                BattleBall opponent = TeamUtilities.GetNearestOpposingBallTo(parentBall);
                if (opponent) return (opponent.transform.position - parentBall.transform.position).normalized;
            }
            if (aimOption == AimOptions.AwayFromOpponent)
            {
                BattleBall opponent = TeamUtilities.GetNearestOpposingBallTo(parentBall);
                if (opponent) return (parentBall.transform.position - opponent.transform.position).normalized;
            }
            
            return Vector2.zero;
        }

        public override void Deactivate()
        {
            
        }

        public override AbilityEffect Clone()
        {
            ShootProjectile clone = new ShootProjectile();
            clone.projectilePrefab = projectilePrefab;
            clone.damage = damage;
            clone.recoil = recoil;
            clone.count = count;
            clone.aimOption = aimOption;
            clone.direction = direction;
            clone.randomSpread = randomSpread;
            return clone;
        }
    }
}