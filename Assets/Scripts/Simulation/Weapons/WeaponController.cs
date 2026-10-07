using System.Collections.Generic;
using Balltallion.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private BattleBall parentBall;
        [SerializeField] private WeaponDataSO weaponData;
        [SerializeField] private Weapon weaponPrefab;
        
        [Header("Animation")]
        [SerializeField, Range(1.0f, 5.0f)] private float hitSpinBoostMult = 1.0f;
        [SerializeField, Range(0.0f, 3.0f)] private float hitSpinBoostDuration = 0.5f;
        
        private int weaponCount;
        private float currentAngle = 0.0f;
        private float spinDirection = 1.0f;

        private float hitSpinBoostTimer = 0.0f;

        private void Start()
        {
            currentAngle = Random.Range(0, 360);
            spinDirection = Random.Range(0, 2) == 0 ? 1.0f : -1.0f;
        }

        public void LoadWeaponData(WeaponDataSO weaponData)
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
                weapon.gameObject.layer = parentBall.gameObject.layer;
                weapon.SetParentBall(parentBall);
                weapon.SetWeaponController(this);
                weapon.transform.localPosition = Vector3.right * weaponData.radialOffset;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.transform.localScale = Vector3.one * weaponData.size;
                weapon.LoadWeaponData(weaponData);
            }
        }

        private void Update()
        {
            float spinBoost = 1.0f;
            if (hitSpinBoostTimer > 0.0f)
            {
                hitSpinBoostTimer = Mathf.Max(hitSpinBoostTimer - Time.deltaTime, 0.0f);
                float animValue = 1.0f - hitSpinBoostTimer / hitSpinBoostDuration;
                animValue = 1.0f - Mathf.Pow(1.0f - animValue, 3.0f); // cubic ease out
                spinBoost = Mathf.Lerp(hitSpinBoostMult, 1.0f, animValue);
            }
            
            currentAngle += 90.0f * spinBoost * weaponData.spinSpeed * spinDirection * Time.deltaTime;
            if (currentAngle >= 360.0f) currentAngle -= 360.0f;
            if (currentAngle < 0.0f) currentAngle += 360.0f;
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, currentAngle);
        }
        
        public float GetSpinDirection() => spinDirection;

        public void ReverseSpinDirection() => spinDirection *= -1.0f;
        public void SpinBoost() => hitSpinBoostTimer = hitSpinBoostDuration;
        
    }
}
