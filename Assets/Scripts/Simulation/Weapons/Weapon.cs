using System;
using System.Collections.Generic;
using Balltallion.Audio;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Projectiles;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private WeaponAudioHandler audioHandler;
        [SerializeField] private ProjectileShooter projectileShooter;
        
        [Header("Debouncing")]
        [SerializeField] private float hitDebounce = 0.45f;
        [SerializeField] private float parryDebounce = 0.5f;
        
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

        private void OnEnable()
        {
            projectileShooter.OnShot += OnWeaponShot;
        }

        private void OnDisable()
        {
            projectileShooter.OnShot -= OnWeaponShot;
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
            
            projectileShooter.gameObject.SetActive(weaponData.shootsProjectiles);
            if (weaponData.shootsProjectiles)
            {
                projectileShooter.LoadWeaponData(weaponData);
                projectileShooter.SetParentBall(parentBall);
            }
            
            audioHandler.LoadWeaponData(weaponData);
        }
        
        public void SetParentBall(BattleBall parentBall) => this.parentBall = parentBall;
        public void SetWeaponController(WeaponController weaponController) => this.weaponController = weaponController;

        private void SetSpriteAndCollider()
        {
            spriteRenderer.sprite = weaponData.weaponSprite;
            float spriteWidth = 0;
            float spriteHeight = 0;
            if (weaponData.weaponSprite != null)
            {
                spriteWidth = weaponData.weaponSprite.texture.width;
                spriteHeight = weaponData.weaponSprite.texture.height;
            }
            float colliderX = weaponData.relativeColliderSize.x * spriteWidth / 256.0f;
            float colliderY = weaponData.relativeColliderSize.y * spriteHeight / 256.0f;
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
            else if (other.TryGetComponent(out Projectile projectile))
            {
                if (CanDeflect(projectile)) Deflect(projectile);
            }
        }

        private bool CanHitBall(BattleBall otherBall)
        {
            if (weaponData.ignoresCollisions) return false;
            
            if (hitLog.TryGetValue(otherBall, out float lastHitTime))
            {
                float timeSinceLastHit = Time.time - lastHitTime;
                return timeSinceLastHit > hitDebounce;
            }
            return true;
        }

        private void BallHit(BattleBall hitBall)
        {
            // Attack enemy
            Vector2 directionalKnockback = CalculateKnockbackDir(hitBall.transform) * weaponData.directionalKnockback;
            Vector2 dirAwayFromSelf = (hitBall.transform.position - parentBall.transform.position).normalized;
            Vector2 awayKnockback = dirAwayFromSelf * weaponData.awayKnockback;
            AttackData attackData = new AttackData
            {
                source = parentBall,
                damage = weaponData.damage,
                knockback = directionalKnockback + awayKnockback
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
        
        private bool CanParry(Weapon otherWeapon)
        {
            if (weaponData.ignoresCollisions || !weaponData.canParry) return false;
            if (otherWeapon.weaponData.ignoresCollisions || !otherWeapon.weaponData.canParry) return false;
            
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

        private void Parry(Weapon hitWeapon)
        {
            BattleBall otherBall = hitWeapon.parentBall;
            Vector2 awayVector = (otherBall.transform.position - parentBall.transform.position).normalized;
            
            float parryForce = (weaponData.parryKnockback + hitWeapon.weaponData.parryKnockback) / 2.0f;
            
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
            ParryRecoil();
            hitWeapon.ParryRecoil();
            
            // Make sure to count this as a hit for both balls, to trigger a debounce for both
            hitLog[hitWeapon.parentBall] = Time.time;
            hitWeapon.hitLog[parentBall] = Time.time;
        }

        public void ParryRecoil()
        {
            HitFlash(parryColor);
            weaponController.SpinBoost();
            weaponController.ReverseSpinDirection();
        }
        
        private bool CanDeflect(Projectile otherProjectile)
        {
            if (weaponData.ignoresCollisions || !weaponData.canDeflect) return false;
            return otherProjectile.CanBeDeflected();
        }

        public void Deflect(Projectile projectile)
        {
            Vector2 dirAwayFromBall = (projectile.transform.position - parentBall.transform.position).normalized;
            projectile.Deflect(dirAwayFromBall);
            
            // Knockback self
            AttackData attackData = new AttackData
            {
                source = projectile.GetShooter(),
                damage = 0,
                knockback = projectile.GetMoveDirection() * projectile.GetKnockback() * 0.65f
            };
            parentBall.Attack(attackData);
            
            // Effects
            if (projectile.DestroyOnDeflect())
            {
                HitFlash(hitFlashColor);
                audioHandler.PlayAttackSound();
            }
            else
            {
                HitFlash(parryColor);
                audioHandler.PlayParrySound();
            }
            bool canReverseSpinDirection = projectile.GetDamage() >= weaponData.damage;
            if (canReverseSpinDirection)
            {
                float dotProd = Vector2.Dot(GetWeaponRotationVector(), CalculateKnockbackDir(projectile.transform));
                if (dotProd > 0) weaponController.ReverseSpinDirection();
                if (Mathf.Abs(dotProd) > 0.5f) weaponController.SpinBoost();
            }
        }

        private void OnWeaponShot()
        {
            audioHandler.PlayAttackSound();
        }
        
        public void HitFlash(Color color)
        {
            hitFlashTimer = hitFlashDuration;
            material.SetColor("_BlendColor", color);
            material.SetFloat("_BlendValue", 1.0f);
        }

        private Vector2 CalculateKnockbackDir(Transform otherTransform)
        {
            Vector2 weaponContactPoint = boxCollider.ClosestPoint(otherTransform.position);
            Vector2 dirAwayFromWeapon = ((Vector2)otherTransform.position - weaponContactPoint).normalized;
            return dirAwayFromWeapon;
        }

        private Vector2 GetWeaponRotationVector() => weaponController.GetSpinDirection() * transform.up;
        public BattleBall GetParentBall() => parentBall;
        public WeaponDataSO GetData() => weaponData;
    }
}