using System.Collections.Generic;
using Balltallion.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private float hitDebounce = 0.45f;

        private Material material;
        
        private BattleBall parentBall;
        private WeaponController weaponController;
        
        private WeaponDataSO weaponData;

        // Tracks the last time a ball was hit 
        private Dictionary<BattleBall, float> hitLog = new();

        private void Awake()
        {
            if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            material = spriteRenderer.material;
        }
        
        public void LoadWeaponData(WeaponDataSO weaponData)
        {
            this.weaponData = weaponData;
            if (weaponData == null) return;
            SetSpriteAndCollider();

            BallDataSO ballData = parentBall.GetBallData();
            if (material != null)
            {
                material.SetFloat("_HueShift", ballData.animHueShift);
                material.SetFloat("_Saturation", ballData.animSaturation);
                material.SetFloat("_Brightness", ballData.animBrightness);
                material.SetFloat("_Contrast", ballData.animContrast);
            }
        }
        
        public void SetParentBall(BattleBall parentBall) => this.parentBall = parentBall;
        public void SetWeaponController(WeaponController weaponController) => this.weaponController = weaponController;

        private void SetSpriteAndCollider()
        {
            spriteRenderer.sprite = weaponData.weaponSprite;
            float colliderX = weaponData.relativeColliderSize.x * weaponData.weaponSprite.texture.width / 256.0f;
            float colliderY = weaponData.relativeColliderSize.y * weaponData.weaponSprite.texture.height / 256.0f;
            boxCollider.size = new Vector2(colliderX, colliderY);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BattleBall otherBall))
            {
                if (CanHitBall(otherBall)) BallHit(otherBall);
            }
            else if (other.TryGetComponent(out Weapon otherWeapon))
            {
                WeaponHit(otherWeapon);
            }
        }

        private bool CanHitBall(BattleBall otherBall)
        {
            if (hitLog.TryGetValue(otherBall, out float lastHitTime))
            {
                float timeSinceLastHit = Time.time - lastHitTime;
                return timeSinceLastHit > hitDebounce;
            }
            return true;
        }

        private void BallHit(BattleBall hitBall)
        {
            AttackData attackData = new AttackData
            {
                source = parentBall,
                damage = weaponData.damage,
                knockback = CalculateKnockback(hitBall)
            };
            hitBall.Attack(attackData);

            weaponController.SpinBoost();
            if (Vector2.Dot(GetWeaponRotationVector(), attackData.knockback) > 0)
            {
                weaponController.ReverseSpinDirection();
            }

            // Recoil
            attackData.damage = 0;
            attackData.knockback *= -0.5f;
            parentBall.Attack(attackData);
            
            hitLog[hitBall] = Time.time;
        }

        private void WeaponHit(Weapon hitWeapon)
        {
            
        }

        private Vector2 CalculateKnockback(BattleBall hitBall)
        {
            Vector2 weaponContactPoint = boxCollider.ClosestPoint(hitBall.transform.position);
            Vector2 dirAwayFromWeapon = ((Vector2)hitBall.transform.position - weaponContactPoint).normalized;
            return dirAwayFromWeapon * weaponData.knockback;
        }

        private Vector2 GetWeaponRotationVector() => weaponController.GetSpinDirection() * transform.up;
        
    }
}