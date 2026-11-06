using UnityEngine;

namespace Balltallion.Simulation.Battle
{
    public class StatModifier
    {
        private string label;
        private float value;
        private OperationType operation;
        public enum OperationType
        {
            FlatAdditive,
            PercentAdditive,
            Multiplicative
        }

        public StatModifier(string label, float value, OperationType operation)
        {
            this.label = label;
            this.value = value;
            this.operation = operation;
        }
        
        public string GetLabel() => label;
        public float GetValue() => value;
        public OperationType GetOperation() => operation;

        public string GetEffectString()
        {
            string sign = Mathf.Approximately(Mathf.Sign(value), -1) ? "-" : "+";
            float absVal = Mathf.Abs(value);
            
            return operation switch
            {
                OperationType.FlatAdditive => $"{sign}{absVal}",
                OperationType.PercentAdditive => $"{sign}{absVal}%",
                OperationType.Multiplicative => $"{absVal:F2}x",
                _ => "???"
            };
        }
    }
}