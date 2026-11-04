using System;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnTimer : AbilityTrigger
    {
        [SerializeField] private float cooldown;
        
        public override AbilityTrigger Clone()
        {
            OnTimer clone = new OnTimer();
            clone.cooldown = cooldown;
            return clone;
        }
    }
}