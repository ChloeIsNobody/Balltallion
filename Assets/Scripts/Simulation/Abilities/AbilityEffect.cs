using System;

namespace Balltallion.Simulation.Abilities
{
    /// <summary>
    /// Decides WHAT happens. An effect never sees triggers: the owning ability calls
    /// <see cref="Apply"/> when its trigger fires.
    /// </summary>
    [Serializable]
    public abstract class AbilityEffect
    {
        /// <summary>Do this effect's work. Act on <c>context.target</c> unless the effect says otherwise.</summary>
        public abstract void Apply(TriggerData context);
    }
}