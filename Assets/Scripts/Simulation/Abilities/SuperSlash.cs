using System;
using System.Collections;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Projectiles;
using Balltallion.Simulation.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Balltallion.Simulation.Abilities
{
    public class SuperSlash : ActiveAbility
    {
        [Header("References")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private AudioSource audioSource;
        
        [Header("Slash Stats")]
        [SerializeField] private int damage = 10;
        [SerializeField] private float knockback;
        [SerializeField] private float radius = 1.0f;
        [SerializeField] private float chargeUpDuration = 0.2f;
        [SerializeField] private float hitboxDuration = 0.2f;

        [Header("Animation")]
        [SerializeField] private float spinBoostMult = 1.0f;
        [SerializeField] private float spinBoostDuration = 1.0f;

        [Header("Audio")]
        [SerializeField] private AudioResource slashSound;
        [SerializeField] private AudioResource superParrySound;
        
        private WeaponController weaponController;
        
        // Tracks the last time a ball, projectile, or weapon, or whatever was hit 
        private List<GameObject> hitLog = new();
        
        private ContactFilter2D targetContactFilter; 
        private HitDetectionModes hitDetectionMode = HitDetectionModes.None;
        private enum HitDetectionModes {None, ChargeUp, Slash}
        
        private bool parryTriggered;
        
        private void OnValidate()
        {
            visualTransform.transform.localScale = new Vector3(radius*2.0f, radius*2.0f, 1.0f);
            circleCollider.radius = radius;
        }

        protected override void Awake()
        {
            OnValidate();
            base.Awake();
            weaponController = parentBall.GetComponentInChildren<WeaponController>();
            hitDetectionMode = HitDetectionModes.None;
            animator.Play("Idle");
            SetupContactFilter();
        }

        private void SetupContactFilter()
        {
            TeamNames opposingTeam = TeamUtilities.GetOpposingTeam(parentBall.GetTeam());
            LayerMask layerMask = TeamUtilities.GetTeamLayerMask(opposingTeam);
            layerMask |= TeamUtilities.GetWeaponTeamLayerMask(opposingTeam);
            layerMask |= TeamUtilities.GetProjectileTeamLayerMask(opposingTeam);
            targetContactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = layerMask,
                useTriggers = true
            };
        }

        private void Start()
        {
            InitializeStateMachine(ChargingState, nameof(ChargingState));
        }
        
        protected override void CooldownEntered()
        {
            animator.Play("Idle");
        }

        protected override void AbilityReady()
        {
            animator.Play("AbilityReady");
        }

        private IEnumerator ChargingState()
        {
            hitLog.Clear();
            parryTriggered = false;
            
            hitDetectionMode = HitDetectionModes.ChargeUp;
            weaponController.ReverseSpinDirection();
            weaponController.SetWeaponsEnabled(false);
            animator.speed = 1.0f/chargeUpDuration;
            animator.Play("ChargeUp");
            
            float chargeUpTimer = chargeUpDuration;
            while (chargeUpTimer > 0.0f)
            {
                chargeUpTimer -= Time.deltaTime;
                if (parryTriggered) break;
                yield return null;
            }
            
            TransitionToState(SlashState(), nameof(SlashState));
        }

        private IEnumerator SlashState()
        {
            hitDetectionMode = HitDetectionModes.Slash;
            weaponController.ReverseSpinDirection();
            weaponController.SpinBoost(spinBoostMult, spinBoostDuration);
            animator.speed = 1.0f/hitboxDuration;
            if (parryTriggered)
            {
                animator.Play("ParrySlash");
                audioSource.resource = superParrySound;
                audioSource.Play();
            }
            else
            {
                animator.Play("Slash");
                audioSource.resource = slashSound;
                audioSource.Play();
            }
            
            yield return new WaitForSeconds(hitboxDuration);
            hitDetectionMode = HitDetectionModes.None;
            animator.Play("Idle");
            
            yield return new WaitForSeconds(0.25f);
            weaponController.SetWeaponsEnabled(true);
            
            TransitionToCooldown();
        }
        
        private void FixedUpdate()
        {
            HitDetection();
        }
        
        private void HitDetection()
        {
            if (hitDetectionMode == HitDetectionModes.None) return;
            
            List<Collider2D> hits = new();
            Physics2D.OverlapCircle(transform.position, radius, targetContactFilter, hits);

            if (hitDetectionMode == HitDetectionModes.ChargeUp)
            {
                SuperSlash otherSlash = hits.Find(c => 
                    c.TryGetComponent(out SuperSlash otherSlash)
                    && !hitLog.Contains(otherSlash.gameObject)
                    && otherSlash.hitDetectionMode == HitDetectionModes.ChargeUp
                    )?.GetComponent<SuperSlash>();
                if (otherSlash) {SuperParry(otherSlash); return; }
                
                BubbleGumShield bubbleGumShield = hits.Find(c =>
                    c.TryGetComponent(out BubbleGumShield bubbleGumShield)
                    && !hitLog.Contains(bubbleGumShield.GetParentBall().gameObject)
                    )?.GetComponent<BubbleGumShield>();
                if (bubbleGumShield && bubbleGumShield.IsShieldActive()) PopShield(bubbleGumShield);
            }

            if (hitDetectionMode == HitDetectionModes.Slash)
            {
                List<Projectile> projectiles = hits.FindAll(c =>
                    c.TryGetComponent(out Projectile projectile)
                    && !hitLog.Contains(projectile.gameObject)
                    && projectile.CanBeDeflected()
                    ).ConvertAll(c => c.GetComponent<Projectile>());
                foreach (Projectile projectile in projectiles) DeflectProjectile(projectile);
                
                BattleBall otherBall = hits.Find(c => 
                    c.TryGetComponent(out BattleBall otherBall)
                    && !hitLog.Contains(otherBall.gameObject)
                    )?.GetComponent<BattleBall>();
                
                if (otherBall) { HitBall(otherBall); return; }

                Weapon otherWeapon = hits.Find(c => 
                    c.TryGetComponent(out Weapon otherWeapon)
                    && !hitLog.Contains(otherWeapon.GetParentBall().gameObject)
                    )?.GetComponent<Weapon>();
                if (otherWeapon) ParryWeapon(otherWeapon);
            }
        }

        private void SuperParry(SuperSlash otherSlash)
        {
            // Setting the parry flag to true skips the charging state
            parryTriggered = true;
            otherSlash.parryTriggered = true;

            BattleBall otherBall = otherSlash.GetParentBall();
            Vector2 awayVector = (otherBall.transform.position - parentBall.transform.position).normalized;
            float parryForce = (knockback + otherSlash.knockback) / 4.0f;
            
            // Knockback the other ball
            AttackData attackData = new AttackData
            {
                source = parentBall,
                damage = 0,
                knockback = awayVector * parryForce
            };
            otherBall.ReceiveAttack(attackData);

            // Knockback self
            attackData.source = otherBall;
            attackData.knockback *= -1.0f;
            parentBall.ReceiveAttack(attackData);
            
            // Prevent any more interactions
            hitLog.Add(otherSlash.parentBall.gameObject);
            otherSlash.hitLog.Add(parentBall.gameObject);
        }
        
        // Refactor this later. Don't leave it hardcoded
        private void PopShield(BubbleGumShield shield)
        {
            shield.Pop();
            parryTriggered = true;
            
            BattleBall otherBall = shield.GetParentBall();
            Vector2 awayVector = (otherBall.transform.position - parentBall.transform.position).normalized;
            float parryForce = knockback;
            
            // Knockback the other ball
            AttackData attackData = new AttackData
            {
                source = parentBall,
                damage = 0,
                knockback = awayVector * (parryForce * 0.5f)
            };
            otherBall.ReceiveAttack(attackData);
            
            // Knockback self
            attackData.source = otherBall;
            attackData.knockback *= -2.0f;
            parentBall.ReceiveAttack(attackData);
            
            hitLog.Add(shield.GetParentBall().gameObject);
        }

        private void HitBall(BattleBall otherBall)
        {
            Vector2 knockbackDir = (otherBall.transform.position - parentBall.transform.position).normalized;
            AttackData attack = new AttackData()
            {
                source = parentBall,
                damage = damage,
                knockback = knockbackDir * knockback
            };
            otherBall.ReceiveAttack(attack);
            hitLog.Add(otherBall.gameObject);
        }

        private void ParryWeapon(Weapon hitWeapon)
        {
            BattleBall otherBall = hitWeapon.GetParentBall();
            Vector2 awayVector = (otherBall.transform.position - parentBall.transform.position).normalized;
            
            float parryForce = (knockback + hitWeapon.GetData().parryKnockback) / 2.0f;
            
            // Knockback the other ball
            AttackData attackData = new AttackData
            {
                source = parentBall,
                damage = 0,
                knockback = awayVector * parryForce
            };
            otherBall.ReceiveAttack(attackData);

            // Knockback self
            attackData.source = otherBall;
            attackData.knockback *= -0.5f;
            parentBall.ReceiveAttack(attackData);
            
            hitWeapon.ParryRecoil();
            hitLog.Add(hitWeapon.GetParentBall().gameObject);
        }

        private void DeflectProjectile(Projectile projectile)
        {
            Vector2 deflectDir = (projectile.transform.position - parentBall.transform.position).normalized;
            projectile.Deflect(deflectDir);
            hitLog.Add(projectile.gameObject);
        }
    }
}