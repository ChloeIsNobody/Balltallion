using Balltallion.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.TeamPicker
{
    public class TeamDisplayIconUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private Image icon;

        public void LoadBallData(BallDataSO ballData, int count)
        {
            nameText.text = ballData.displayName;
            icon.sprite = ballData.ballSprite;
            countText.text = $"x{count}";
        }

        public void SetCount(int newCount)
        {
            countText.text = $"x{newCount}";
        }
    }
}