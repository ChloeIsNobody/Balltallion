using System.Collections.Generic;
using Balltallion.Battle;
using UnityEngine;

namespace Balltallion.TeamPicker
{
    public class TeamDisplayUI : MonoBehaviour
    {
        [SerializeField] private List<TeamDisplayIconUI> teamDisplayIcons;
        private Dictionary<BallDataSO, int> ballCounts;
        
        public void UpdateDisplay(List<BallDataSO> team)
        {
            ballCounts = new Dictionary<BallDataSO, int>();
            foreach (var ballData in team)
            {
                ballCounts[ballData] = ballCounts.ContainsKey(ballData) ? ballCounts[ballData] + 1 : 1;
            }

            foreach (TeamDisplayIconUI icon in teamDisplayIcons)
            {
                icon.gameObject.SetActive(false);
            }

            int iconIdx = 0;
            foreach (KeyValuePair<BallDataSO, int> entry in ballCounts)
            {
                if (iconIdx >= teamDisplayIcons.Count) break;
                TeamDisplayIconUI icon = teamDisplayIcons[iconIdx];
                icon.gameObject.SetActive(true);
                icon.LoadBallData(entry.Key, entry.Value);
                iconIdx++;
            }
        }
    }
}