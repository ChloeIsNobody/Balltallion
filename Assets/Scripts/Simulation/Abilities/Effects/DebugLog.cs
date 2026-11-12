using System;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Effects
{
    [Serializable]
    public class DebugLog : AbilityEffect
    {
        [SerializeField] private string activateMessage = "Ability activated!";
        [SerializeField] private string deactivateMessage = "Ability deactivated!";
        
        public override void Activate()
        {
            Debug.Log(activateMessage);
        }

        public override void Deactivate()
        {
            Debug.Log(deactivateMessage);
        }

        public override AbilityEffect Clone()
        {
            DebugLog clone = new DebugLog();
            clone.activateMessage = activateMessage;
            clone.deactivateMessage = deactivateMessage;
            return clone;
        }
    }
}