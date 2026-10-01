using Balltallion.BallSimulation;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Battle
{
    public class BattleBall : MonoBehaviour
    {
        [SerializeField, Expandable] private BallDataSO ballData;
        [SerializeField] private BallAnimator ballAnimator;
        [SerializeField] private bool debugLogToConsole = false;
        
        private int health;
        
        private BallBody ballBody;

        private void Awake()
        {
            ballBody = GetComponent<BallBody>();
        }

        private void Start()
        {
            if (ballData) LoadBallStats(ballData);
        }

        [Button("Editor Refresh Stats")]
        private void EditorLoadStats()
        {
            if (ballData != null) LoadBallStats(ballData);
        }

        private void LoadBallStats(BallDataSO ballData)
        {
            this.ballData = ballData;
            health = ballData.maxHealth;
            
            name = ballData.displayName;
            ballAnimator.SetSprite(ballData.ballSprite);
            
            if (!ballBody) ballBody = GetComponent<BallBody>();
            ballBody.LoadBallStats(ballData);
            ballAnimator.SetSpringData(ballData.mass);
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
        public float GetMaxHealth() => ballData.maxHealth;

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
            
            if (ballData.velocityScaledContactDamage)
            {
                attackData.damage = ballData.GetVelocityScaledDamage(data.collisionPower);
            }
            else attackData.damage = ballData.contactDamage;
            
            otherBall.Attack(attackData);
            
            if (debugLogToConsole) Debug.Log($"{name} collided with {otherBall.name}, dealing {attackData.damage} damage!");
        }
        
        public Sprite GetSprite() => ballData.ballSprite;
        public Color GetColor() => ballData.debugColor;
    }
}
