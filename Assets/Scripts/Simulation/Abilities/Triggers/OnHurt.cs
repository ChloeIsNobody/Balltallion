using System;
using Balltallion.Simulation.Battle;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnHurt : AbilityTrigger
    {
        [SerializeField] private float damageThreshold = 0;
        [SerializeField] private float cooldown = 0;

        private float cooldownTimer;
        
        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            ball.OnHurt += OnBallHurt;
        }

        public override void Update()
        {
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
        }

        public void OnBallHurt(float damage)
        {
            if (cooldownTimer > 0) return;
            
            if (damage >= damageThreshold)
            {
                Trigger();
                cooldownTimer = cooldown;
            }
        }
        
        public override AbilityTrigger Clone()
        {
            OnHurt clone = new OnHurt();
            clone.damageThreshold = damageThreshold;
            clone.cooldown = cooldown;
            return clone;
        }
    }
}