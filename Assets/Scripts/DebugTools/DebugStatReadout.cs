using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using TMPro;
using UnityEngine;

namespace Balltallion.DebugTools
{
    public class DebugStatReadout : MonoBehaviour
    {
        [SerializeField] private ReadoutModes readoutMode;
        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI extraInfoText;
        
        private Stat trackedStat;

        private enum ReadoutModes
        {
            OnlyFinalStat,
            AllCalculationSteps,
            ModifierList,
        }

        public void SetTrackedStat(Stat stat)
        {
            trackedStat = stat;
        }

        private void Update()
        {
            if (trackedStat == null) return;

            statText.text = $"{trackedStat.GetName()}: {trackedStat.GetValue():F0}";
            
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
                extraInfoText.text = "Modifiers:";
                
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