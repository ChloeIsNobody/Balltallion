using System.Collections;
using Balltallion.Simulation.Physics;
using Balltallion.Simulation.Projectiles;
using Balltallion.Simulation.Weapons;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
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
            projectileShooter.Shoot(projectilePrefab, shotRecoil);
            yield return null;
            TransitionToCooldown();
        }
    }
}