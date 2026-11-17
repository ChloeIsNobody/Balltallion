using System;
using Balltallion.Simulation.Battle;
using Balltallion.Simulation.Physics;
using UnityEngine;

namespace Balltallion.Simulation.Abilities.Triggers
{
    [Serializable]
    public class OnContact : AbilityTrigger
    {
        [SerializeField] private float cooldown = 0;

        private float cooldownTimer;
        
        public override void BindToBall(BattleBall ball)
        {
            base.BindToBall(ball);
            ball.GetComponent<BallBody>().OnBallCollision += _ => OnBallCollision();
        }
        public override void Update()
        {
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;
        }

        public void OnBallCollision()
        {
            if (cooldownTimer > 0) return;
            Trigger();
            cooldownTimer = cooldown;
        }
        
        public override AbilityTrigger Clone()
        {
            OnContact clone = new OnContact();
            clone.cooldown = cooldown;
            return clone;
        }
    }
}