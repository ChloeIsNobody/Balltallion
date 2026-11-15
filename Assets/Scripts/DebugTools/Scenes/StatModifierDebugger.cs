using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Simulation.Battle;
using Balltallion.UI.Components.Debugging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.DebugTools.Scenes
{
    public class StatModifierDebugger : MonoBehaviour
    {
        [SerializeField] private StatReadout statReadout;
        [SerializeField] private DeviceDataSO deviceData;
        
        [SerializeField] private string statName;
        [SerializeField] private float baseValue;
        [SerializeField] private float minValue = 0;
        [SerializeField] private float maxValue = 200;
        
        [SerializeField] private List<StatModifierToggle> modifierDebugSlots;
        private Stat stat;
        
        private void Start()
        {
            stat = new Stat(statName, baseValue, "F0", minValue, maxValue);
            statReadout.SetTrackedStat(stat);
            
            foreach (StatModifierToggle slot in modifierDebugSlots)
            {
                slot.Initialize(deviceData, Keyboard.current);
                slot.SetAffectedStat(stat);
            }
        }
    }
}