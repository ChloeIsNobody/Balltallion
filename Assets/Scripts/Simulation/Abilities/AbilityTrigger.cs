using System;
using System.Collections;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    /// <summary>
    /// Decides WHEN an ability fires. A trigger listens to one ball's events and calls
    /// <see cref="Fire"/>. It never sees effects: the ability that owns it hands it a callback
    /// when it binds, and that callback is the only thing <see cref="Fire"/> reaches.
    /// </summary>
    [Serializable]
    public abstract class AbilityTrigger
    {
        [NonSerialized] private AbilityBuilder builder;
        [NonSerialized] private Action<TriggerEvent> onFired;
        [NonSerialized] private bool isBound;

        /// <summary>The ball this trigger belongs to. Null when not bound.</summary>
        protected BattleBall Owner => builder ? builder.Owner : null;

        public bool IsBound => isBound;

        /// <summary>
        /// Starts listening. Only <see cref="AbilitySO"/> calls this, on its runtime copy,
        /// passing the method that runs its effects.
        /// </summary>
        internal void Bind(AbilityBuilder abilityBuilder, Action<TriggerEvent> fired)
        {
            if (isBound) Unbind();
            builder = abilityBuilder;
            onFired = fired;
            isBound = true;
            OnGrant();
        }

        /// <summary>Stops listening. Safe to call more than once.</summary>
        internal void Unbind()
        {
            if (!isBound) return;
            OnRevoke(); // runs while Owner is still available
            isBound = false;
            onFired = null;
            builder = null;
        }

        /// <summary>Subscribe to the events this trigger cares about. <see cref="Owner"/> is set.</summary>
        protected abstract void OnGrant();

        /// <summary>Unsubscribe from everything <see cref="OnGrant"/> subscribed to, and stop any coroutine it started.</summary>
        protected abstract void OnRevoke();

        /// <summary>Call this when the trigger's condition happens.</summary>
        protected void Fire(TriggerEvent triggerEvent)
        {
            if (!isBound) return;
            onFired?.Invoke(triggerEvent);
        }

        /// <summary>Runs a coroutine on the ball (for timer triggers such as Over time).</summary>
        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            return builder ? builder.StartCoroutine(routine) : null;
        }

        protected void StopCoroutine(Coroutine coroutine)
        {
            if (builder && coroutine != null) builder.StopCoroutine(coroutine);
        }
    }
}
