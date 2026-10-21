using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    /// <summary>
    /// What a trigger saw when it fired. Triggers fill in what they know; the ability
    /// turns it into an AbilityContext for the effects.
    /// </summary>
    public struct TriggerEvent
    {
        /// <summary>Where it happened, such as a wall contact point. Zero if it doesn't apply.</summary>
        public Vector2 point;

        /// <summary>The other ball involved, such as the one that was bounced into. Null if none.</summary>
        public BattleBall other;
    }
}