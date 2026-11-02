using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Teams;
using TMPro;
using UnityEngine;

namespace Balltallion.UI.Battle
{
    public class BattleResultUI : MonoBehaviour
    {
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private TextMeshProUGUI resultText;

        private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }

        private void OnEnable()
        {
             battleManager.OnBattleEnded += ShowResult;
        }

        private void OnDisable()
        {
            battleManager.OnBattleEnded -= ShowResult;   
        }

        private void ShowResult(TeamNames team)
        {
            Show();
            if (team == TeamNames.Player) resultText.text = "Player 1 Wins!";
            if (team == TeamNames.Enemy) resultText.text = "Player 2 Wins!";
        }
        
        public void Show() => canvasGroup.alpha = 1;
        public void Hide() => canvasGroup.alpha = 0;
    }
}