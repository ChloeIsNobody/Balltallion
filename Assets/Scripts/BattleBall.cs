using NaughtyAttributes;
using UnityEngine;

namespace Balltallion
{
    public class BattleBall : MonoBehaviour
    {
        [SerializeField, Expandable] private BallStatsSO ballStats;
        [SerializeField] private SpriteRenderer ballSpriteRenderer;
        
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

        public void TakeDamage(int damage)
        {
            if (damage <= 0) return;
            
            health -= damage;
            
            string damageText = damage.ToString();
            ScoreFloaterSpawner.Instance.SpawnScoreFloater(transform.position, damageText, Color.white);
            
            if (health < 0) Die();
        }

        public void Die()
        {
            Debug.Log($"{name} died! :(");
            Destroy(gameObject);
        }
        
        public float GetHealth() => health;
        public float GetMaxHealth() => ballStats.maxHealth;

        private void OnBounce(BallCollisionData data)
        {
            Debug.Log($"{name} bounced!");
        }

        private void OnBallCollision(BallCollisionData data)
        {
            BattleBall otherBall = data.otherBall.GetComponent<BattleBall>();
            if (!otherBall) return;

            int damage;
            if (ballStats.velocityScaledContactDamage) damage = ballStats.GetVelocityScaledDamage(data.collisionPower);
            else damage = ballStats.contactDamage;
            
            otherBall.TakeDamage(damage);
            
            Debug.Log($"{name} collided with {otherBall.name}, dealing {damage} damage!");
        }
    }
}
