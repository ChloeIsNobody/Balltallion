using Balltallion.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Weapons
{
    public class Projectile : MonoBehaviour
    {
        private BattleBall shooter;
        private Vector2 moveDirection;
        private int damage;
        private float speed;

        private bool initialized;
        
        public void Initialize(BattleBall shooter, Vector2 shootDirection, int damage, float speed, TeamNames team)
        {
            this.shooter = shooter;
            moveDirection = shootDirection;
            this.damage = damage;
            this.speed = speed;
            gameObject.layer = TeamUtilities.GetProjectileTeamLayer(team);
            
            initialized = true;
        }

        private void Update()
        {
            transform.position += (Vector3)moveDirection * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!initialized) return;
            
            if (other.TryGetComponent(out BattleBall otherBall))
            {
                AttackData attackData = new AttackData()
                {
                    source = shooter,
                    damage = damage,
                    knockback = Vector2.zero
                };
                otherBall.Attack(attackData);
            }
            
            Destroy(gameObject);
        }
    }
}