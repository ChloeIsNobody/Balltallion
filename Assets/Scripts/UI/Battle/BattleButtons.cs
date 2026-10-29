using Balltallion.Simulation.Battle;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Balltallion.UI.Battle
{
    public class BattleButtons : MonoBehaviour
    {
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button backButton;
        
        private void OnEnable()
        {
            restartButton.onClick.AddListener(Restart);
            backButton.onClick.AddListener(Return);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveAllListeners();
            backButton.onClick.RemoveAllListeners();
        }
        
        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Return();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Restart();
            }
        }

        private void Restart()
        {
            battleManager.RestartBattle();
        }

        private void Return()
        {
            battleManager.ReturnToTeamPickerScene();
        }
    }
}