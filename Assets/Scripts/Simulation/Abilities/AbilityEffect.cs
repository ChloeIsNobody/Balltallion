using System;
using Balltallion.Simulation.Battle;

namespace Balltallion.Simulation.Abilities
{
    [Serializable]
    public abstract class AbilityEffect
    {
        protected BattleBall parentBall;
        
        public void SetParentBall(BattleBall parentBall) => this.parentBall = parentBall;
        
        public abstract void Activate();
        public abstract void Deactivate();
        
        public virtual void Update()
        {
            
        }
        
        public abstract AbilityEffect Clone();
    }
}