using System;
using System.Collections.Generic;
using Balltallion.Battle;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Balltallion.TeamPicker
{
    public class TeamPickerUI : MonoBehaviour
    {
        [SerializeField] private List<BallDataSO> selectableBalls;
        [SerializeField] private int teamLimit = 2;
        
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

        private int selectedBallIdx = 0;
        private BallDataSO selectedBallData;
        private List<BallDataSO> playerTeam = new();
        private List<BallDataSO> enemyTeam = new();

        private void Start()
        {
            SetSelectedBall(0);
            playerTeamDisplay.UpdateDisplay(playerTeam);
            enemyTeamDisplay.UpdateDisplay(enemyTeam);
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
        }

        private void Update()
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                RotateSelectionLeft();
            }
            
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                RotateSelectionRight();
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                AddSelectedToPlayerTeam();
            }

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                AddSelectedToEnemyTeam();
            }
            
            playButton.interactable = CanStartBattle();
            if (Keyboard.current.enterKey.wasPressedThisFrame && CanStartBattle())
            {
                StartBattle();
            }
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
            if (playerTeam.Count >= teamLimit) return;
            playerTeam.Add(selectedBallData);
            playerTeamDisplay.UpdateDisplay(playerTeam);
        }

        private void AddSelectedToEnemyTeam()
        {
            if (enemyTeam.Count >= teamLimit) return;
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
            BattleManager.playerTeamLoadout = new List<BallDataSO>(playerTeam);
            BattleManager.enemyTeamLoadout = new List<BallDataSO>(enemyTeam);
            SceneManager.LoadScene("Arena");
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