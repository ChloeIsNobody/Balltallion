using System;
using Balltallion.Simulation.Battle;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnPrimaryActiveUsed : AbilityTrigger
    {
        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            ball.OnPrimaryAbilityUsed += Trigger;
        }
        
        public override AbilityTrigger Clone()
        {
            OnPrimaryActiveUsed clone = new OnPrimaryActiveUsed();
            return clone;
        }
    }
}