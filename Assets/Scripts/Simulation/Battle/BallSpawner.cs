using Balltallion.Simulation.Physics;
using Balltallion.Simulation.Teams;
using Balltallion.TeamPicker;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Battle
{
    public class BallSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] private BattleBall battleBallPrefab;
        [SerializeField] private Rect playerSpawnRect;
        [SerializeField] private Rect enemySpawnRect;
        [SerializeField, Range(0.0f, 360.0f)] private float playerLaunchAngle = 0.0f;
        [SerializeField, Range(0.0f, 360.0f)] private float enemyLaunchAngle = 0.0f;
        [SerializeField, Range(0.0f, 360.0f)] private float launchAngleSpread = 0.0f;
        
        public BattleBall InstantiateBall(BallLoadout ballLoadout, TeamNames team)
        {
            BallDataSO ballData = ballLoadout.GetBallData();
            BattleBall ball = Instantiate(battleBallPrefab);
            ball.LoadBallData(ballData, team);

            Rect spawnRect = team == TeamNames.Player ? playerSpawnRect : enemySpawnRect;
            ball.transform.position = GetRandomPositionInRect(spawnRect);
            
            LaunchBall(ball, team);

            return ball;
        }
        
        private void LaunchBall(BattleBall ball, TeamNames team)
        {
            float launchAngle = team == TeamNames.Player ? playerLaunchAngle : enemyLaunchAngle;
            launchAngle += Random.Range(-launchAngleSpread/2.0f, launchAngleSpread/2.0f);
            Vector2 startVelocity = Vector2.zero;
            startVelocity.x = Mathf.Cos(Mathf.Deg2Rad * launchAngle);
            startVelocity.y = Mathf.Sin(Mathf.Deg2Rad * launchAngle);
            startVelocity *= ball.GetBallData().speed;
            
            BallBody body = ball.GetComponent<BallBody>();
            body.SetVelocity(startVelocity);
        }

        private Vector2 GetRandomPositionInRect(Rect rect)
        {
            Vector2 result = Vector2.zero;
            result.x = Random.Range(rect.x - rect.width/2.0f, rect.x + rect.width/2.0f);
            result.y = Random.Range(rect.y - rect.height/2.0f, rect.y + rect.height/2.0f);
            return result;
        }

        private void OnDrawGizmosSelected()
        {
            DrawRect(playerSpawnRect, Color.blue);
            DrawRect(enemySpawnRect, Color.red);
        }

        private void DrawRect(Rect rect, Color color)
        {
            Gizmos.color = color;
            Vector2 center = new Vector2(rect.x, rect.y);
            Vector2 size = new Vector2(rect.width, rect.height);
            Gizmos.DrawWireCube(center, size);
        }
    }
}