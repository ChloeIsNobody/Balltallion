using System;
using Balltallion.Input;
using Balltallion.Simulation.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    /*
     * This class allows an input-toggleable stat modifier to be defined in the inspector
     * Each toggle owns a single modifier, and toggling will add and remove the modifier on command.
     *
     * This tool expects that there is some sort of UI icon and text to display the toggle state.
     */
    
    [Serializable]
    public class DebugModifierToggle : MonoBehaviour
    {
        public Key key;
        public BattleBall.StatNames statName;
        [SerializeField] private StatModifier.OperationType operation;
        [SerializeField] private float modifierValue;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;

        private StatModifier statModifier;
        private bool isModifierApplied;
        private Stat affectedStat;

        private void OnValidate()
        {
            if (nameText) nameText.text = statName.ToString();
        }

        public void Initialize(DeviceDataSO deviceData, InputDevice inputDevice)
        {
            statModifier = new StatModifier($"DebugToggle-{key.ToString()}", modifierValue, operation);
            
            icon.sprite = deviceData.GetIconForControl(Keyboard.current[key].displayName);
            icon.color = Color.black;
            valueText.color = Color.black;
            nameText.color = Color.black;
            
            valueText.text = statModifier.GetEffectString();
        }

        public void SetAffectedStat(Stat stat)
        {
            affectedStat = stat;
            nameText.text = affectedStat?.GetName() ?? "NULL";
            if (affectedStat == null) RemoveModifier();
        }

        private void Update()
        {
            if (affectedStat == null) return;
            if (Keyboard.current[key].wasPressedThisFrame) ToggleModifier();
        }

        private void ToggleModifier()
        {
            if (isModifierApplied) RemoveModifier();
            else ApplyModifier();
        }

        private void ApplyModifier()
        {
            affectedStat?.AddModifier(statModifier);
            isModifierApplied = true;
            icon.color = Color.white;
            valueText.color = Color.white;
            nameText.color = Color.white;
        }

        private void RemoveModifier()
        {
            affectedStat?.RemoveModifier(statModifier);
            isModifierApplied = false;
            icon.color = Color.black;
            valueText.color = Color.black;
            nameText.color = Color.black;
        }
    }
}