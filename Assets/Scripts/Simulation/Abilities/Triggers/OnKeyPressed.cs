using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnKeyPressed : AbilityTrigger
    {
        [SerializeField] private Key key;
        [InfoBox("Toggle Mode: Pressing once activates the trigger, pressing again deactivates the trigger.")]
        [SerializeField] private bool isToggle = false;
        
        private bool isActivated = false;
        
        public override void Update()
        {
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                if (!isToggle)
                {
                    Trigger();
                    return;
                }
                
                if (!isActivated)
                {
                    Trigger(); 
                    isActivated = true;
                }
                else
                {
                    Untrigger();
                    isActivated = false;
                }
            }
        }
        
        public override AbilityTrigger Clone()
        {
            OnKeyPressed clone = new OnKeyPressed();
            clone.key = key;
            clone.isToggle = isToggle;
            return clone;
        }
    }
}