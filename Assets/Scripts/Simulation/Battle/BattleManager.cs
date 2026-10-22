using System.Collections;
using System.Collections.Generic;
using Balltallion.Simulation;
using Balltallion.Audio;
using Balltallion.Simulation.Physics;
using Balltallion.TeamPicker;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Balltallion.Simulation.Battle
{
    public class BattleManager : MonoBehaviour
    { 
        [SerializeField] private BallSpawner ballSpawner;
        
        [Header("UI")]
        [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] private BallHealthDisplayUI playerHealthDisplay;
        [SerializeField] private BallHealthDisplayUI enemyHealthDisplay;

        public static List<BallLoadout> playerTeamLoadout;
        public static List<BallLoadout> enemyTeamLoadout;
        
        private BattleBall currentPlayerBall;
        private BattleBall currentEnemyBall;
        
        [Header("Audio")]
        [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] private BattleAudioHandler battleAudioHandler;
        
        private Coroutine currentState;
        
        private void Awake()
        {
            playerHealthDisplay.SetTrackedBall(currentPlayerBall);
            enemyHealthDisplay.SetTrackedBall(currentEnemyBall);
            
            if (battleAudioHandler != null)
                battleAudioHandler.PlayBattleMusic();
        }

        private void Start()
        {
            StartCoroutine(BattleState());
        }

        private IEnumerator BattleState()
        {
            int playerBallIdx = 0;
            int enemyBallIdx = 0;
            TeamNames winner;

            while (true)
            {
                if (!currentPlayerBall)
                {
                    if (playerBallIdx < playerTeamLoadout.Count)
                    {
                        currentPlayerBall = ballSpawner.InstantiateBall(playerTeamLoadout[playerBallIdx++], TeamNames.Player);
                    }
                    else
                    {
                        winner = TeamNames.Enemy;
                        break;
                    }
                }

                if (!currentEnemyBall)
                {
                    if (enemyBallIdx < enemyTeamLoadout.Count)
                    {
                        currentEnemyBall = ballSpawner.InstantiateBall(enemyTeamLoadout[enemyBallIdx++], TeamNames.Enemy);
                    }
                    else
                    {
                        winner = TeamNames.Player;
                        break;
                    }
                }

                yield return null;
            }
            
            Debug.Log($"Battle over! {winner} won!");
        }

        public void RestartBattle()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ReturnToTeamPickerScene()
        {
            if (battleAudioHandler) battleAudioHandler.StopBattleMusic();
            SceneManager.LoadScene("TeamPicker");
        }
    }
}