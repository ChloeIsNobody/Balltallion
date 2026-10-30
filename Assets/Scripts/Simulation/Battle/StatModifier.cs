namespace Balltallion.Simulation.Battle
{
    public class StatModifier
    {
        private string name;
        private float value;
        private OperationType operation;
        public enum OperationType
        {
            FlatAdditive,
            PercentAdditive,
            Multiplicative
        }

        public StatModifier(string name, float value, OperationType operation)
        {
            this.name = name;
            this.value = value;
            this.operation = operation;
        }
        
        public string GetName() => name;
        public float GetValue() => value;
        public OperationType GetOperation() => operation;
    }
}