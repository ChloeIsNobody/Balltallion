using System;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using TMPro;
using UnityEngine;

namespace Balltallion.DebugTools
{
    public class DebugStatReadout : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ReadoutModes readoutMode;
        
        [Header("References")]
        [SerializeField] private TMP_Dropdown statDropdown;
        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI extraInfoText;

        private BattleBall trackedBall;
        private Stat trackedStat;
        private BattleBall.StatNames dropdownSelectedStat;

        private enum ReadoutModes
        {
            OnlyFinalStat,
            AllCalculationSteps,
            ModifierList,
        }

        private void Awake()
        {
            if (statDropdown) InitializeStatDropdown();
        }

        private void InitializeStatDropdown()
        {
            List<string> statNames = new List<string>(Enum.GetNames(typeof(BattleBall.StatNames)));
            statDropdown.ClearOptions();
            statDropdown.AddOptions(statNames);
        }
        
        private void OnEnable()
        {
            statDropdown?.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDisable()
        {
            statDropdown?.onValueChanged.RemoveAllListeners();
        }
        
        private void OnDropdownValueChanged(int selectedIndex)
        {
            dropdownSelectedStat = (BattleBall.StatNames)selectedIndex;
            if (trackedBall) trackedStat = trackedBall.GetStat(dropdownSelectedStat);
        }

        public void SetTrackedBall(BattleBall ball)
        {
            trackedBall = ball;
            trackedStat = trackedBall?.GetStat(dropdownSelectedStat);
        }
        
        public void SetTrackedStat(Stat stat)
        {
            trackedStat = stat;
        }

        private void Update()
        {
            if (trackedStat == null)
            {
                statText.text = "NULL";
                extraInfoText.enabled = false;
                return;
            }

            statText.text = $"{trackedStat.GetName()}: {trackedStat.GetDisplayValue()}";
            
            extraInfoText.enabled = readoutMode != ReadoutModes.OnlyFinalStat;

            if (readoutMode == ReadoutModes.AllCalculationSteps)
            {
                extraInfoText.text = $"Base Value: {trackedStat.GetBaseValue():F0}";
                extraInfoText.text += $"\nFlat Addition: {trackedStat.GetFlatAddedValue():F0}";
                extraInfoText.text += $"\nPercent Multiplier: {trackedStat.GetPercentAddedValue():F2}%";
                extraInfoText.text += $"\nMultiplier: {trackedStat.GetMultiplierValue():F2}x";
            }

            if (readoutMode == ReadoutModes.ModifierList)
            {
                extraInfoText.text = $"Base Value: {trackedStat.GetBaseValue():F0}";
                extraInfoText.text += "\nModifiers:";
                
                IReadOnlyList<StatModifier> modifiers = trackedStat.GetModifiers();
                foreach (StatModifier modifier in modifiers)
                {
                    string modifierString = "\n\t";
                    modifierString += modifier.GetName() + " / ";
                    modifierString += modifier.GetEffectString();
                    extraInfoText.text += modifierString;
                }
            }
        }
    }
}