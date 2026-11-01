using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balltallion.UI.Battle
{
    public class BallHealthDisplayUI : MonoBehaviour
    {
        [SerializeField] private TeamNames trackedTeam;
        
        [Header("References")]
        [SerializeField] private GameObject battleManagerRef;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image iconImage;
        
        private BattleBall trackedBall;
        private IBattleManager battleManager;
        private CanvasGroup canvasGroup;

        private void Start()
        {
            battleManager = battleManagerRef.GetComponent<IBattleManager>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        private void Update()
        {
            canvasGroup.alpha = trackedBall ? 1 : 0;
            if (!trackedBall) SearchForBall();
            
            if (trackedBall)
            {
                nameText.text = trackedBall.name;
                iconImage.sprite = trackedBall.GetSprite();
                float healthRatio = trackedBall.GetHealth() / trackedBall.GetStat(BattleBall.StatNames.MaxHealth);
                healthText.text = $"HP: {trackedBall.GetHealth()} / {trackedBall.GetStat(BattleBall.StatNames.MaxHealth)}";
                healthSlider.value = healthRatio;
            }
        }

        private void SearchForBall()
        {
            trackedBall = trackedTeam switch
            {
                TeamNames.Player => battleManager.GetCurrentPlayerBall(),
                TeamNames.Enemy => battleManager.GetCurrentEnemyBall(),
                _ => trackedBall
            };
        }
    }
}