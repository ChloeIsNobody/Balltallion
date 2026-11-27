using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Players;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Balltallion.UI.TeamPicker
{
    public class TeamPickerUI : MonoBehaviour
    {
        [SerializeField, Scene] private string previousScene;
        [SerializeField, Scene] private string nextScene;
        
        [SerializeField] private List<LocalPlayerSO> localPlayers;
        [SerializeField] private List<BallDataSO> selectableBalls;
        
        [Header("Ball Selection")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] private Button leftSelectButton;
        [SerializeField] private Button rightSelectButton;
        [SerializeField] private Button addToPlayerTeamButton;
        [SerializeField] private Button addToEnemyTeamButton;
        [SerializeField] private Button clearPlayerTeamButton;
        [SerializeField] private Button clearEnemyTeamButton;
        [SerializeField] private TextMeshProUGUI ballNameText;
        [SerializeField] private Image ballIcon;
        
        [Header("Team Displays")]
        [SerializeField] private TeamDisplayUI playerTeamDisplay;
        [SerializeField] private TeamDisplayUI enemyTeamDisplay;

        [Header("Other Stuff")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button backButton;

        private int selectedBallIdx = 0;
        private BallDataSO selectedBallData;
        private List<BallDataSO> playerTeam = new();
        private List<BallDataSO> enemyTeam = new();

        private void Start()
        {
            SetSelectedBall(0);
            playerTeamDisplay.UpdateDisplay(playerTeam);
            enemyTeamDisplay.UpdateDisplay(enemyTeam);
            InputManager.Instance?.SetPlayerActionMaps(ActionMaps.Navigation);
        }

        private void OnEnable()
        {
            leftSelectButton.onClick.AddListener(RotateSelectionLeft);
            rightSelectButton.onClick.AddListener(RotateSelectionRight);
            addToPlayerTeamButton.onClick.AddListener(AddSelectedToPlayerTeam);
            addToEnemyTeamButton.onClick.AddListener(AddSelectedToEnemyTeam);
            clearPlayerTeamButton.onClick.AddListener(ClearPlayerTeam);
            clearEnemyTeamButton.onClick.AddListener(ClearEnemyTeam);
            playButton.onClick.AddListener(StartBattle);
            backButton.onClick.AddListener(Back);
        }

        private void OnDisable()
        {
            leftSelectButton.onClick.RemoveAllListeners();
            rightSelectButton.onClick.RemoveAllListeners();
            addToPlayerTeamButton.onClick.RemoveAllListeners();
            addToEnemyTeamButton.onClick.RemoveAllListeners();
            clearPlayerTeamButton.onClick.RemoveAllListeners();
            clearEnemyTeamButton.onClick.RemoveAllListeners();
            playButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            playButton.interactable = CanStartBattle();
        }

        private void RotateSelectionLeft()
        {
            SetSelectedBall(selectedBallIdx - 1);
        }

        private void RotateSelectionRight()
        {
            SetSelectedBall(selectedBallIdx + 1);
        }

        private void AddSelectedToPlayerTeam()
        {
            if (playerTeam.Count >= 3) return;
            playerTeam.Add(selectedBallData);
            playerTeamDisplay.UpdateDisplay(playerTeam);
        }

        private void AddSelectedToEnemyTeam()
        {
            if (enemyTeam.Count >= 3) return;
            enemyTeam.Add(selectedBallData);
            enemyTeamDisplay.UpdateDisplay(enemyTeam);
        }

        private void ClearPlayerTeam()
        {
            playerTeam.Clear();
            playerTeamDisplay.UpdateDisplay(playerTeam);
        }

        private void ClearEnemyTeam()
        {
            enemyTeam.Clear();
            enemyTeamDisplay.UpdateDisplay(enemyTeam);
        }

        private bool CanStartBattle()
        {
            return playerTeam.Count > 0 && enemyTeam.Count > 0;
        }
        
        private void StartBattle()
        {
            int ballIndex = 0;
            BattleManager.playerTeamLoadout = new PlayerTeamLoadout();
            foreach (BallDataSO ball in playerTeam)
            {
                BallLoadout loadout = BattleManager.playerTeamLoadout.GetBallLoadout(ballIndex++);
                loadout.SetBall(ball);
            }

            ballIndex = 0;
            BattleManager.enemyTeamLoadout = new PlayerTeamLoadout();
            foreach (BallDataSO ball in enemyTeam)
            {
                BallLoadout loadout = BattleManager.enemyTeamLoadout.GetBallLoadout(ballIndex++);
                loadout.SetBall(ball);
            }
            
            SceneManager.LoadScene(nextScene);
        }

        private void Back()
        {
            SceneManager.LoadScene(previousScene);
        }

        private void SetSelectedBall(int newIdx)
        {
            selectedBallIdx = newIdx;
            if (selectedBallIdx < 0) selectedBallIdx = selectableBalls.Count - 1;
            if (selectedBallIdx >= selectableBalls.Count) selectedBallIdx = 0;
            
            selectedBallData = selectableBalls[selectedBallIdx];
            ballNameText.text = selectedBallData.displayName;
            ballIcon.sprite = selectedBallData.ballSprite;
        }
    }
}