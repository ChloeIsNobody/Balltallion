using System;
using System.Collections.Generic;
using Balltallion.Battle;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Balltallion.TeamPicker
{
    public class TeamPickerUI : MonoBehaviour
    {
        [SerializeField] private List<BallDataSO> selectableBalls;
        
        [Header("Ball Selection")]
        [HorizontalLine(color: EColor.Gray, height:1.5f)]
        [SerializeField] private Button leftSelectButton;
        [SerializeField] private Button rightSelectButton;
        [SerializeField] private Button addToPlayerTeamButton;
        [SerializeField] private Button addToEnemyTeamButton;
        [SerializeField] private TextMeshProUGUI ballNameText;
        [SerializeField] private Image ballIcon;

        [Header("Other Stuff")]
        [SerializeField] private Button playButton;

        private int selectedBallIdx = 0;
        private BallDataSO selectedBallData;

        private void Start()
        {
            SetSelectedBall(0);
        }

        private void OnEnable()
        {
            leftSelectButton.onClick.AddListener(RotateSelectionLeft);
            rightSelectButton.onClick.AddListener(RotateSelectionRight);
        }

        private void OnDisable()
        {
            leftSelectButton.onClick.RemoveListener(RotateSelectionLeft);
            rightSelectButton.onClick.RemoveListener(RotateSelectionRight);
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
        }

        private void RotateSelectionLeft()
        {
            SetSelectedBall(selectedBallIdx - 1);
        }

        private void RotateSelectionRight()
        {
            SetSelectedBall(selectedBallIdx + 1);
        }

        private void SetSelectedBall(int newIdx)
        {
            selectedBallIdx = newIdx;
            if (selectedBallIdx < 0) selectedBallIdx = selectableBalls.Count - 1;
            if (selectedBallIdx >= selectableBalls.Count) selectedBallIdx = 0;
            
            selectedBallData = selectableBalls[selectedBallIdx];
            ballNameText.text = selectedBallData.name;
            ballIcon.sprite = selectedBallData.ballSprite;
        }
    }
}