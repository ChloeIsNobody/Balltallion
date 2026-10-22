using Balltallion.Simulation.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.TeamPicker
{
    public class TeamDisplayIconUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image icon;

        public void LoadBallData(BallDataSO ballData)
        {
            nameText.text = ballData.displayName;
            icon.sprite = ballData.ballSprite;
        }
    }
}