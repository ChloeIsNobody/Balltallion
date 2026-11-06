using System;
using Balltallion.Simulation.Battle;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnSpawn : AbilityTrigger
    {
        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            Trigger();
        }

        public override AbilityTrigger Clone()
        {
            OnSpawn clone = new OnSpawn();
            return clone;
        }
    }
}