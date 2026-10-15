using System;
using System.Collections;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Projectiles;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Balltallion.Simulation.Abilities
{
    public class KnightAbility : MonoBehaviour
    {
        public event Action OnCooldownComplete;
        
        [Header("References")]
        [SerializeField] private Collider2D triggerCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshPro cooldownText;
        
        [Header("Stats")]
        [SerializeField] private int damage = 10;
        [SerializeField] private float knockback;
        [SerializeField] private float cooldown = 1.0f;
        [SerializeField] private float hitboxDuration = 0.2f;
        [SerializeField] private float hitDebounce = 0.45f;
        
        private float cooldownTimer;
        private bool onCooldown;
        
        private BattleBall parentBall;
        private Coroutine cooldownCoroutine;
        private Coroutine abilityCoroutine;
        
        private KeyControl abilityKey;
        
        // Tracks the last time a ball, projectile, or weapon, or whatever was hit 
        private List<GameObject> hitLog = new();

        private void Awake()
        {
            parentBall = GetComponentInParent<BattleBall>();
            gameObject.layer = TeamUtilities.GetWeaponTeamLayerID(parentBall.GetTeam());
            abilityKey = parentBall.GetTeam() == TeamNames.Player ? Keyboard.current.dKey : Keyboard.current.jKey;
            SetHitboxEnabled(false);
        }

        private void Start()
        {
            StartCooldown();
        }
        
        private void Update()
        {
            if (abilityKey.wasPressedThisFrame)
            {
                if (CanActivateAbility())
                {
                    Activate();
                }
                else
                {
                    Debug.Log($"{parentBall.name}'s ability is on cooldown! ({cooldownTimer:F1}s left...)");
                }
            }
        }

        private bool CanActivateAbility() => cooldownTimer <= 0.0f;

        private void Activate()
        {
            cooldownText.text = "";
            hitLog.Clear();
            if (abilityCoroutine != null) StopCoroutine(abilityCoroutine);
            abilityCoroutine = StartCoroutine(AbilityCoroutine());
        }

        private IEnumerator AbilityCoroutine()
        {
            Debug.Log($"{parentBall.name}'s ability activated!");
            SetHitboxEnabled(true);
            yield return new WaitForSeconds(hitboxDuration);
            SetHitboxEnabled(false);
            StartCooldown();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out BattleBall otherBall))
            {
                if (CanHitBall(otherBall)) HitBall(otherBall);
            }

            if (other.TryGetComponent(out Projectile projectile))
            {
                DeflectProjectile(projectile);
            }
        }
        
        private bool CanHitBall(BattleBall otherBall)
        {
            return !hitLog.Contains(otherBall.gameObject);
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

        private void DeflectProjectile(Projectile projectile)
        {
            Vector2 deflectDir = (projectile.transform.position - parentBall.transform.position).normalized;
            projectile.Deflect(deflectDir);
            hitLog.Add(projectile.gameObject);
        }

        private void SetHitboxEnabled(bool val)
        {
            triggerCollider.enabled = val;
            spriteRenderer.enabled = val;
        }

        private void StartCooldown()
        {
            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        }
        
        private IEnumerator CooldownCoroutine()
        {
            onCooldown = true;
            cooldownTimer = cooldown;

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
        }
    }
}