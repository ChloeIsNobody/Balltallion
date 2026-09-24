using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion
{
    public class BallHealthDisplayUI : MonoBehaviour
    {
        [SerializeField] private BattleBall trackedBall;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image iconImage;

        private void Update()
        {
            if (trackedBall)
            {
                nameText.text = trackedBall.name;
                iconImage.sprite = trackedBall.GetSprite();
                float healthRatio = trackedBall.GetHealth() / trackedBall.GetMaxHealth();
                healthText.text = $"HP: {trackedBall.GetHealth()} / {trackedBall.GetMaxHealth()}";
                healthSlider.value = healthRatio;
            }
            else
            {
                healthText.text = "HP: 0 / 0";
                healthSlider.value = 0;
            }
        }
    }
}