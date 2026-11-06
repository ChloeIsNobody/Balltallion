using System.Collections;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Physics;
using Balltallion.Simulation.Projectiles;
using Balltallion.Simulation.Weapons;
using UnityEngine;

namespace Balltallion.Simulation.ActiveAbilities
{
    public class SuperShot : ActiveAbility
    {
        private BallBody ballBody;
        private WeaponController weaponController;
        private ProjectileShooter projectileShooter;
        
        [Header("Settings")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float shotRecoil;
        
        protected override void Awake()
        {
            base.Awake();

            ballBody = parentBall.GetComponent<BallBody>();
            weaponController = parentBall.GetComponentInChildren<WeaponController>();
            projectileShooter = weaponController.GetMainWeapon().GetComponentInChildren<ProjectileShooter>();
        }

        private void Start()
        {
            InitializeStateMachine(ShootState, nameof(ShootState));
        }
        
        protected override void CooldownEntered()
        {
            
        }

        protected override void AbilityReady()
        {
            
        }

        private IEnumerator ShootState()
        {
            ballBody.SetVelocity(Vector2.zero);
            float damage = parentBall.GetStat(BattleBall.StatNames.AbilityDamage).GetValue();
            projectileShooter.Shoot(projectilePrefab, damage, shotRecoil);
            yield return null;
            TransitionToCooldown();
        }
    }
}