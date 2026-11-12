using System;
using Balltallion.Simulation.ActiveAbilities;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnTimer : AbilityTrigger
    {
        [SerializeField] private float cooldown;

        private float timer;

        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            timer = cooldown;
        }
        
        public override void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Trigger();
                timer = cooldown;
            }
        }
        
        public override AbilityTrigger Clone()
        {
            OnTimer clone = new OnTimer();
            clone.cooldown = cooldown;
            return clone;
        }
    }
}