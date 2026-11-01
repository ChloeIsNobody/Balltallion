using System;
using System.Collections.Generic;
using Balltallion.Input;
using Balltallion.Simulation.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    public class StatModifierDebugger : MonoBehaviour
    {
        [SerializeField] private DebugStatReadout debugStatReadout;
        [SerializeField] private DeviceDataSO deviceData;
        
        [SerializeField] private string statName;
        [SerializeField] private float baseValue;
        [SerializeField] private float minValue = 0;
        [SerializeField] private float maxValue = 200;
        
        [SerializeField] private List<DebugModifierToggle> modifierDebugSlots;
        private Stat stat;
        
        private void Start()
        {
            stat = new Stat(statName, baseValue, "F0", minValue, maxValue);
            debugStatReadout.SetTrackedStat(stat);
            
            foreach (DebugModifierToggle slot in modifierDebugSlots)
            {
                slot.Initialize(stat, deviceData, Keyboard.current);
            }
        }
    }
}