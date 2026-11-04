using System;
using System.Collections.Generic;
using Balltallion.Simulation.Battle;
using NaughtyAttributes;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    [CreateAssetMenu(menuName = "Balltallion/Test", fileName = "ReferenceTest")]
    public class AbilityTest : ScriptableObject {

        // The type that implements ICommand will be displayed in the popup.
        [SerializeReference, SubclassSelector] ITrigger trigger;
        [SerializeReference, SubclassSelector] List<IEffect> effect;
    }

    public interface ITrigger {
        void Activate();
        void Deactivate();
    }

    public interface IEffect
    {
        void Execute();
    }

    [Serializable]
    public class OnSpawn : ITrigger
    {
        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public class OnPrimaryAbilityUsed : ITrigger
    {
        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public class OnTimer : ITrigger
    {
        [SerializeField] private float cooldown;
        
        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public class OnHealthThreshold : ITrigger
    {
        [InfoBox("Trigger activates when health is...")]
        [SerializeField] private ComparisonType comparisonType;
        [SerializeField] private float percentValue;
        [SerializeField] private bool triggersOnce;

        private bool triggerLock;

        private enum ComparisonType
        {
            LessThan,
            LessThanOrEqualTo,
            GreaterThan,
            GreaterThanOrEqualTo,
        }
        
        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class DamageSelf : IEffect
    {
        [SerializeField] private float damage;
        
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public class HealSelf : IEffect
    {
        [SerializeField] private float damage;
        
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public class KnockbackSelf : IEffect
    {
        [SerializeField] private float knockbackStrength;
        [SerializeField] private KnockbackOptions knockbackOptions;
        [SerializeField, Range(0.0f, 360.0f)] private float direction;
        [SerializeField, Range(0.0f, 360.0f)] private float randomSpread;

        private enum KnockbackOptions
        {
            Fixed,
            Random,
            TowardsOpponent,
            AwayFromOpponent,
        }
        
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class ModifyStat : IEffect
    {
        [InfoBox("Applies modifier on trigger activate. Removes on trigger deactivate.")]
        [SerializeField] private BattleBall.StatNames stat;
        
        [Header("Modifier Settings")]
        [SerializeField] private string modifierLabel;
        [SerializeField] private StatModifier.OperationType operation;
        [SerializeField] private float value;
        
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public class ModifyStatTemporary : IEffect
    {
        [InfoBox("Applies modifier on trigger activate. Removes after duration.")]
        [SerializeField] private BattleBall.StatNames stat;
        [SerializeField] private float duration;
        
        [Header("Modifier Settings")]
        [SerializeField] private string modifierLabel;
        [SerializeField] private StatModifier.OperationType operation;
        [SerializeField] private float value;
        
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}