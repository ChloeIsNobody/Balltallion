using System.Collections;
using Balltallion.Audio;
using Balltallion.Simulation.Teams;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Balltallion.Simulation.Battle
{
    public class BattleManager : MonoBehaviour
    { 
        [SerializeField] private BallSpawner ballSpawner;
        
        [Header("UI")]
        [HorizontalLine(color: EColor.Gray, height: 1.5f)]
        [SerializeField] private BallHealthDisplayUI playerHealthDisplay;
        [SerializeField] private BallHealthDisplayUI enemyHealthDisplay;

        public static TeamLoadout playerTeamLoadout;
        public static TeamLoadout enemyTeamLoadout;
        
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
            TeamNames winner;
            playerTeamLoadout.ResetIdx();
            enemyTeamLoadout.ResetIdx();

            while (true)
            {
                if (!currentPlayerBall && playerTeamLoadout.HasNextBall())
                {
                    currentPlayerBall = ballSpawner.InstantiateBall(playerTeamLoadout.GetNextBall(), TeamNames.Player);
                }

                if (!currentEnemyBall && enemyTeamLoadout.HasNextBall())
                {
                    currentEnemyBall = ballSpawner.InstantiateBall(enemyTeamLoadout.GetNextBall(), TeamNames.Enemy);
                }

                if (!currentPlayerBall && !playerTeamLoadout.HasNextBall()) { winner = TeamNames.Enemy; break; }
                if (!currentEnemyBall && !enemyTeamLoadout.HasNextBall()) { winner = TeamNames.Player; break; }

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