using UnityEngine;

namespace Balltallion
{
    public class BattleBall : MonoBehaviour
    {
        [SerializeField] private int health = 100;
        
        private BallBody ballBody;

        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
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
