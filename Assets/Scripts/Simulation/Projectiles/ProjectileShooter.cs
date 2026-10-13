using System;
using System.Collections;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Balltallion.Simulation.Projectiles
{
    public class ProjectileShooter : MonoBehaviour
    {
        public event Action OnShot;
        
        private WeaponDataSO weaponData;
        private Coroutine stateCoroutine;
        
        private BattleBall parentBall;
        
        public void LoadWeaponData(WeaponDataSO newData)
        {
            weaponData = newData;
        }
        
        private void Start() => ResetState();

        private void ResetState()
        {
            if (stateCoroutine != null) StopCoroutine(stateCoroutine);
            stateCoroutine = StartCoroutine(ReloadState());
        }
        
        public void SetParentBall(BattleBall parentBall) => this.parentBall = parentBall;

        private IEnumerator ShootState()
        {
            int currentAmmo = weaponData.ammo;
            while (currentAmmo > 0)
            {
                Shoot();
                currentAmmo--;
                if (currentAmmo == 0) break;
                yield return new WaitForSeconds(weaponData.delayBetweenShots);
            }
            stateCoroutine = StartCoroutine(ReloadState());
        }

        private IEnumerator ReloadState()
        {
            yield return new WaitForSeconds(weaponData.reloadDuration);
            stateCoroutine = StartCoroutine(ShootState());
        }

        private void Shoot()
        {
            Projectile newProjectile = Instantiate(weaponData.projectilePrefab);
            newProjectile.transform.position = transform.position;
            Vector2 shootDirection = CalculateShootDirection();
            newProjectile.Initialize(parentBall, shootDirection, parentBall.GetTeam());

            AttackData recoilKnockback = new AttackData()
            {
                source = parentBall,
                damage = 0,
                knockback = -shootDirection * weaponData.shotRecoil
            };
            parentBall.Attack(recoilKnockback);
            
            OnShot?.Invoke();
        }

        private Vector2 CalculateShootDirection()
        {
            float angleVariation = Random.Range(-weaponData.shotSpread/2.0f, weaponData.shotSpread/2.0f);
            return Quaternion.Euler(0.0f, 0.0f, angleVariation) * transform.right;
        }
    }
}