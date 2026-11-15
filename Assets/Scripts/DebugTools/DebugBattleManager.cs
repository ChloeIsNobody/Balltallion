using System.Collections;
using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Simulation.Abilities;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Balltallion.DebugTools
{
    public class DebugBattleManager : MonoBehaviour, IBattleManager
    {
        [SerializeField] private BallDataSO playerBall;
        [SerializeField] private List<EquipableDataSO> playerEquipables;
        [SerializeField] private BallDataSO enemyBall;
        [SerializeField] private List<EquipableDataSO> enemyEquipables;
        
        [Header("References")]
        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private BallSpawner ballSpawner;
        
        private BattleBall currentPlayerBall;
        private BattleBall currentEnemyBall;
        
        private void Start()
        {
            SpawnBalls();
        }

        private void SpawnBalls()
        {
            if (playerBall)
            {
                BallLoadout playerBallLoadout = new BallLoadout(playerBall, playerEquipables);
                currentPlayerBall = ballSpawner.InstantiateBall(playerBallLoadout, localPlayers[0].GetInputRelay(), TeamNames.Player);
            }

            if (enemyBall)
            {
                BallLoadout enemyBallLoadout = new BallLoadout(enemyBall, enemyEquipables);
                currentEnemyBall = ballSpawner.InstantiateBall(enemyBallLoadout, localPlayers[1].GetInputRelay(), TeamNames.Enemy);
            }
        }

        public BattleBall GetCurrentPlayerBall() => currentPlayerBall;
        public BattleBall GetCurrentEnemyBall() => currentEnemyBall;
        public void RestartBattle()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}