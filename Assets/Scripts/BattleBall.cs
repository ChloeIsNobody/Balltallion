using UnityEngine;

namespace Balltallion
{
    public class BattleBall : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        private int health;
        
        private BallBody ballBody;

        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
            
            health = maxHealth;
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
        public float GetMaxHealth() => maxHealth;

        private void OnBounce(BallCollisionData data)
        {
            Debug.Log($"{name} bounced!");
        }

        private void OnBallCollision(BallCollisionData data)
        {
            BattleBall otherBall = data.otherBall.GetComponent<BattleBall>();
            if (!otherBall) return;

            int damage = (int)data.collisionPower;
            otherBall.TakeDamage(damage);
            
            Debug.Log($"{name} collided with {otherBall.name}, dealing {damage} damage!");
        }
    }
}
