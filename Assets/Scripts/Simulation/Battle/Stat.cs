using System;
using System.Collections.Generic;

namespace Balltallion.Simulation.Battle
{
    public class Stat
    {
        public event Action OnValueChanged;
        
        private string name;
        
        private float baseValue;
        private float flatAddedValue;
        private float percentAddedValue;
        private float multiplierValue;
        private float finalValue;

        private List<StatModifier> statModifiers = new();

        public Stat(string name, float baseValue)
        {
            this.name = name;
            this.baseValue = baseValue;
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
        
        public string GetName() => name;
        public float GetBaseValue() => baseValue;
        public float GetValue() => finalValue;
        public float GetFlatAddedValue() => flatAddedValue;
        public float GetPercentAddedValue() => percentAddedValue;
        public float GetMultiplierValue() => multiplierValue;

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
            OnValueChanged?.Invoke();
        }
    }
}