using System;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnPrimaryAbilityUsed : AbilityTrigger
    {
        public override AbilityTrigger Clone()
        {
            OnPrimaryAbilityUsed clone = new OnPrimaryAbilityUsed();
            return clone;
        }
    }
}