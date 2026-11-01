using System;
using System.Collections.Generic;
using System.Linq;
using Balltallion.Input;
using Balltallion.Simulation.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.DebugTools
{
    public class DevArenaDebugPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DeviceDataSO keyboardDeviceData;
        [SerializeField] private GameObject battleManagerRef;
        [SerializeField] private List<DebugModifierToggle> modifierToggles;
        [SerializeField] private List<DebugStatReadout> statReadouts;
        
        private IBattleManager battleManager;
        private BattleBall trackedBall;

        private void Awake()
        {
            battleManager = battleManagerRef.GetComponent<IBattleManager>();
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
            foreach (DebugModifierToggle toggle in modifierToggles)
            {
                Stat ballStat = ball.GetStat(toggle.statName);
                toggle.Initialize(ballStat, keyboardDeviceData, Keyboard.current);
            }

            foreach (DebugStatReadout readout in statReadouts)
            {
                readout.SetTrackedBall(ball);
            }
        }
    }
}