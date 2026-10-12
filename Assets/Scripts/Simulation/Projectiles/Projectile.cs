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

        private BattleBall shooter;
        private BallBody ballBody;
        private ContactFilter2D targetContactFilter; 
        
        private Vector2 moveDirection;

        private int currentBounces = 0;
        private float deflectCooldown = 0.5f;
        private float deflectTimer = 0.0f;
        
        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
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
            List<Collider2D> hits = new();
            Physics2D.OverlapCircle(transform.position, transform.localScale.x, targetContactFilter, hits);
            foreach (Collider2D hit in hits)
            {
                BattleBall ball = hit.GetComponent<BattleBall>();
                if (ball) HitBall(ball);
            }
            
            if (deflectTimer > 0.0f) deflectTimer -= Time.fixedDeltaTime;
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
            if (currentBounces >= maxBounces) Destroy(gameObject);
            currentBounces++;
        }

        public void Deflect(Vector2 newDirection)
        {
            ballBody.SetVelocity(newDirection.normalized * ballBody.GetVelocity().magnitude);
            if (currentBounces >= maxBounces) Destroy(gameObject);
            currentBounces++;
            deflectTimer = deflectCooldown;
        } 
        
        public bool CanBeDeflected() => deflectTimer <= 0.0f;
        
        public BattleBall GetShooter() => shooter;
        public float GetDamage() => damage;
        public float GetKnockback() => knockback;
        public Vector2 GetMoveDirection() => ballBody.GetVelocity().normalized;
    }
}