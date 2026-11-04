using System;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities
{
    [Serializable]
    public abstract class AbilityTrigger
    {
        public event Action OnTrigger;
        public event Action OnUntrigger;

        protected BattleBall parentBall;
        
        public void SetParentBall(BattleBall parentBall) => this.parentBall = parentBall;
        
        protected void Trigger() => OnTrigger?.Invoke();
        protected void Untrigger() => OnUntrigger?.Invoke();

        public virtual void Update()
        {
            
        }
        
        public abstract AbilityTrigger Clone();
    }
}
