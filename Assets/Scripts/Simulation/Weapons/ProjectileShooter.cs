using System.Collections;
using Balltallion.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class ProjectileShooter : MonoBehaviour
    {
        private WeaponDataSO weaponData;
        private Coroutine stateCoroutine;
        
        private BattleBall parentBall;
        
        public void LoadWeaponData(WeaponDataSO newData)
        {
            weaponData = newData;
            ResetState();
        }

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
            newProjectile.Initialize(parentBall, CalculateShootDirection(), parentBall.GetTeam());

            AttackData recoilKnockback = new AttackData()
            {
                source = parentBall,
                damage = 0,
                knockback = -CalculateShootDirection() * weaponData.shotRecoil
            };
            parentBall.Attack(recoilKnockback);
        }

        private Vector2 CalculateShootDirection()
        {
            return transform.right;
        }
    }
}