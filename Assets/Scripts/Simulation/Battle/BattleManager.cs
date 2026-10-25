using System;
using System.Collections;
using Balltallion.Audio;
using Balltallion.Simulation.Teams;
using Balltallion.UI.Battle;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Balltallion.Simulation.Battle
{
    public class BattleManager : MonoBehaviour
    {
        public event Action<TeamNames> OnBattleEnded; 
        
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private BattleCountdownUI countdownUI;

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
            if (battleAudioHandler != null)
                battleAudioHandler.PlayBattleMusic();
        }

        private void Start()
        {
            if (playerTeamLoadout == null) Debug.LogWarning("Player team loadout not set!");
            if (enemyTeamLoadout == null) Debug.LogWarning("Enemy team loadout not set!");
            if (playerTeamLoadout != null && enemyTeamLoadout != null)
            {
                StartCoroutine(BattleState());
            }
        }

        private IEnumerator BattleState()
        {
            TeamNames winner;
            playerTeamLoadout.ResetIdx();
            enemyTeamLoadout.ResetIdx();

            yield return countdownUI.CountdownCoroutine();

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
            OnBattleEnded?.Invoke(winner);
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
        
        public BattleBall GetCurrentPlayerBall() => currentPlayerBall;
        public BattleBall GetCurrentEnemyBall() => currentEnemyBall;
    }
}