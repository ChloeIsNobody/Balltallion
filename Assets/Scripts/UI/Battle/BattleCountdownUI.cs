using System.Collections;
using TMPro;
using UnityEngine;

namespace Balltallion.UI.Battle
{
    public class BattleCountdownUI : MonoBehaviour
    {
        [SerializeField] private int countdownDuration;
        [SerializeField] private TextMeshProUGUI countdownText;
        
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }
        
        public IEnumerator CountdownCoroutine()
        {
            Show();
            
            int countdownTime = countdownDuration;
            while (countdownTime > 0)
            {
                countdownText.text = countdownTime.ToString();
                yield return new WaitForSeconds(1);
                countdownTime--;
            }
            
            Hide();
        }
        
        public void Show() => canvasGroup.alpha = 1;
        public void Hide() => canvasGroup.alpha = 0;
    }
}