using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.TeamPicker
{
    public class TeamDisplayUI : MonoBehaviour
    {
        [SerializeField] private List<TeamDisplayIconUI> teamDisplayIcons;
        
        public void UpdateDisplay(List<BallDataSO> team)
        {
            foreach (TeamDisplayIconUI icon in teamDisplayIcons)
            {
                icon.gameObject.SetActive(false);
            }

            int iconIdx = 0;
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