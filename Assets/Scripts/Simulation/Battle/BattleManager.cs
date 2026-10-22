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
        
        private void Awake()
        {
            currentPlayerBall = ballSpawner.InstantiateBall(playerTeamLoadout[0], TeamNames.Player);
            currentEnemyBall = ballSpawner.InstantiateBall(enemyTeamLoadout[0], TeamNames.Enemy);
            
            playerHealthDisplay.SetTrackedBall(currentPlayerBall);
            enemyHealthDisplay.SetTrackedBall(currentEnemyBall);
            
            if (battleAudioHandler != null)
                battleAudioHandler.PlayBattleMusic();
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