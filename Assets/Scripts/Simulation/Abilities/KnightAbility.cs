using System;
using System.Collections;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Projectiles;
using Balltallion.Simulation.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Balltallion.Simulation.Abilities
{
    public class KnightAbility : MonoBehaviour
    {
        public event Action OnCooldownStarted;
        public event Action OnCooldownComplete;
        public event Action OnAbilityActivated;
        public event Action OnAbilityActivationBlocked;
        
        [Header("References")]
        [SerializeField] private Collider2D triggerCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshPro cooldownText;
        
        [Header("Stats")]
        [SerializeField] private int damage = 10;
        [SerializeField] private float knockback;
        [SerializeField] private float cooldown = 1.0f;
        [SerializeField] private float chargeUpDuration = 0.2f;
        [SerializeField] private float hitboxDuration = 0.2f;
        [SerializeField] private float hitDebounce = 0.45f;
        
        private BattleBall parentBall;
        private Coroutine runnerCoroutine;
        private KeyControl abilityKey;
        
        private float cooldownTimer;
        private bool onCooldown;
        private bool isChargingUp;
        private bool activationTrigger;
        
        // Tracks the last time a ball, projectile, or weapon, or whatever was hit 
        private List<GameObject> hitLog = new();
        
        /*
         * Cooldown -> wait for input -> charge up -> slash -> Cooldown
         */

        private void Awake()
        {
            parentBall = GetComponentInParent<BattleBall>();
            gameObject.layer = TeamUtilities.GetWeaponTeamLayerID(parentBall.GetTeam());
            abilityKey = parentBall.GetTeam() == TeamNames.Player ? Keyboard.current.dKey : Keyboard.current.jKey;
            triggerCollider.enabled = false;
            spriteRenderer.enabled = false;
        }

        private void Start()
        {
            runnerCoroutine = StartCoroutine(RunnerCoroutine());
        }
        
        private void Update()
        {
            ReadInput();
        }

        private void ReadInput()
        {
            if (abilityKey.wasPressedThisFrame)
            {
                if (onCooldown)
                {
                    Debug.Log($"{parentBall.name}'s ability is on cooldown! ({cooldownTimer:F1}s left...)");
                    OnAbilityActivationBlocked?.Invoke();
                }
                else
                {
                    activationTrigger = true;
                }
            }
        }

        private IEnumerator RunnerCoroutine()
        {
            while (true)
            {
                yield return StartCoroutine(CooldownCoroutine());
                yield return StartCoroutine(AbilityCoroutine());
            }
        }
        
        private IEnumerator CooldownCoroutine()
        {
            onCooldown = true;
            cooldownTimer = cooldown;
            OnCooldownStarted?.Invoke();

            while (cooldownTimer > 0.0f)
            {
                cooldownTimer -= Time.deltaTime;
                cooldownText.text = $"{cooldownTimer:F1}s";
                yield return null;
            }
            
            cooldownTimer = 0.0f;
            onCooldown = false;
            cooldownText.text = abilityKey.name;
            Debug.Log($"{parentBall.name}'s ability is now ready!");
            OnCooldownComplete?.Invoke();
            
            yield return new WaitUntil(() => activationTrigger);
            activationTrigger = false;
            cooldownText.text = "";
            Debug.Log($"{parentBall.name}'s ability activated!");
            OnAbilityActivated?.Invoke();
        }

        private IEnumerator AbilityCoroutine()
        {
            hitLog.Clear();
            triggerCollider.enabled = true;
            isChargingUp = true;
            
            yield return new WaitForSeconds(chargeUpDuration);
            spriteRenderer.enabled = true;
            isChargingUp = false;
            
            yield return new WaitForSeconds(hitboxDuration);
            triggerCollider.enabled = false;
            spriteRenderer.enabled = false;
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out KnightAbility knightAbility))
            {
                Debug.Log("SUPER PARRY?!?!?");
            }
            
            if (isChargingUp) return;
            
            if (other.TryGetComponent(out BattleBall otherBall))
            {
                if (CanHitObject(otherBall.gameObject)) HitBall(otherBall);
            }

            if (other.TryGetComponent(out Weapon weapon))
            {
                if (CanHitObject(weapon.gameObject)) ParryWeapon(weapon);
            }

            if (other.TryGetComponent(out Projectile projectile))
            {
                if (CanHitObject(projectile.gameObject) && projectile.CanBeDeflected()) DeflectProjectile(projectile);
            }
        }

        private bool CanSuperParry(KnightAbility otherAbility)
        {
            return !hitLog.Contains(otherAbility.gameObject) && otherAbility.isChargingUp;
        }

        private void SuperParry(KnightAbility otherAbility)
        {
            // Prevent any more interactions
            hitLog.Add(otherAbility.gameObject);
            otherAbility.hitLog.Add(gameObject);
        }
        
        private bool CanHitObject(GameObject otherObject)
        {
            return !hitLog.Contains(otherObject);
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
            otherBall.Attack(attack);
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
            otherBall.Attack(attackData);

            // Knockback self
            attackData.source = otherBall;
            attackData.knockback *= -0.5f;
            parentBall.Attack(attackData);
            
            hitWeapon.ParryRecoil();
            hitLog.Add(hitWeapon.gameObject);
        }

        private void DeflectProjectile(Projectile projectile)
        {
            Vector2 deflectDir = (projectile.transform.position - parentBall.transform.position).normalized;
            projectile.Deflect(deflectDir);
            hitLog.Add(projectile.gameObject);
        }
    }
}