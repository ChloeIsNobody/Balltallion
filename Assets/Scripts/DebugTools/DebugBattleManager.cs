using System.Collections;
using System.Collections.Generic;
using Balltallion.Input;
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
        [SerializeField] private BallDataSO enemyBall;
        
        [Header("References")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private BallSpawner ballSpawner;
        
        private BattleBall currentPlayerBall;
        private BattleBall currentEnemyBall;
        
        private void Start()
        {
            StartCoroutine(DelayedStart());
        }

        // Has to be done just because of input state setup being finicky
        private IEnumerator DelayedStart()
        {
            yield return null;
            InitializeInput();
            SpawnBalls();
        }

        private void InitializeInput()
        {
            inputManager.ActivateSplitKeyboard(Keyboard.current);
            foreach (LocalPlayerSO localPlayer in localPlayers)
            {
                localPlayer.GetInputRelay().SwitchActionMap(ActionMaps.Battle);
            }
        }

        private void SpawnBalls()
        {
            if (playerBall)
            {
                BallLoadout playerBallLoadout = new BallLoadout(playerBall);
                currentPlayerBall = ballSpawner.InstantiateBall(playerBallLoadout, localPlayers[0].GetInputRelay(), TeamNames.Player);
            }

            if (enemyBall)
            {
                BallLoadout enemyBallLoadout = new BallLoadout(enemyBall);
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