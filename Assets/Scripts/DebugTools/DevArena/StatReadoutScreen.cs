using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Simulation.Battle;
using Balltallion.UI.Components.Debugging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.DebugTools.DevArena
{
    public class StatReadoutScreen : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DeviceDataSO keyboardDeviceData;
        [SerializeField] private GameObject battleManagerRef;
        [SerializeField] private List<StatModifierToggle> modifierToggles;
        [SerializeField] private List<StatReadout> statReadouts;
        
        private IBattleManager battleManager;
        private BattleBall trackedBall;

        private void Awake()
        {
            battleManager = battleManagerRef.GetComponent<IBattleManager>();
            foreach (StatModifierToggle toggle in modifierToggles)
            {
                toggle.Initialize(keyboardDeviceData, Keyboard.current);
            }
        }
        
        private void Update()
        {
            if (!trackedBall && battleManager.GetCurrentPlayerBall())
            {
                SetTrackedBall(battleManager.GetCurrentPlayerBall());
            }
        }

        private void SetTrackedBall(BattleBall ball)
        {
            trackedBall = ball;
            trackedBall.OnDeath += OnTrackedBallDied;
                
            foreach (StatModifierToggle toggle in modifierToggles)
            {
                Stat ballStat = ball.GetStat(toggle.statName);
                toggle.SetAffectedStat(ballStat);
            }

            foreach (StatReadout readout in statReadouts)
            {
                readout.SetTrackedBall(ball);
            }
        }

        private void OnTrackedBallDied()
        {
            foreach (StatModifierToggle toggle in modifierToggles)
            {
                toggle.SetAffectedStat(null);
            }

            foreach (StatReadout readout in statReadouts)
            {
                readout.SetTrackedBall(null);
            }
        }
    }
}