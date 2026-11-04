using System;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnSpawn : AbilityTrigger
    {
        public override AbilityTrigger Clone()
        {
            OnSpawn clone = new OnSpawn();
            return clone;
        }
    }
}