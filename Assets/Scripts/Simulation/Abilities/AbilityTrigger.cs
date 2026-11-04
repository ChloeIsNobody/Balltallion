using System;

namespace Balltallion.Simulation.Abilities
{
    [Serializable]
    public abstract class AbilityTrigger
    {
        public event Action OnTrigger;
        public event Action OnUntrigger;
    }
}
