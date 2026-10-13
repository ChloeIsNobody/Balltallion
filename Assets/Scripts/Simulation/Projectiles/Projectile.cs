using System.Collections;
using System.Collections.Generic;
using Balltallion.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [Header("Stats")]
        [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] private int damage;
        [SerializeField] private float knockback;
        [SerializeField, Range(0, 5)] private int maxBounces;
        [SerializeField] private bool destroyOnDeflect;
        
        [Header("Death Animation Settings")]
        [SerializeField] private BallAnimator ballAnimator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color deflectColor = Color.yellow;
        [SerializeField, Range(0.0f, 100.0f)] private float deathLaunchForce;
        [SerializeField, Range(0.0f, 90.0f)] private float deathLaunchSpread;
        [SerializeField, Range(0.0f, 10.0f)] private float deathDuration;
        [SerializeField, Range(0.0f, 1.0f)] private float deathSaturation;
        [SerializeField, Range(0.0f, 1.0f)] private float deathBrightness;
        [SerializeField, Range(0.0f, 1.0f)] private float deathContrast;

        private BattleBall shooter;
        private BallBody ballBody;
        private ContactFilter2D targetContactFilter; 
        private Material material;
        
        private Vector2 moveDirection;

        private int currentBounces = 0;
        private float deflectCooldown = 0.5f;
        private float deflectCooldownTimer = 0.0f;
        private bool isDead;
        
        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
            material = spriteRenderer.material;
        }
        
        public void Initialize(BattleBall shooter, Vector2 shootDirection, TeamNames team)
        {
            this.shooter = shooter;
            ballBody.SetVelocity(shootDirection * ballBody.GetSpeed());
            
            gameObject.layer = TeamUtilities.GetProjectileTeamLayerID(team);
            
            targetContactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = TeamUtilities.GetTeamLayerMask(TeamUtilities.GetOpposingTeam(shooter.GetTeam()))
            };
        }

        private void FixedUpdate()
        {
            if (isDead) return;
            
            if (deflectCooldownTimer > 0.0f)
            {
                deflectCooldownTimer -= Time.fixedDeltaTime;
            }
            else
            {
                HitDetection();
            }
        }

        private void HitDetection()
        {
            List<Collider2D> hits = new();
            Physics2D.OverlapCircle(transform.position, transform.localScale.x, targetContactFilter, hits);
            foreach (Collider2D hit in hits)
            {
                BattleBall ball = hit.GetComponent<BattleBall>();
                if (ball) HitBall(ball);
            }
        }

        private void HitBall(BattleBall ball)
        {
            AttackData attackData = new AttackData()
            {
                source = shooter,
                damage = damage,
                knockback = GetMoveDirection() * knockback
            };
            ball.Attack(attackData);
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (currentBounces >= maxBounces) Die();
            currentBounces++;
        }

        public void Deflect(Vector2 deflectDir)
        {
            if (destroyOnDeflect)
            {
                Die();
                ballAnimator.HitFlash();
            }
            else
            {
                ballBody.SetVelocity(deflectDir * ballBody.GetSpeed());
                ballAnimator.HitFlash(deflectColor);
                deflectCooldownTimer = deflectCooldown;
                currentBounces++;
            }
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            StartCoroutine(DeathCoroutine());
        }
        
        private IEnumerator DeathCoroutine()
        {
            float launchAngle = Random.Range(-deathLaunchSpread, deathLaunchSpread);
            float randomLaunchForce = deathLaunchForce * Random.Range(0.8f, 1.2f);
            Vector2 launchVector = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*launchAngle), Mathf.Cos(Mathf.Deg2Rad*launchAngle)) * randomLaunchForce;
            
            ballBody.SetVelocity(Vector2.zero);
            ballBody.ApplyKnockback(launchVector);
            ballBody.SetGravityScale(1.0f);
            ballBody.SetColliderEnabled(false);

            float timer = 0.0f;
            while (timer < deathDuration)
            {
                timer += Time.deltaTime;
                float t = timer / deathDuration;
                material.SetFloat("_Saturation", deathSaturation);
                material.SetFloat("_Brightness", deathBrightness);
                material.SetFloat("_Contrast", deathContrast);
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1f - t);
                yield return null;
            }
            
            Destroy(gameObject);
        }
        
        public bool CanBeDeflected() => deflectCooldownTimer <= 0.0f && !isDead;
        public BattleBall GetShooter() => shooter;
        public float GetVelocityMag() => ballBody.GetVelocity().magnitude;
        public float GetDamage() => damage;
        public float GetKnockback() => knockback;
        public bool DestroyOnDeflect() => destroyOnDeflect;
        public Vector2 GetMoveDirection() => ballBody.GetVelocity().normalized;
    }
}