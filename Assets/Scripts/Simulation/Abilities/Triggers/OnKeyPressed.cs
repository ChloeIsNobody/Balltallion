using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnKeyPressed : AbilityTrigger
    {
        [InfoBox("Pressing once activates the trigger, pressing again deactivates the trigger.")]
        [SerializeField] private Key key;
        
        public override AbilityTrigger Clone()
        {
            OnKeyPressed clone = new OnKeyPressed();
            clone.key = key;
            return clone;
        }
    }
}