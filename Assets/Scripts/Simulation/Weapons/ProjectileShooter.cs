using System.Collections;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class ProjectileShooter : MonoBehaviour
    {
        private WeaponDataSO weaponData;
        private Coroutine stateCoroutine;
        
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
            Debug.Log("BANG");
        }
    }
}