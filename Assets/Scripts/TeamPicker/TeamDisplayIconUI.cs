using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.TeamPicker
{
    public class TeamDisplayIconUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image icon;

        private void Awake()
        {
            LoadBallData(null);
        }

        public void LoadBallData(BallDataSO ballData)
        {
            nameText.text = ballData ? ballData.displayName : "";
            icon.sprite = ballData ? ballData.ballSprite : null;
            icon.color = ballData ? Color.white : Color.clear;
        }
    }
}