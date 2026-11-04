using System;

namespace Balltallion.Simulation.Abilities
{
    [Serializable]
    public abstract class AbilityEffect
    {
        public abstract void Activate();
        public abstract void Deactivate();
    }
}