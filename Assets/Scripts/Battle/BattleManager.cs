using Balltallion.Simulation;
using UnityEngine;

namespace Balltallion.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private BattleBall battleBallPrefab;
        
        [Header("Battle Setup")]
        [SerializeField] private BallDataSO playerBallData;
        [SerializeField] private BallDataSO enemyBallData;
        [SerializeField] private Transform playerSpawnTransform;
        [SerializeField] private Transform enemySpawnTransform;
        [SerializeField, Range(0.0f, 360.0f)] private float playerLaunchAngle = 0.0f;
        [SerializeField, Range(0.0f, 360.0f)] private float enemyLaunchAngle = 0.0f;
        [SerializeField, Range(0.0f, 360.0f)] private float launchAngleSpread = 0.0f;

        private BattleBall playerBall;
        private BattleBall enemyBall;
        
        private void Awake()
        {
            InstantiatePlayer();
            InstantiateEnemy();
        }

        private void InstantiatePlayer()
        {
            playerBall = Instantiate(battleBallPrefab);
            playerBall.LoadBallData(playerBallData, TeamNames.Player);
            playerBall.transform.position = playerSpawnTransform.position;
            
            float launchAngle = playerLaunchAngle + Random.Range(-launchAngleSpread/2.0f, launchAngleSpread/2.0f);
            Vector2 startVelocity = Vector2.zero;
            startVelocity.x = Mathf.Cos(Mathf.Deg2Rad * launchAngle);
            startVelocity.y = Mathf.Sin(Mathf.Deg2Rad * launchAngle);
            startVelocity *= playerBallData.speed;
            
            BallBody playerBody = playerBall.GetComponent<BallBody>();
            playerBody.SetVelocity(startVelocity);
        }

        private void InstantiateEnemy()
        {
            enemyBall = Instantiate(battleBallPrefab);
            enemyBall.LoadBallData(enemyBallData, TeamNames.Enemy);
            enemyBall.transform.position = enemySpawnTransform.position;
            
            float launchAngle = enemyLaunchAngle + Random.Range(-launchAngleSpread/2.0f, launchAngleSpread/2.0f);
            Vector2 startVelocity = Vector2.zero;
            startVelocity.x = Mathf.Cos(Mathf.Deg2Rad * launchAngle);
            startVelocity.y = Mathf.Sin(Mathf.Deg2Rad * launchAngle);
            startVelocity *= enemyBallData.speed;
            
            BallBody enemyBody = enemyBall.GetComponent<BallBody>();
            enemyBody.SetVelocity(startVelocity);
        }
    }
}