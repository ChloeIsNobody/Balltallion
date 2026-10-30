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
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private DeviceDataSO deviceData;
        
        [SerializeField] private string statName;
        [SerializeField] private float baseValue;
        [SerializeField] private float minValue = 0;
        [SerializeField] private float maxValue = 200;
        
        [SerializeField] private List<DebugModifierToggle> modifierDebugSlots;
        private Stat stat;
        
        [Serializable]
        private class DebugModifierToggle
        {
            public Key key;
            [SerializeField] private StatModifier.OperationType operation;
            [SerializeField] private float modifierValue;
            [SerializeField] private Image icon;
            [SerializeField] private TextMeshProUGUI text;

            private StatModifier statModifier;
            private bool isModifierApplied;

            public void SetupIcon(Sprite sprite)
            {
                icon.sprite = sprite;
                icon.color = Color.black;
                text.color = Color.black;

                text.text = operation switch
                {
                    StatModifier.OperationType.FlatAdditive => $"+{modifierValue}",
                    StatModifier.OperationType.PercentAdditive => $"+{modifierValue}%",
                    StatModifier.OperationType.Multiplicative => $"{modifierValue:F2}x",
                    _ => "???"
                };
            }

            public void ToggleModifier(Stat stat)
            {
                if (isModifierApplied) RemoveModifier(stat);
                else ApplyModifier(stat);
            }

            private void ApplyModifier(Stat stat)
            {
                statModifier = new StatModifier(key.ToString(), modifierValue, operation);
                stat.AddModifier(statModifier);
                isModifierApplied = true;
                icon.color = Color.white;
                text.color = Color.white;
            }

            private void RemoveModifier(Stat stat)
            {
                stat.RemoveModifier(statModifier);
                isModifierApplied = false;
                icon.color = Color.black;
                text.color = Color.black;
            }
        }
        
        private void Start()
        {
            stat = new Stat(statName, baseValue, minValue, maxValue);
            
            foreach (DebugModifierToggle slot in modifierDebugSlots)
            {
                Sprite iconSprite = deviceData.GetIconForControl(Keyboard.current[slot.key].displayName);
                slot.SetupIcon(iconSprite);
            }
        }

        private void Update()
        {
            foreach (DebugModifierToggle slot in modifierDebugSlots)
            {
                if (Keyboard.current[slot.key].wasPressedThisFrame)
                {
                    slot.ToggleModifier(stat);
                }
            }
            
            debugText.text = $"Base Value: {stat.GetBaseValue():F0}";
            debugText.text += $"\nFlat Addition: {stat.GetFlatAddedValue():F0}";
            debugText.text += $"\nPercent Multiplier: {stat.GetPercentAddedValue():F2}%";
            debugText.text += $"\nMultiplier: {stat.GetMultiplierValue():F2}x";
            debugText.text += $"\n\n{stat.GetName()}: {stat.GetValue():F0}";
        }
    }
}