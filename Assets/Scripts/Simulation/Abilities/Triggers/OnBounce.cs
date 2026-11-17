using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Physics;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnBounce : AbilityTrigger
    {
        [SerializeField] private int bouncesNeeded = 1;

        private int bounceCounter;
        
        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            ball.GetComponent<BallBody>().OnWallBounce += _ => OnWallBounce();
        }

        public void OnWallBounce()
        {
            bounceCounter++;
            if (bounceCounter >= bouncesNeeded)
            {
                Trigger();
                bounceCounter = 0;
            }
        }
        
        public override AbilityTrigger Clone()
        {
            OnBounce clone = new OnBounce();
            clone.bouncesNeeded = bouncesNeeded;
            return clone;
        }
    }
}