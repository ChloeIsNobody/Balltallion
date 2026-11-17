using System;
using Balltallion.Simulation.Battle;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnDeath : AbilityTrigger
    {
        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            ball.OnDeath += Trigger;
        }
        
        public override AbilityTrigger Clone()
        {
            OnDeath clone = new OnDeath();
            return clone;
        }
    }
}