using System;
using System.Collections.Generic;
using Balltallion.Simulation;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Balltallion.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private BattleBall battleBallPrefab;

        [Header("Team Data")]
        [SerializeField] private loadModes loadMode;
        [SerializeField] private BallDataSO playerBallData;
        [SerializeField] private BallDataSO enemyBallData;

        private enum loadModes
        {
            Editor,
            TeamPicker
        }
        
        [Header("Battle Setup")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] private Rect playerSpawnRect;
        [SerializeField] private Rect enemySpawnRect;
        [SerializeField, Range(0.0f, 360.0f)] private float playerLaunchAngle = 0.0f;
        [SerializeField, Range(0.0f, 360.0f)] private float enemyLaunchAngle = 0.0f;
        [SerializeField, Range(0.0f, 360.0f)] private float launchAngleSpread = 0.0f;

        [Header("UI")]
        [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] private BallHealthDisplayUI playerHealthDisplay;
        [SerializeField] private BallHealthDisplayUI enemyHealthDisplay;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button backButton;

        public static List<BallDataSO> playerTeamLoadout = new();
        public static List<BallDataSO> enemyTeamLoadout = new();
        
        private List<BattleBall> playerBalls = new();
        private List<BattleBall> enemyBalls = new();
        
        private void Awake()
        {
            if (loadMode == loadModes.Editor)
            {
                playerTeamLoadout.Clear();
                playerTeamLoadout.Add(playerBallData);
                enemyTeamLoadout.Clear();
                enemyTeamLoadout.Add(enemyBallData);
            }
            
            InstantiateBalls();
            playerHealthDisplay.SetTrackedBall(playerBalls[0]);
            enemyHealthDisplay.SetTrackedBall(enemyBalls[0]);
        }

        private void OnEnable()
        {
            replayButton.onClick.AddListener(ReplayBattle);
            backButton.onClick.AddListener(ReturnToTeamPickerScene);
        }

        private void OnDisable()
        {
            replayButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        private void InstantiateBalls()
        {
            foreach (BallDataSO ballData in playerTeamLoadout)
            {
                InstantiateBall(ballData, TeamNames.Player);
            }

            foreach (BallDataSO ballData in enemyTeamLoadout)
            {
                InstantiateBall(ballData, TeamNames.Enemy);
            }
        }

        private void InstantiateBall(BallDataSO ballData, TeamNames team)
        {
            BattleBall ball = Instantiate(battleBallPrefab);
            ball.LoadBallData(ballData, team);

            Rect spawnRect = team == TeamNames.Player ? playerSpawnRect : enemySpawnRect;
            ball.transform.position = GetRandomPositionInRect(spawnRect);
            
            float launchAngle = team == TeamNames.Player ? playerLaunchAngle : enemyLaunchAngle;
            launchAngle += Random.Range(-launchAngleSpread/2.0f, launchAngleSpread/2.0f);
            Vector2 startVelocity = Vector2.zero;
            startVelocity.x = Mathf.Cos(Mathf.Deg2Rad * launchAngle);
            startVelocity.y = Mathf.Sin(Mathf.Deg2Rad * launchAngle);
            startVelocity *= ballData.speed;
            
            BallBody body = ball.GetComponent<BallBody>();
            body.SetVelocity(startVelocity);

            List<BattleBall> teamList = team == TeamNames.Player ? playerBalls : enemyBalls;
            teamList.Add(ball);
        }

        private void ReplayBattle()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ReturnToTeamPickerScene()
        {
            SceneManager.LoadScene("TeamPicker");
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