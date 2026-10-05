using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponDataSO weaponData;
        [SerializeField] private Weapon weaponPrefab;
        
        private int weaponCount;

        public void LoadWeaponStats(WeaponDataSO weaponData)
        {
            this.weaponData = weaponData;
            ClearChildTransforms();
            if (weaponData == null) return;
            InstantiateWeapons();
        }

        private void ClearChildTransforms()
        {
            List<Transform> children = new();
            foreach (Transform child in transform) children.Add(child);
            foreach (Transform child in children) DestroyImmediate(child.gameObject);
        }

        private void InstantiateWeapons()
        {
            for (int i = 0; i < weaponData.weaponCount; i++)
            {
                float weaponAngle = 0.0f + i * (360.0f / weaponData.weaponCount);
                
                GameObject rotator = new GameObject($"Rotator{i}");
                rotator.transform.SetParent(transform);
                rotator.transform.localPosition = Vector3.zero;
                rotator.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, weaponAngle);
                rotator.transform.localScale = Vector3.one;

                Weapon weapon = Instantiate(weaponPrefab, rotator.transform);
                weapon.gameObject.name = $"Weapon{i}";
                weapon.transform.localPosition = Vector3.right;
                weapon.transform.localRotation = Quaternion.identity;
                rotator.transform.localScale = Vector3.one;
            }
        }
    }
}
