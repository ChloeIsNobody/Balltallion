using Balltallion.Simulation.Battle;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    public class DebugBattleButtons : MonoBehaviour
    {
        [SerializeField] private GameObject battleManagerRef;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button backButton;

        private IBattleManager battleManager;
        
        private void OnEnable()
        {
            if (restartButton) restartButton.onClick.AddListener(Restart);
            if (backButton) backButton.onClick.AddListener(Return);
        }

        private void OnDisable()
        {
            if (restartButton) restartButton.onClick.RemoveAllListeners();
            if (backButton) backButton.onClick.RemoveAllListeners();
        }

        private void Awake()
        {
            battleManager = battleManagerRef.GetComponent<IBattleManager>();
        }

        private void Restart()
        {
            battleManager.RestartBattle();
        }

        private void Return()
        {
            SceneManager.LoadScene("TeamPicker");
        }
    }
}