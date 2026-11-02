using System;
using System.Collections.Generic;
using UnityEngine;

namespace Balltallion.Simulation.Battle
{
    public class Stat
    {
        public event Action<float> OnValueChanged;
        
        private string name;
        
        private float baseValue;
        private float minValue;
        private float maxValue;
        
        private float flatAddedValue;
        private float percentAddedValue;
        private float multiplierValue;
        private float finalValue;

        private List<StatModifier> statModifiers = new();
        private string displayFormat;
        private bool displaysWithPercent;

        public Stat(string name, float baseValue, string displayFormat, float minValue = 0, float maxValue = Mathf.Infinity)
        {
            this.name = name;
            this.baseValue = baseValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            
            displaysWithPercent = displayFormat[displayFormat.Length - 1] == '%';
            if (displaysWithPercent) displayFormat = displayFormat.Substring(0, displayFormat.Length - 1);
            
            RecalculateModifiers();
        }

        public void AddModifier(StatModifier modifier)
        {
            statModifiers.Add(modifier);
            RecalculateModifiers();
        }

        public void RemoveModifier(StatModifier modifier)
        {
            if (statModifiers.Remove(modifier)) RecalculateModifiers();
        }

        private void RecalculateModifiers()
        {
            flatAddedValue = 0;
            percentAddedValue = 100;
            multiplierValue = 1;
            
            foreach (StatModifier modifier in statModifiers) 
            {
                switch (modifier.GetOperation())
                {
                    case StatModifier.OperationType.FlatAdditive:
                        flatAddedValue += modifier.GetValue();
                        break;
                    case StatModifier.OperationType.PercentAdditive:
                        percentAddedValue += modifier.GetValue();
                        break;
                    case StatModifier.OperationType.Multiplicative:
                        multiplierValue *= modifier.GetValue();
                        break;
                }
            }

            finalValue = (baseValue + flatAddedValue) * percentAddedValue/100 * multiplierValue;
            finalValue = Mathf.Clamp(finalValue, minValue, maxValue);
            OnValueChanged?.Invoke(finalValue);
        }
        
        public string GetName() => name;
        public float GetBaseValue() => baseValue;
        public float GetValue() => finalValue;
        public float GetValueBaseRatio() => finalValue / baseValue;

        public string GetDisplayValue()
        {
            string result = GetValue().ToString(displayFormat);
            if (displaysWithPercent) result += "%";
            return result;
        }
        public string GetDisplayBaseValue()
        {
            string result = baseValue.ToString(displayFormat);
            if (displaysWithPercent) result += "%";
            return result;
        }
        
        public float GetFlatAddedValue() => flatAddedValue;
        public float GetPercentAddedValue() => percentAddedValue;
        public float GetMultiplierValue() => multiplierValue;
        public IReadOnlyList<StatModifier> GetModifiers() => statModifiers;
    }
}