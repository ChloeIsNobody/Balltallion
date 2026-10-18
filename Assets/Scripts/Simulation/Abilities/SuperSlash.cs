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
    public class SuperSlash : MonoBehaviour
    {
        public event Action OnCooldownStarted;
        public event Action OnCooldownComplete;
        public event Action OnAbilityActivated;
        public event Action OnAbilityActivationBlocked;
        
        [Header("References")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private Animator animator;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private TextMeshPro cooldownText;
        [SerializeField] private AudioSource audioSource;
        
        [Header("Stats")]
        [SerializeField] private int damage = 10;
        [SerializeField] private float knockback;
        [SerializeField] private float cooldown = 1.0f;
        [SerializeField] private float radius = 1.0f;
        [SerializeField] private float chargeUpDuration = 0.2f;
        [SerializeField] private float hitboxDuration = 0.2f;

        [Header("Animation")]
        [SerializeField] private float spinBoostMult = 1.0f;
        [SerializeField] private float spinBoostDuration = 1.0f;

        [Header("Audio")]
        [SerializeField] private AudioResource slashSound;
        [SerializeField] private AudioResource superParrySound;
        
        private BattleBall parentBall;
        private WeaponController weaponController;
        private Coroutine runnerCoroutine;
        private KeyControl abilityKey;
        
        // Tracks the last time a ball, projectile, or weapon, or whatever was hit 
        private List<GameObject> hitLog = new();
        
        private ContactFilter2D targetContactFilter; 
        private HitDetectionModes hitDetectionMode = HitDetectionModes.None;
        private enum HitDetectionModes {None, ChargeUp, Slash}
        
        private float cooldownTimer;
        private bool onCooldown;
        private bool activationTrigger;
        private bool chargeUpSkipTrigger;
        private bool isParry;
        
        /*
         * Cooldown -> wait for input -> charge up -> slash -> Cooldown
         */

        private void OnValidate()
        {
            visualTransform.transform.localScale = new Vector3(radius*2.0f, radius*2.0f, 1.0f);
            circleCollider.radius = radius;
        }

        private void Awake()
        {
            OnValidate();
            parentBall = GetComponentInParent<BattleBall>();
            weaponController = parentBall.GetComponentInChildren<WeaponController>();
            hitDetectionMode = HitDetectionModes.None;
            animator.Play("Idle");
            SetupTeam();
        }

        private void SetupTeam()
        {
            gameObject.layer = TeamUtilities.GetWeaponTeamLayerID(parentBall.GetTeam());
            abilityKey = parentBall.GetTeam() == TeamNames.Player ? Keyboard.current.dKey : Keyboard.current.jKey;
            
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
            runnerCoroutine = StartCoroutine(RunnerCoroutine());
        }
        
        private void Update()
        {
            ReadInput();
        }

        private void FixedUpdate()
        {
            HitDetection();
        }

        private void ReadInput()
        {
            if (abilityKey.wasPressedThisFrame)
            {
                if (onCooldown)
                {
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
            animator.Play("AbilityReady");
            OnCooldownComplete?.Invoke();
            
            yield return new WaitUntil(() => activationTrigger);
            activationTrigger = false;
            cooldownText.text = "";
            OnAbilityActivated?.Invoke();
        }

        private IEnumerator AbilityCoroutine()
        {
            hitLog.Clear();
            isParry = false;
            ChargeUp();
            
            float chargeUpTimer = chargeUpDuration;
            while (chargeUpTimer > 0.0f)
            {
                chargeUpTimer -= Time.deltaTime;
                if (chargeUpSkipTrigger)
                {
                    chargeUpSkipTrigger = false;
                    break;
                }
                yield return null;
            }
            
            Slash();
            
            yield return new WaitForSeconds(hitboxDuration);
            hitDetectionMode = HitDetectionModes.None;
            animator.Play("Idle");
            
            yield return new WaitForSeconds(0.25f);
            weaponController.SetWeaponsEnabled(true);
        }

        private void ChargeUp()
        {
            hitDetectionMode = HitDetectionModes.ChargeUp;
            weaponController.ReverseSpinDirection();
            weaponController.SetWeaponsEnabled(false);
            animator.speed = 1.0f/chargeUpDuration;
            animator.Play("ChargeUp");
        }

        private void Slash()
        {
            hitDetectionMode = HitDetectionModes.Slash;
            weaponController.ReverseSpinDirection();
            weaponController.SpinBoost(spinBoostMult, spinBoostDuration);
            animator.speed = 1.0f/hitboxDuration;
            if (isParry)
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
                if (otherSlash) SuperParry(otherSlash);
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

        private void SuperParry(SuperSlash otherAbility)
        {
            // Play slash animation immediately by skipping charge up
            chargeUpSkipTrigger = true;
            otherAbility.chargeUpSkipTrigger = true;
            isParry = true;
            otherAbility.isParry = true;

            BattleBall otherBall = otherAbility.parentBall;
            Vector2 awayVector = (otherBall.transform.position - parentBall.transform.position).normalized;
            float parryForce = (knockback + otherAbility.knockback) / 4.0f;
            
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
            
            // Prevent any more interactions
            hitLog.Add(otherAbility.parentBall.gameObject);
            otherAbility.hitLog.Add(parentBall.gameObject);
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