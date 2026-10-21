using System;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    /// <summary>
    /// One ability: a trigger and the effects it runs. This is the only class that knows
    /// about both. It binds the trigger with its own callback, and when the trigger fires
    /// it builds an <see cref="TriggerData"/> and applies each effect in order.
    ///
    /// The asset in the Project window is shared by every ball that lists it, so it is
    /// never bound directly. <see cref="AbilityRunner"/> makes a copy per ball with
    /// <see cref="CreateRuntimeCopy"/>, and only that copy is bound. Anything a trigger or
    /// effect remembers during a fight (a count, a timer) therefore belongs to one ball.
    /// </summary>
    [CreateAssetMenu(menuName = "Balltallion/Ability", fileName = "New Ability")]
    public class AbilitySO : ScriptableObject
    {
        // Most activations of this ability that can be running at once: the first one, plus effects
        // that set off the same trigger again (for example knocking the owner back on a knockback trigger).
        private const int MaxNestedFires = 4;

        [SerializeReference, SubclassSelector] private AbilityTrigger trigger;
        [SerializeReference, SubclassSelector] private AbilityEffect effect;

        // Plain private: not saved in the asset, not copied by Instantiate, but kept when
        // scripts recompile during Play mode, so AbilityRunner can bind the copy again.
        private bool isRuntimeCopy;

        [NonSerialized] private AbilityBuilder builder;
        [NonSerialized] private bool isBound;
        [NonSerialized] private int fireDepth;

        public bool IsRuntimeCopy => isRuntimeCopy;
        public bool IsBound => isBound;

        /// <summary>
        /// A per-ball copy of this asset, including its own copies of the trigger and every
        /// effect. Bind the copy, never the asset.
        /// </summary>
        public AbilitySO CreateRuntimeCopy()
        {
            AbilitySO copy = Instantiate(this);
            copy.name = name;
            copy.isRuntimeCopy = true;
            return copy;
        }

        public void Grant(AbilityBuilder abilityBuilder)
        {
            if (!isRuntimeCopy)
            {
                Debug.LogError($"Bind a copy from CreateRuntimeCopy(), not the '{name}' asset itself. " +
                               "The asset is shared by every ball that uses it.", this);
                return;
            }
            if (trigger == null)
            {
                Debug.LogWarning($"Ability '{name}' has no trigger, so it will never fire.", this);
                return;
            }

            Revoke();
            builder = abilityBuilder;
            isBound = true;
            trigger.Bind(abilityBuilder, OnTriggerFired);
        }

        public void Revoke()
        {
            if (!isBound) return;
            trigger?.Unbind();
            isBound = false;
            builder = null;
        }

        private void OnTriggerFired(TriggerEvent triggerEvent)
        {
            if (!isBound) return;
            BattleBall owner = builder ? builder.Owner : null;
            if (!owner) return;

            if (fireDepth >= MaxNestedFires)
            {
                Debug.LogWarning($"Ability '{name}' kept setting off its own trigger, so the extra activation was skipped.", this);
                return;
            }

            TriggerData context = new TriggerData
            {
                ability = this,
                builder = builder,
                owner = owner,
                target = owner,
                other = triggerEvent.other,
                point = triggerEvent.point,
            };

            fireDepth++;
            try
            {
                effect?.Apply(context);
            }
            finally
            {
                fireDepth--;
            }
        }
/*
#if UNITY_EDITOR
        private void OnValidate()
        {
            // Pressing + or "Duplicate Array Element" on a [SerializeReference] list can leave two
            // slots pointing at the same trigger or effect object. Give each slot its own copy.
            if (Balltallion.Editor.SharedReferenceFixer.SeparateSharedReferences(this))
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif */
    }
}
