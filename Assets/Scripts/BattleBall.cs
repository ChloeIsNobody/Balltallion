using NaughtyAttributes;
using UnityEngine;

namespace Balltallion
{
    public class BattleBall : MonoBehaviour
    {
        [SerializeField, Expandable] private BallStatsSO ballStats;
        [SerializeField] private SpriteRenderer ballSpriteRenderer;
        [SerializeField] private bool debugLogToConsole = false;
        
        private int health;
        
        private BallBody ballBody;

        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
            if (ballStats) LoadBallStats(ballStats);
        }

        [Button("Editor Refresh Stats")]
        private void EditorLoadStats()
        {
            if (ballStats != null) LoadBallStats(ballStats);
        }

        private void LoadBallStats(BallStatsSO ballStats)
        {
            this.ballStats = ballStats;
            health = ballStats.maxHealth;
            
            name = ballStats.displayName;
            ballSpriteRenderer.sprite = ballStats.ballSprite;
            
            if (!ballBody) ballBody = GetComponent<BallBody>();
            ballBody.LoadBallStats(ballStats);
        }
        
        private void OnEnable()
        {
            ballBody.OnBounce += OnBounce;
            ballBody.OnBallCollision += OnBallCollision;
        }

        private void OnDisable()
        {
            ballBody.OnBounce -= OnBounce;
            ballBody.OnBallCollision -= OnBallCollision;
        }

        public void Attack(AttackData attackData)
        {
            TakeDamage(attackData.damage);
            ballBody.ApplyKnockback(attackData.knockback);
            
            if (attackData.damage > 0 )
            {
                Color color = attackData.source ? attackData.source.GetColor() : Color.white;
                ScoreFloaterSpawner.Instance.SpawnScoreFloater(transform.position, attackData.damage, color);
            }
        }

        private void TakeDamage(int damage)
        {
            if (damage <= 0) return;
            health -= damage;
            if (health <= 0) Die();
        }

        public void Die()
        {
            if (debugLogToConsole) Debug.Log($"{name} died! :(");
            Destroy(gameObject, 0f);
        }
        
        public float GetHealth() => health;
        public float GetMaxHealth() => ballStats.maxHealth;

        private void OnBounce(BallCollisionData data)
        {
            if (debugLogToConsole) Debug.Log($"{name} bounced!");
        }

        private void OnBallCollision(BallCollisionData data)
        {
            BattleBall otherBall = data.otherBall.GetComponent<BattleBall>();
            if (!otherBall) return;
            
            AttackData attackData = new AttackData();
            attackData.source = this;
            
            if (ballStats.velocityScaledContactDamage)
            {
                attackData.damage = ballStats.GetVelocityScaledDamage(data.collisionPower);
            }
            else attackData.damage = ballStats.contactDamage;
            
            otherBall.Attack(attackData);
            
            if (debugLogToConsole) Debug.Log($"{name} collided with {otherBall.name}, dealing {attackData.damage} damage!");
        }
        
        public Sprite GetSprite() => ballSpriteRenderer.sprite;
        public Color GetColor() => ballStats.debugColor;
    }
}
