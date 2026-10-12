using System;
using System.Collections.Generic;
using Balltallion.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [Header("Stats")] [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] private int damage;
        [SerializeField, Range(0.0f, 25.0f)] private float speed = 5.0f;
        [SerializeField] private float knockback;
        [SerializeField, Range(0, 5)] private int maxBounces;

        private BattleBall shooter;
        private Rigidbody2D rb;
        private ContactFilter2D targetContactFilter; 
        
        private Vector2 moveDirection;
        private bool initialized;

        private int currentBounces = 0;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        public void Initialize(BattleBall shooter, Vector2 shootDirection, TeamNames team)
        {
            this.shooter = shooter;
            rb.linearVelocity = shootDirection * speed;
            
            gameObject.layer = TeamUtilities.GetProjectileTeamLayerID(team);
            
            targetContactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = TeamUtilities.GetTeamLayerMask(TeamUtilities.GetOpposingTeam(shooter.GetTeam()))
            };

            initialized = true;
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
        }

        private void HitBall(BattleBall ball)
        {
            AttackData attackData = new AttackData()
            {
                source = shooter,
                damage = damage,
                knockback = Vector2.zero
            };
            ball.Attack(attackData);
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (currentBounces >= maxBounces) Destroy(gameObject);
            currentBounces++;
        }
    }
}