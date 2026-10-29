using System.Collections.Generic;
using Balltallion.Simulation.Teams;
using TMPro;
using UnityEngine;

namespace Balltallion.UI.TeamPicker
{
    public class TeamDisplayUI : MonoBehaviour
    {
        [SerializeField] private List<TeamDisplayIconUI> teamDisplayIcons;
        [SerializeField] private List<TextMeshProUGUI> numberTexts;

        [SerializeField] private bool reverseOrder = false;

        private void Awake()
        {
            if (reverseOrder)
            {
                numberTexts[0].text = "3";
                numberTexts[2].text = "1";
                
                TextMeshProUGUI temp = numberTexts[0];
                numberTexts[0] = numberTexts[2];
                numberTexts[2] = temp;
                
                TeamDisplayIconUI tempIcon = teamDisplayIcons[0];
                teamDisplayIcons[0] = teamDisplayIcons[2];
                teamDisplayIcons[2] = tempIcon;
            }
        }
        
        public void UpdateDisplay(List<BallDataSO> team)
        {
            int iconIdx = 0;
            
            foreach (TeamDisplayIconUI icon in teamDisplayIcons) icon.LoadBallData(null);
            
            foreach (BallDataSO ball in team)
            {
                TeamDisplayIconUI icon = teamDisplayIcons[iconIdx];
                icon.gameObject.SetActive(true);
                icon.LoadBallData(ball);
                iconIdx++;
            }
        }
    }
}