using System.Collections.Generic;
using Balltallion.Audio;
using Balltallion.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private float hitDebounce = 0.45f;
        [SerializeField] private float parryDebounce = 0.5f;
        [SerializeField] private WeaponAudioHandler audioHandler;
        
        [Header("Hitflash")]
        [SerializeField] private Color hitFlashColor = Color.white;
        [SerializeField] private Color parryColor = Color.yellow;
        [SerializeField, Range(0.0f, 1.0f)] private float hitFlashDuration = 0.15f;
        [SerializeField, Range(0.0f, 1.0f)] private float hitFlashStrength = 1.0f;

        private Material material;
        private float hitFlashTimer = 0.0f;
        
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
            
            audioHandler.LoadWeaponData(weaponData);
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

        private void Update()
        {
            if (hitFlashTimer > 0.0f)
            {
                hitFlashTimer = Mathf.Max(hitFlashTimer - Time.deltaTime, 0.0f);
                float animValue = 1.0f - hitFlashTimer / hitFlashDuration;
                animValue = 1.0f - Mathf.Pow(1.0f - animValue, 3.0f); // cubic ease out
                float blendValue = Mathf.Lerp(hitFlashStrength, 0.0f, animValue);
                material.SetFloat("_BlendValue", blendValue);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BattleBall otherBall))
            {
                if (CanHitBall(otherBall)) BallHit(otherBall);
            }
            else if (other.TryGetComponent(out Weapon otherWeapon))
            {
                if (CanParry(otherWeapon)) Parry(otherWeapon);
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

        private bool CanParry(Weapon otherWeapon)
        {
            BattleBall otherBall = otherWeapon.parentBall;
            bool parentBallCanParry = true;
            bool otherBallCanParry = true;

            if (hitLog.TryGetValue(otherBall, out var lastHitTime))
            {
                float timeSinceLastHit = Time.time - lastHitTime;
                parentBallCanParry = timeSinceLastHit > parryDebounce;
            }
            if (otherWeapon.hitLog.TryGetValue(parentBall, out lastHitTime))
            {
                float timeSinceLastHit = Time.time - lastHitTime;
                otherBallCanParry = timeSinceLastHit > parryDebounce;
            }
            
            return parentBallCanParry && otherBallCanParry;
        }

        private void BallHit(BattleBall hitBall)
        {
            // Attack enemy
            AttackData attackData = new AttackData
            {
                source = parentBall,
                damage = weaponData.damage,
                knockback = CalculateKnockback(hitBall)
            };
            hitBall.Attack(attackData);
            audioHandler.PlayAttackSound();

            // Recoil self
            attackData.damage = 0;
            attackData.knockback *= -0.5f;
            parentBall.Attack(attackData);
            
            // Effects
            weaponController.SpinBoost();
            if (Vector2.Dot(GetWeaponRotationVector(), attackData.knockback) < 0)
            {
                weaponController.ReverseSpinDirection();
            }
            
            hitLog[hitBall] = Time.time;
        }

        private void Parry(Weapon hitWeapon)
        {
            BattleBall otherBall = hitWeapon.parentBall;
            Vector2 awayVector = (otherBall.transform.position - parentBall.transform.position).normalized;

            float parryForce = (weaponData.knockback + hitWeapon.weaponData.knockback) / 2.0f;
            
            // Knockback the other ball
            AttackData attackData = new AttackData
            {
                source = parentBall,
                damage = 0,
                knockback = awayVector * parryForce
            };
            otherBall.Attack(attackData);

            // Knockback self
            attackData.source = otherBall;
            attackData.knockback *= -1.0f;
            parentBall.Attack(attackData);
            
            // Effects
            audioHandler.PlayParrySound();
            HitFlash(parryColor);
            weaponController.SpinBoost();
            weaponController.ReverseSpinDirection();
            
            hitWeapon.HitFlash(parryColor);
            hitWeapon.weaponController.SpinBoost();
            hitWeapon.weaponController.ReverseSpinDirection();
            
            // Make sure to count this as a hit for both balls, to trigger a debounce for both
            hitLog[hitWeapon.parentBall] = Time.time;
            hitWeapon.hitLog[parentBall] = Time.time;
        }
        
        public void HitFlash(Color color)
        {
            hitFlashTimer = hitFlashDuration;
            material.SetColor("_BlendColor", color);
            material.SetFloat("_BlendValue", 1.0f);
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