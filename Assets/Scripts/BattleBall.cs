using UnityEngine;

namespace Balltallion
{
    public class BattleBall : MonoBehaviour
    {
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

        private void OnBounce(BallCollisionData data)
        {
            Debug.Log($"{name} bounced!");
        }

        private void OnBallCollision(BallCollisionData data)
        {
            BattleBall otherBall = data.otherBall.GetComponent<BattleBall>();
            if (!otherBall) return;

            float damage = data.collisionPower;
            
            Debug.Log($"{name} collided with {otherBall.name}, dealing {damage} damage!");
        }
    }
}
