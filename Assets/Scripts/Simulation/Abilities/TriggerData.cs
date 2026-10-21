using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    /// <summary>
    /// Everything an effect needs to know about one activation. Built by the ability
    /// each time its trigger fires.
    /// </summary>
    public class TriggerData
    {
        /// <summary>The runtime copy of the ability that fired.</summary>
        public AbilitySO ability;

        /// <summary>The owner's runner. Use it for coroutines (Delay) and per-ball bookkeeping.</summary>
        public AbilityBuilder builder;

        /// <summary>The ball that has the ability.</summary>
        public BattleBall owner;

        /// <summary>The ball effects act on. Starts as the owner; meta effects like "Apply to opponent" change it.</summary>
        public BattleBall target;

        /// <summary>The other ball involved in the trigger, if any.</summary>
        public BattleBall other;

        /// <summary>Where the trigger happened, if it has a position.</summary>
        public Vector2 point;

        /// <summary>A copy of this context aimed at a different ball. The original is left unchanged.</summary>
        public TriggerData WithTarget(BattleBall newTarget)
        {
            TriggerData copy = (TriggerData)MemberwiseClone();
            copy.target = newTarget;
            return copy;
        }
    }
}